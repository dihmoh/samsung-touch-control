## $(date +%Y-%m-%d) - [Command Injection via Config File]
**Vulnerability:** Command injection via PowerShell when interacting with DeviceInstanceId sourced from config.json.
**Learning:** The app used string interpolation for PowerShell execution with a user-controlled config value.
**Prevention:** Always validate or sanitize input before passing it to shell execution contexts like PowerShell. Used regex ^[A-Za-z0-9\\&_\-\.\:]+$ to enforce safe PnP Device IDs.

## 2024-05-24 - [Command Injection Fallback Bypass]
**Vulnerability:** A fallback path in `GetTouchState` for when WMI queries fail fell back to executing a PowerShell script passing a user-controlled `InstanceId` parameter. This code path lacked the input validation check added to the WMI query path, allowing potential command injection.
**Learning:** Security fixes applied to primary code paths can sometimes be missed in fallback or error-handling paths, leading to vulnerabilities that persist in specific environments or situations.
**Prevention:** Always verify that input validation and sanitization checks are applied uniformly across all code paths, including fallback and exception blocks that consume user input.
