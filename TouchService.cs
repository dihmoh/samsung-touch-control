using System.Diagnostics;

namespace TouchToggle
{
    internal class TouchService
    {
        private string? _cachedInstanceId = null;

        public string? DetectTouchDevice()
        {
            try
            {
#pragma warning disable CA1416
                using var searcher = new System.Management.ManagementObjectSearcher(
                    "SELECT DeviceID FROM Win32_PnPEntity WHERE Status = 'OK' AND (Name LIKE '%touch screen%' OR Name LIKE '%tela touch%' OR Name LIKE '%touchscreen%')");

                foreach (System.Management.ManagementObject device in searcher.Get())
                {
                    return device["DeviceID"]?.ToString();
                }
#pragma warning restore CA1416
            }
            catch
            {
                try
                {
                    string script = @"Get-PnpDevice | Where-Object {
                        ($_.FriendlyName -like '*touch screen*' -or
                         $_.FriendlyName -like '*tela touch*' -or
                         $_.FriendlyName -like '*touchscreen*') -and
                        $_.Status -eq 'OK'
                    } | Select-Object -First 1 -ExpandProperty InstanceId";

                    string result = RunPowerShell(script);
                    return string.IsNullOrWhiteSpace(result) ? null : result.Trim();
                }
                catch { }
            }
            return null;
        }

        private string GetInstanceId(ConfigManager config)
        {
            if (_cachedInstanceId != null) return _cachedInstanceId;

            if (!string.IsNullOrWhiteSpace(config.DeviceInstanceId))
            {
                _cachedInstanceId = config.DeviceInstanceId!;
                return _cachedInstanceId;
            }

            string? detected = DetectTouchDevice();
            if (detected != null)
            {
                _cachedInstanceId = detected;
                config.DeviceInstanceId = detected;
                config.Save();
                return _cachedInstanceId;
            }

            return string.Empty;
        }

        public bool? GetTouchState(ConfigManager config)
        {
            try
            {
                string id = GetInstanceId(config);
                if (string.IsNullOrEmpty(id) || !IsValidInstanceId(id)) return null;

#pragma warning disable CA1416
                // Optimization: Directly instantiate ManagementObject via path instead of WQL Searcher
                // This bypasses the WQL parser/evaluator, reducing CPU/IPC overhead for exact PK matches.
                string queryId = id.Replace("\\", "\\\\");
                string wmiPath = $"Win32_PnPEntity.DeviceID=\"{queryId}\"";

                try
                {
                    using var device = new System.Management.ManagementObject(wmiPath);
                    device.Get(); // Throws if device doesn't exist

                    string? status = device["Status"]?.ToString();
                    if (status != null)
                    {
                        if (status.Contains("OK")) return true;
                        if (status.Contains("Error") || status.Contains("Disabled") || status.Contains("Unknown")) return false;
                    }
                }
                catch (System.Management.ManagementException)
                {
                    // Device not found or WMI error
                }
#pragma warning restore CA1416
            }
            catch
            {
                try
                {
                    string id = GetInstanceId(config);
                    if (string.IsNullOrEmpty(id) || !IsValidInstanceId(id)) return null;

                    string script = $"(Get-PnpDevice -InstanceId '{id}').Status";
                    var result = RunPowerShell(script);
                    if (result.Contains("OK")) return true;
                    if (result.Contains("Error") || result.Contains("Disabled") || result.Contains("Unknown")) return false;
                }
                catch { }
            }
            return null;
        }

        public bool SetTouchState(bool enable, ConfigManager config)
        {
            try
            {
                string id = GetInstanceId(config);
                if (string.IsNullOrEmpty(id) || !IsValidInstanceId(id)) return false;

                // Limpa o cache para forçar re-detecção após mudança de estado
                _cachedInstanceId = null;

                try
                {
#pragma warning disable CA1416
                    // Optimization: Use direct WMI path instantiation instead of Searcher.
                    // Note: WMI methods Enable/Disable are on Win32_PnPDevice, but Win32_PnPEntity
                    // is often used interchangeably or shares the same underlying provider.
                    // The original code queried Win32_PnPDevice. We'll instantiate that directly.
                    string queryId = id.Replace("\\", "\\\\");
                    string wmiPath = $"Win32_PnPDevice.DeviceID=\"{queryId}\"";

                    using var device = new System.Management.ManagementObject(wmiPath);
                    // No need to call .Get() before invoking methods if the path is fully qualified.
                    // But we'll try to invoke directly to save time.
                    string methodName = enable ? "Enable" : "Disable";
                    var result = device.InvokeMethod(methodName, null);
                    if (result != null && result.ToString() == "0") return true;
#pragma warning restore CA1416
                }
                catch { }

                string action = enable ? "Enable-PnpDevice" : "Disable-PnpDevice";
                string script = $"{action} -InstanceId '{id}' -Confirm:$false";
                int exitCode = RunPowerShellElevated(script);
                return exitCode == 0;
            }
            catch { }
            return false;
        }

        public bool ToggleTouch(bool currentState, ConfigManager config)
        {
            return SetTouchState(!currentState, config);
        }

        private bool IsValidInstanceId(string id)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(id, @"^[A-Za-z0-9\\&_\-\.\:]+$");
        }

        private readonly string _powerShellPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System),
            @"WindowsPowerShell\v1.0\powershell.exe");

        private string RunPowerShell(string script)
        {
            var psi = new ProcessStartInfo
            {
                FileName = _powerShellPath,
                Arguments = $"-NoProfile -NonInteractive -Command \"{script}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            string output = process?.StandardOutput.ReadToEnd() ?? "";
            process?.WaitForExit();
            return output.Trim();
        }

        private int RunPowerShellElevated(string script)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = _powerShellPath,
                    Arguments = $"-NoProfile -NonInteractive -Command \"{script}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var process = Process.Start(psi);
                process?.WaitForExit();
                if (process?.ExitCode == 0) return 0;
            }
            catch { }

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = _powerShellPath,
                    Arguments = $"-NoProfile -NonInteractive -Command \"{script}\"",
                    UseShellExecute = true,
                    Verb = "runas",
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                process?.WaitForExit();
                return process?.ExitCode ?? 1;
            }
            catch { }

            return 1;
        }
    }
}