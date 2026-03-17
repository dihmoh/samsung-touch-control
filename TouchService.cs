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

                string script = $"(Get-PnpDevice -InstanceId '{id}').Status";
                var result = RunPowerShell(script);
                if (result.Contains("OK")) return true;
                if (result.Contains("Error") || result.Contains("Disabled") || result.Contains("Unknown")) return false;
            }
            catch { }
            return null;
        }

        public bool SetTouchState(bool enable, ConfigManager config)
        {
            try
            {
                string id = GetInstanceId(config);
                if (string.IsNullOrEmpty(id) || !IsValidInstanceId(id)) return false;

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

        // Security Enhancement: Validate InstanceId to prevent PowerShell command injection
        private bool IsValidInstanceId(string id)
        {
            // PnP Device IDs typically contain alphanumeric characters, backslashes, ampersands, underscores, and hyphens.
            // Single quotes, semicolons, and other shell operators are strictly prohibited.
            return System.Text.RegularExpressions.Regex.IsMatch(id, @"^[A-Za-z0-9\\&_\-\.\:]+$");
        }

        private string RunPowerShell(string script)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
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
                    FileName = "powershell.exe",
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
                    FileName = "powershell.exe",
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