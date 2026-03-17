using System.Globalization;

namespace TouchToggle
{
    internal static class Strings
    {
        private static bool _isPtBr = CultureInfo.CurrentUICulture.Name.StartsWith("pt", StringComparison.OrdinalIgnoreCase);

        // App
        public static string AppName => "Samsung Touch Control";
        public static string Subtitle => _isPtBr ? "Samsung Galaxy Book 3 360" : "Samsung Galaxy Book 3 360";

        // Tray
        public static string TouchEnabled => _isPtBr ? "Touch Ativado" : "Touch Enabled";
        public static string TouchDisabled => _isPtBr ? "Touch Desativado" : "Touch Disabled";
        public static string TrayEnabled => _isPtBr ? "Samsung Touch Control — Touch Ativado" : "Samsung Touch Control — Touch Enabled";
        public static string TrayDisabled => _isPtBr ? "Samsung Touch Control — Touch Desativado" : "Samsung Touch Control — Touch Disabled";

        // Menu
        public static string MenuEnable => _isPtBr ? "✅  Ativar Touch" : "✅  Enable Touch";
        public static string MenuDisable => _isPtBr ? "🚫  Desativar Touch" : "🚫  Disable Touch";
        public static string MenuToggle => _isPtBr ? "🔄  Alternar Touch" : "🔄  Toggle Touch";
        public static string MenuPanel => _isPtBr ? "⚙️  Abrir Painel" : "⚙️  Open Panel";
        public static string MenuStartup => _isPtBr ? "🚀  Iniciar com o Windows" : "🚀  Start with Windows";
        public static string MenuStartupChecked => _isPtBr ? "🚀  Iniciar com o Windows ✓" : "🚀  Start with Windows ✓";
        public static string MenuExit => _isPtBr ? "❌  Sair" : "❌  Exit";

        // Balloon
        public static string BalloonStarted => _isPtBr ? "Samsung Touch Control iniciado!" : "Samsung Touch Control started!";
        public static string BalloonHotkey(string modifier, string key) =>
            _isPtBr ? $"Atalho: {modifier}+{key}" : $"Shortcut: {modifier}+{key}";
        public static string BalloonHotkeyFailed => _isPtBr ? "Atalho não pôde ser registrado." : "Shortcut could not be registered.";
        public static string BalloonStartupOn => _isPtBr ? "✅ Será iniciado com o Windows!" : "✅ Will start with Windows!";
        public static string BalloonStartupOff => _isPtBr ? "❌ Removido da inicialização do Windows." : "❌ Removed from Windows startup.";

        // Popup
        public static string PopupEnabled => _isPtBr ? "Touch Ativado" : "Touch Enabled";
        public static string PopupDisabled => _isPtBr ? "Touch Desativado" : "Touch Disabled";
        public static string PopupSubEnabled => _isPtBr ? "Touchscreen habilitado" : "Touchscreen enabled";
        public static string PopupSubDisabled => _isPtBr ? "Touchscreen desabilitado" : "Touchscreen disabled";

        // UI
        public static string StatusEnabled => _isPtBr ? "Touch Ativado" : "Touch Enabled";
        public static string StatusDisabled => _isPtBr ? "Touch Desativado" : "Touch Disabled";
        public static string HotkeyLabel(string modifier, string key) =>
            _isPtBr ? $"⌨️  Atalho: {modifier}+{key}" : $"⌨️  Shortcut: {modifier}+{key}";
    }
}