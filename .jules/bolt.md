## 2024-03-14 - WMI Queries vs PowerShell Process Spawning
**Learning:** Spawning a new `powershell.exe` process via `Process.Start` is incredibly slow (often 500-2000ms) because it has to load the .NET runtime, PowerShell engine, and required modules (like PnP). This is a major bottleneck on the cold path (startup) and hot paths.
**Action:** When querying system information like `Get-PnpDevice`, always prefer direct WMI/CIM queries using `System.Management.ManagementObjectSearcher` (takes ~20-50ms). Keep PowerShell as a fallback inside a try-catch block for robustness in case WMI is corrupted or permissions are missing. Note that WQL requires escaping backslashes in queries (e.g. `DeviceID = '...'`).

## 2024-06-25 - WQL Server-Side Filtering
**Learning:** Using client-side C# string parsing to filter results from a broad WMI query (e.g., `SELECT DeviceID, Name FROM Win32_PnPEntity WHERE Status = 'OK'`) forces the system to instantiate and transfer many COM objects over IPC, which is slow and memory-intensive.
**Action:** Always prefer server-side filtering using the `LIKE` or `=` operators directly in the WQL query (e.g., `SELECT DeviceID FROM Win32_PnPEntity WHERE Status = 'OK' AND Name LIKE '%touch screen%'`). This drastically reduces the number of instantiated objects and speeds up execution.

## 2024-11-20 - Direct WMI Object Instantiation vs WQL Queries
**Learning:** Using `ManagementObjectSearcher` with a WQL `WHERE` clause for primary key lookups (e.g., querying by `DeviceID`) incurs unnecessary WQL parsing and query evaluator overhead before finding the single object.
**Action:** For single-object primary key lookups, instantiate the `ManagementObject` directly using the exact WMI path (e.g., `new ManagementObject($"Win32_PnPDevice.DeviceID=\"{queryId}\"")`). This completely bypasses the WQL evaluator overhead and directly resolves the target WMI object, significantly speeding up methods like GetTouchState and SetTouchState.
