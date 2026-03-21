## 2026-03-19 - [Command Injection via Config File]
**Vulnerability:** Command injection via PowerShell when interacting with DeviceInstanceId sourced from config.json.
**Learning:** The app used string interpolation for PowerShell execution with a user-controlled config value.
**Prevention:** Always validate or sanitize input before passing it to shell execution contexts like PowerShell. Used regex ^[A-Za-z0-9\\&_\-\.\:]+$ to enforce safe PnP Device IDs.

## 2024-05-24 - [Command Injection Fallback Bypass]
**Vulnerability:** A fallback path in `GetTouchState` for when WMI queries fail fell back to executing a PowerShell script passing a user-controlled `InstanceId` parameter. This code path lacked the input validation check added to the WMI query path, allowing potential command injection.
**Learning:** Security fixes applied to primary code paths can sometimes be missed in fallback or error-handling paths, leading to vulnerabilities that persist in specific environments or situations.
**Prevention:** Always verify that input validation and sanitization checks are applied uniformly across all code paths, including fallback and exception blocks that consume user input.

## 2026-03-20 - [Stored XSS / Local HTML Injection via WebView2]
**Vulnerability:** User-controlled configuration data (`HotkeyModifier` and `HotkeyKey` from `config.json`) was directly interpolated into HTML rendered by local WebView2 instance, allowing JavaScript execution.
**Learning:** Local desktop applications using web technologies (like WebView2 or Electron) are susceptible to XSS if configuration files, even when stored locally, are modified to include malicious payloads. This can lead to unauthorized actions being executed within the application context.
**Prevention:** Always sanitize any user-controlled data or configuration strings using `System.Net.WebUtility.HtmlEncode()` before embedding them into local HTML content.

## 2026-03-19 - [Binary Planting / Local Privilege Escalation via ProcessStartInfo]
**Vulnerability:** Invoking system binaries like `powershell.exe` without an absolute path leaves the application vulnerable to binary planting or path interception if the system's `PATH` environment variable or the application's working directory is compromised. This is especially critical when running processes with elevated privileges (`Verb = "runas"`).
**Learning:** `Process.Start` resolves relative executable names using the system `PATH`. A malicious actor could place a rogue `powershell.exe` in a directory that appears earlier in the `PATH` or in the working directory, leading to arbitrary code execution, potentially with elevated privileges.
**Prevention:** Always use fully qualified absolute paths for system executables. For `powershell.exe`, use `Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"WindowsPowerShell\v1.0\powershell.exe")`.
