namespace TouchToggle
{
    public partial class MainForm : Form
    {
        private TrayManager _tray = null!;
        private TouchService _touch = null!;
        private HotkeyManager _hotkey = null!;
        private ConfigManager _config = null!;
        private bool _touchEnabled = true;

        public MainForm()
        {
            InitializeComponent();
            InitializeApp();
            ConnectUIEvents();
        }

        private void InitializeApp()
        {
            try
            {
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
                if (File.Exists(iconPath))
                    this.Icon = new Icon(iconPath);
            }
            catch { }

            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Opacity = 0;

            _config = ConfigManager.Load();
            _config.HotkeyModifier = "Ctrl+Alt";
            _config.HotkeyKey = "T";
            _config.Save();

            _touch = new TouchService();
            _tray = new TrayManager();

            var realState = _touch.GetTouchState(_config);
            _touchEnabled = realState ?? _config.TouchEnabled;
            _config.TouchEnabled = _touchEnabled;
            _config.Save();

            _tray.UpdateIcon(_touchEnabled);
            _tray.UpdateStartupMenu(_config.GetStartWithWindows());
            UpdateUI();

            _tray.OnEnable += () => SetTouch(true);
            _tray.OnDisable += () => SetTouch(false);
            _tray.OnToggle += ToggleTouch;
            _tray.OnOpenPanel += OpenPanel;
            _tray.OnToggleStartup += ToggleStartup;
            _tray.OnExit += ExitApp;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            _hotkey = new HotkeyManager(this.Handle);
            _hotkey.HotkeyPressed += ToggleTouch;
            bool ok = _hotkey.Register(_config.HotkeyModifier, _config.HotkeyKey);

            Task.Delay(1500).ContinueWith(_ =>
            {
                this.Invoke(() =>
                {
                    if (ok)
                        _tray.ShowBalloon(Strings.BalloonStarted,
                            Strings.BalloonHotkey(_config.HotkeyModifier, _config.HotkeyKey));
                    else
                        _tray.ShowBalloon(Strings.AppName,
                            Strings.BalloonHotkeyFailed);
                });
            });
        }

        private void ConnectUIEvents()
        {
            _btnToggle.Click += (s, e) => ToggleTouch();

            _panelTop.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    NativeMethods.ReleaseCapture();
                    NativeMethods.SendMessage(this.Handle, 0xA1, 2, 0);
                }
            };
        }

        private Region? _btnToggleRegion;

        private void UpdateUI()
        {
            if (_btnToggle == null || _lblStatus == null) return;

            _btnToggle.Text = _touchEnabled ? "ON" : "OFF";
            _btnToggle.BackColor = _touchEnabled
                ? Color.FromArgb(0, 120, 215)
                : Color.FromArgb(180, 0, 0);

            _lblStatus.Text = _touchEnabled
                ? Strings.StatusEnabled
                : Strings.StatusDisabled;

            _lblHotkey.Text = Strings.HotkeyLabel(_config.HotkeyModifier, _config.HotkeyKey);

            if (_btnToggleRegion == null)
            {
                var path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddEllipse(0, 0, _btnToggle.Width, _btnToggle.Height);
                _btnToggleRegion = new Region(path);
                _btnToggle.Region = _btnToggleRegion;
            }
        }

        private void SetTouch(bool enable)
        {
            bool success = _touch.SetTouchState(enable, _config);
            if (success)
            {
                _touchEnabled = enable;
                _config.TouchEnabled = enable;
                _config.Save();
                _tray.UpdateIcon(_touchEnabled);
                UpdateUI();
                OverlayPopup.ShowPopup(_touchEnabled);
            }
        }

        private void ToggleTouch()
        {
            SetTouch(!_touchEnabled);
        }

        private void ToggleStartup()
        {
            bool current = _config.GetStartWithWindows();
            bool success = _config.SetStartWithWindows(!current);
            if (success)
            {
                _tray.UpdateStartupMenu(!current);
                _tray.ShowBalloon(Strings.AppName,
                    !current
                        ? Strings.BalloonStartupOn
                        : Strings.BalloonStartupOff);
            }
        }

        private void OpenPanel()
        {
            this.Opacity = 1;
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.Show();
            this.BringToFront();
        }

        private void ExitApp()
        {
            _config.Save();
            _hotkey?.Unregister();
            _tray.Dispose();
            _btnToggleRegion?.Dispose();
            Application.Exit();
        }

        protected override void WndProc(ref Message m)
        {
            _hotkey?.ProcessMessage(m);
            base.WndProc(ref m);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                this.ShowInTaskbar = false;
                this.Opacity = 0;
            }
            else
            {
                base.OnFormClosing(e);
            }
        }
    }

    internal static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int ReleaseCapture();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    }
}