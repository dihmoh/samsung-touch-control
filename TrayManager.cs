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

            _notifyIcon.DoubleClick += (s, e) => OnOpenPanel?.Invoke();
            UpdateIcon(true);
        }

        public void UpdateIcon(bool touchEnabled)
        {
            _notifyIcon.Text = touchEnabled
                ? Strings.TrayEnabled
                : Strings.TrayDisabled;

            _notifyIcon.Icon = touchEnabled
                ? CreateIcon("ON", Color.FromArgb(0, 120, 215))
                : CreateIcon("OFF", Color.FromArgb(180, 0, 0));

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

        private Icon CreateIcon(string text, Color bgColor)
        {
            var bmp = new Bitmap(32, 32);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.Transparent);

            using var brush = new SolidBrush(bgColor);
            g.FillEllipse(brush, 1, 1, 30, 30);

            using var font = new Font("Arial", text == "ON" ? 10f : 8f, FontStyle.Bold);
            using var textBrush = new SolidBrush(Color.White);
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString(text, font, textBrush, new RectangleF(0, 0, 32, 32), sf);

            return Icon.FromHandle(bmp.GetHicon());
        }

        public void Dispose()
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _menu.Dispose();
        }
    }
}