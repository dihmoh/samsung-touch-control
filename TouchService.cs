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
                // Bolt Optimization: Direct WMI Object Instantiation
                // Instead of using ManagementObjectSearcher which has to parse WQL,
                // we instantiate the ManagementObject directly by its WMI path.
                // This is significantly faster for single-object primary key lookups.
                string queryId = id.Replace("\\", "\\\\");
                try
                {
                    using var device = new System.Management.ManagementObject($"Win32_PnPEntity.DeviceID=\"{queryId}\"");
                    device.Get(); // Force population of properties

                    string? status = device["Status"]?.ToString();
                    if (status != null)
                    {
                        if (status.Contains("OK")) return true;
                        if (status.Contains("Error") || status.Contains("Disabled") || status.Contains("Unknown")) return false;
                    }
                }
                catch (System.Management.ManagementException)
                {
                    // If the device is not found, device.Get() throws an exception.
                    // This mirrors the old behavior where the searcher returned an empty collection.
                    return null;
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
                    string queryId = id.Replace("\\", "\\\\");
#pragma warning disable CA1416
                    // Bolt Optimization: Direct WMI Object Instantiation
                    // Using ManagementObject directly by WMI path instead of a WQL search
                    // to speed up single-object resolution and method invocation.
                    using var device = new System.Management.ManagementObject($"Win32_PnPDevice.DeviceID=\"{queryId}\"");
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