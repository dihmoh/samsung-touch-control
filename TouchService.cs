using System.Diagnostics;

namespace TouchToggle
{
    internal class TouchService
    {
        private const string InstanceId = "HID\\ELAN902C&COL01\\5&292CAA11&0&0000";

        public bool? GetTouchState()
        {
            try
            {
                string script = $"(Get-PnpDevice -InstanceId '{InstanceId}').Status";
                var result = RunPowerShell(script);
                if (result.Contains("OK")) return true;
                if (result.Contains("Error") || result.Contains("Disabled") || result.Contains("Unknown")) return false;
            }
            catch { }
            return null;
        }

        public bool SetTouchState(bool enable)
        {
            try
            {
                string action = enable ? "Enable-PnpDevice" : "Disable-PnpDevice";
                string script = $"{action} -InstanceId '{InstanceId}' -Confirm:$false";
                int exitCode = RunPowerShellElevated(script);
                return exitCode == 0;
            }
            catch { }
            return false;
        }

        public bool ToggleTouch(bool currentState)
        {
            return SetTouchState(!currentState);
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
            // Tenta primeiro sem elevação (se já for admin)
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

            // Se falhar, tenta com elevação
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