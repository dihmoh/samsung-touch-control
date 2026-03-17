## $(date +%Y-%m-%d) - [Command Injection via Config File]
**Vulnerability:** Command injection via PowerShell when interacting with DeviceInstanceId sourced from config.json.
**Learning:** The app used string interpolation for PowerShell execution with a user-controlled config value.
**Prevention:** Always validate or sanitize input before passing it to shell execution contexts like PowerShell. Used regex ^[A-Za-z0-9\\&_\-\.\:]+$ to enforce safe PnP Device IDs.
