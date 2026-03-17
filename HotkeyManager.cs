using System.Runtime.InteropServices;

namespace TouchToggle
{
    internal class HotkeyManager : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 9000;
        private const uint MOD_CTRL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_WIN = 0x0008;

        private IntPtr _handle;
        private bool _registered = false;

        public event Action? HotkeyPressed;

        public HotkeyManager(IntPtr windowHandle)
        {
            _handle = windowHandle;
        }

        public bool Register(string modifier, string key)
        {
            Unregister();

            uint mod = ParseModifier(modifier);
            uint vk = ParseKey(key);

            if (vk == 0) return false;

            _registered = RegisterHotKey(_handle, HOTKEY_ID, mod, vk);
            return _registered;
        }

        public void Unregister()
        {
            if (_registered)
            {
                UnregisterHotKey(_handle, HOTKEY_ID);
                _registered = false;
            }
        }

        public void ProcessMessage(Message m)
        {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == HOTKEY_ID)
                HotkeyPressed?.Invoke();
        }

        private uint ParseModifier(string modifier)
        {
            uint mod = 0;
            if (modifier.Contains("Ctrl")) mod |= MOD_CTRL;
            if (modifier.Contains("Shift")) mod |= MOD_SHIFT;
            if (modifier.Contains("Alt")) mod |= MOD_ALT;
            if (modifier.Contains("Win")) mod |= MOD_WIN;
            return mod;
        }

        private uint ParseKey(string key)
        {
            if (key.Length == 1 && char.IsLetter(key[0]))
                return (uint)char.ToUpper(key[0]);

            return key.ToUpper() switch
            {
                "F1" => 0x70,
                "F2" => 0x71,
                "F3" => 0x72,
                "F4" => 0x73,
                "F5" => 0x74,
                "F6" => 0x75,
                "F7" => 0x76,
                "F8" => 0x77,
                "F9" => 0x78,
                "F10" => 0x79,
                "F11" => 0x7A,
                "F12" => 0x7B,
                _ => 0
            };
        }

        public void Dispose()
        {
            Unregister();
        }
    }
}