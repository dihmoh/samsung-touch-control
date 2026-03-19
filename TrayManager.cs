namespace TouchToggle
{
    internal class TrayManager : IDisposable
    {
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip _menu;
        private ToolStripMenuItem _menuEnable;
        private ToolStripMenuItem _menuDisable;
        private ToolStripMenuItem _menuToggle;
        private ToolStripMenuItem _menuPanel;
        private ToolStripMenuItem _menuStartup;
        private ToolStripMenuItem _menuExit;

        private Icon _iconOn;
        private Icon _iconOff;

        public event Action? OnEnable;
        public event Action? OnDisable;
        public event Action? OnToggle;
        public event Action? OnOpenPanel;
        public event Action? OnToggleStartup;
        public event Action? OnExit;

        public TrayManager()
        {
            _menu = new ContextMenuStrip();
            _menu.Renderer = new ToolStripProfessionalRenderer();

            _menuEnable = new ToolStripMenuItem(Strings.MenuEnable);
            _menuDisable = new ToolStripMenuItem(Strings.MenuDisable);
            _menuToggle = new ToolStripMenuItem(Strings.MenuToggle);
            _menuPanel = new ToolStripMenuItem(Strings.MenuPanel);
            _menuStartup = new ToolStripMenuItem(Strings.MenuStartup);
            _menuExit = new ToolStripMenuItem(Strings.MenuExit);

            _menuEnable.Click += (s, e) => OnEnable?.Invoke();
            _menuDisable.Click += (s, e) => OnDisable?.Invoke();
            _menuToggle.Click += (s, e) => OnToggle?.Invoke();
            _menuPanel.Click += (s, e) => OnOpenPanel?.Invoke();
            _menuStartup.Click += (s, e) => OnToggleStartup?.Invoke();
            _menuExit.Click += (s, e) => OnExit?.Invoke();

            _menu.Items.Add(_menuEnable);
            _menu.Items.Add(_menuDisable);
            _menu.Items.Add(_menuToggle);
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add(_menuPanel);
            _menu.Items.Add(_menuStartup);
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add(_menuExit);

            _notifyIcon = new NotifyIcon
            {
                ContextMenuStrip = _menu,
                Visible = true,
                Text = Strings.AppName
            };

            _iconOn = CreateTouchIcon(ColorTranslator.FromHtml("#40C057"), true);
            _iconOff = CreateTouchIcon(ColorTranslator.FromHtml("#ff5050"), false);

            _notifyIcon.DoubleClick += (s, e) => OnOpenPanel?.Invoke();
            UpdateIcon(true);
        }

        public void UpdateIcon(bool touchEnabled)
        {
            _notifyIcon.Text = touchEnabled
                ? Strings.TrayEnabled
                : Strings.TrayDisabled;

            _notifyIcon.Icon = touchEnabled ? _iconOn : _iconOff;

            _menuEnable.Enabled = !touchEnabled;
            _menuDisable.Enabled = touchEnabled;
        }

        public void UpdateStartupMenu(bool startupEnabled)
        {
            _menuStartup.Checked = startupEnabled;
            _menuStartup.Text = startupEnabled
                ? Strings.MenuStartupChecked
                : Strings.MenuStartup;
        }

        public void ShowBalloon(string title, string message, int milliseconds = 3000)
        {
            _notifyIcon.BalloonTipTitle = title;
            _notifyIcon.BalloonTipText = message;
            _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            _notifyIcon.ShowBalloonTip(milliseconds);
        }

        private static Icon CreateTouchIcon(Color mainColor, bool enabled)
        {
            var bmp = new Bitmap(32, 32);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            // Desenhar botão on/off (pill / switch toggle)
            using var mainBrush = new SolidBrush(mainColor);
            
            // Fundo "pill" (arredondado)
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int h = 18;
            int w = 30;
            int y = 7;
            int x = 1;
            path.AddArc(x, y, h, h, 90, 180);
            path.AddArc(x + w - h, y, h, h, 270, 180);
            path.CloseFigure();

            g.FillPath(mainBrush, path);

            // Círculo interno branco ("knob")
            using var whiteBrush = new SolidBrush(Color.White);
            if (enabled)
            {
                g.FillEllipse(whiteBrush, x + w - h + 2, y + 2, h - 4, h - 4);
            }
            else
            {
                g.FillEllipse(whiteBrush, x + 2, y + 2, h - 4, h - 4);
            }

            return Icon.FromHandle(bmp.GetHicon());
        }

        [System.Runtime.InteropServices.DllImport("user32.dll",
            CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        public void Dispose()
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _menu.Dispose();

            if (_iconOn != null) { DestroyIcon(_iconOn.Handle); _iconOn.Dispose(); }
            if (_iconOff != null) { DestroyIcon(_iconOff.Handle); _iconOff.Dispose(); }
        }
    }

    internal static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(
            this Graphics g, Brush brush,
            float x, float y, float w, float h, float r)
        {
            using var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(x, y, r * 2, r * 2, 180, 90);
            path.AddArc(x + w - r * 2, y, r * 2, r * 2, 270, 90);
            path.AddArc(x + w - r * 2, y + h - r * 2, r * 2, r * 2, 0, 90);
            path.AddArc(x, y + h - r * 2, r * 2, r * 2, 90, 90);
            path.CloseFigure();
            g.FillPath(brush, path);
        }
    }
}