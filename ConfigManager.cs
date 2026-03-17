using System.Text.Json;
using Microsoft.Win32;

namespace TouchToggle
{
    internal class ConfigManager
    {
        private static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SamsungTouchControl", "config.json");

        private const string RegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "Samsung Touch Control";

        public bool TouchEnabled { get; set; } = true;
        public string HotkeyModifier { get; set; } = "Ctrl+Alt";
        public string HotkeyKey { get; set; } = "T";
        public bool StartWithWindows { get; set; } = false;
        public string? DeviceInstanceId { get; set; } = null;

        public static ConfigManager Load()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    return JsonSerializer.Deserialize<ConfigManager>(json) ?? new ConfigManager();
                }
            }
            catch { }
            return new ConfigManager();
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigPath, json);
            }
            catch { }
        }

        public bool GetStartWithWindows()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegistryKey, false);
                return key?.GetValue(AppName) != null;
            }
            catch { }
            return false;
        }

        public bool SetStartWithWindows(bool enable)
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegistryKey, true);
                if (key == null) return false;

                if (enable)
                {
                    string exePath = Application.ExecutablePath;
                    key.SetValue(AppName, $"\"{exePath}\"");
                }
                else
                {
                    key.DeleteValue(AppName, false);
                }

                StartWithWindows = enable;
                Save();
                return true;
            }
            catch { }
            return false;
        }
    }
}