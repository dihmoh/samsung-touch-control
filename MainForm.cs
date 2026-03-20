using Microsoft.Web.WebView2.Core;

namespace TouchToggle
{
    public partial class MainForm : Form
    {
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private TrayManager _tray = null!;
        private TouchService _touch = null!;
        private HotkeyManager _hotkey = null!;
        private ConfigManager _config = null!;
        private bool _touchEnabled = true;
        private bool _webViewReady = false;

        public MainForm()
        {
            InitializeComponent();
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 18, 18));
            InitializeApp();
            InitializeWebView();
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
            if (string.IsNullOrEmpty(_config.HotkeyModifier) || string.IsNullOrEmpty(_config.HotkeyKey))
            {
                _config.HotkeyModifier = "Ctrl+Alt";
                _config.HotkeyKey = "T";
                _config.Save();
            }

            _touch = new TouchService();
            _tray = new TrayManager();

            var realState = _touch.GetTouchState(_config);
            _touchEnabled = realState ?? _config.TouchEnabled;

            // ✅ CORREÇÃO: Sempre reativa o touch ao iniciar
            // Evita que o Windows mantenha o touch desativado após reiniciar
            if (!_touchEnabled)
            {
                bool restored = _touch.SetTouchState(true, _config);
                if (restored)
                {
                    _touchEnabled = true;
                }
            }

            _config.TouchEnabled = _touchEnabled;
            _config.Save();

            _tray.UpdateIcon(_touchEnabled);
            _tray.UpdateStartupMenu(_config.GetStartWithWindows());

            _tray.OnEnable += () => SetTouch(true);
            _tray.OnDisable += () => SetTouch(false);
            _tray.OnToggle += ToggleTouch;
            _tray.OnOpenPanel += OpenPanel;
            _tray.OnToggleStartup += ToggleStartup;
            _tray.OnExit += ExitApp;
        }

        private async void InitializeWebView()
        {
            try
            {
                string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TouchToggle", "WebView2");
                var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await _webView.EnsureCoreWebView2Async(env);

                _webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
                _webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                _webView.CoreWebView2.Settings.IsZoomControlEnabled = false;

                _webView.CoreWebView2.WebMessageReceived += OnWebMessage;
                _webViewReady = true;

                LoadPanel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao carregar a interface (WebView2).\n" + ex.Message,
                    "Componente Ausente",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void LoadPanel()
        {
            bool on = _touchEnabled;
            string accentColor = on ? "#1259c3" : "#2c2c4d";
            string statusText = on ? Strings.StatusEnabled : Strings.StatusDisabled;
            string statusColor = on ? "#64a0ff" : "#dc5050";
            string hotkeyText = Strings.HotkeyLabel(_config.HotkeyModifier, _config.HotkeyKey)
                .Replace("⌨️  Atalho: ", "Atalho: ")
                .Replace("⌨️  Shortcut: ", "Shortcut: ");
            string touchIcon = on ? GetTouchIconSvg() : GetTouchOffIconSvg();
            string startupBg = _config.GetStartWithWindows() ? "#1259c3" : "rgba(255,255,255,0.2)";
            string knobLeft = _config.GetStartWithWindows() ? "23px" : "3px";

            string html = $@"<!DOCTYPE html>
<html>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1'>
<style>
  * {{ margin: 0; padding: 0; box-sizing: border-box; user-select: none; }}
  
  body {{
    width: 100vw;
    height: 100vh;
    background: #1a1a2e;
    font-family: 'Segoe UI', system-ui, sans-serif;
    overflow: hidden;
    display: flex;
    flex-direction: column;
  }}

  .header {{
    background: {accentColor};
    height: 72px;
    display: flex;
    align-items: center;
    padding: 0 20px;
    transition: background 0.3s ease;
    -webkit-app-region: drag;
  }}

  .header-text {{ flex: 1; }}
  .header h1 {{ font-size: 16px; font-weight: 700; color: white; letter-spacing: 0.2px; }}
  .header p {{ font-size: 12px; color: rgba(255,255,255,0.7); margin-top: 2px; }}

  .btn-close {{
    width: 28px; height: 28px;
    border-radius: 50%;
    background: rgba(255,255,255,0.15);
    border: none; color: white; font-size: 14px;
    cursor: pointer; display: flex; align-items: center; justify-content: center;
    transition: background 0.15s;
    -webkit-app-region: no-drag;
  }}
  .btn-close:hover {{ background: rgba(255,255,255,0.25); }}
  .btn-close:active {{ background: rgba(255,255,255,0.35); }}

  .content-area {{
    padding: 16px; flex: 1; display: flex; flex-direction: column; gap: 12px;
  }}

  .card {{
    background: #252542;
    border-radius: 18px;
    padding: 20px;
  }}

  .card-main {{
    display: flex; flex-direction: column; align-items: center; gap: 16px;
    padding-top: 28px; padding-bottom: 20px;
  }}

  .ring-container {{
    width: 140px; height: 140px;
    display: flex; align-items: center; justify-content: center;
    border-radius: 50%;
    border: 2px solid {statusColor}44;
  }}

  .btn-toggle {{
    width: 110px; height: 110px;
    border-radius: 50%;
    background: {accentColor};
    border: none; cursor: pointer;
    display: flex; align-items: center; justify-content: center;
    transition: background 0.3s, transform 0.1s;
  }}
  .btn-toggle:hover {{ filter: brightness(1.1); transform: scale(1.02); }}
  .btn-toggle:active {{ transform: scale(0.95); }}
  .btn-toggle svg {{ width: 48px; height: 48px; }}

  .status {{
    font-size: 15px; font-weight: 600; color: {statusColor};
    letter-spacing: 0.3px;
  }}

  .hotkey-container {{
    font-size: 12px; color: rgba(255,255,255,0.5);
    display: flex; align-items: center; gap: 6px;
    padding: 6px 14px;
    background: rgba(255,255,255,0.06);
    border-radius: 8px; cursor: pointer; transition: 0.2s;
  }}
  .hotkey-container:hover {{ background: rgba(255,255,255,0.12); color: rgba(255,255,255,0.8); }}
  .btn-close:focus-visible, .btn-toggle:focus-visible, .hotkey-container:focus-visible, .card-startup:focus-visible {{
    outline: 2px solid white; outline-offset: 2px;
  }}

  .card-startup {{
    display: flex; justify-content: space-between; align-items: center;
    padding: 16px 20px; cursor: pointer; border-radius: 18px;
    transition: background 0.2s;
  }}
  .card-startup:hover {{ background: #2c2c4d; }}
  .card-startup:active {{ background: #1e1e36; }}

  .text-col {{ display: flex; flex-direction: column; }}
  .text-title {{ font-size: 14px; font-weight: 600; color: white; }}
  .text-sub {{ font-size: 12px; color: rgba(255,255,255,0.5); margin-top: 2px; }}

  .toggle-switch {{
    width: 44px; height: 24px;
    background: {startupBg};
    border-radius: 12px; position: relative; transition: 0.3s;
  }}
  .toggle-knob {{
    width: 18px; height: 18px; background: white; border-radius: 50%;
    position: absolute; top: 3px; left: {knobLeft};
    transition: 0.3s;
  }}
</style>
</head>
<body>

<div class='header'>
  <div class='header-text'>
    <h1>Samsung Touch Control</h1>
    <p>Samsung Galaxy Book 3 360</p>
  </div>
  <button class='btn-close' onclick=""window.chrome.webview.postMessage('close')"" aria-label='{Strings.AccessibleClose}'>✕</button>
</div>

<div class='content-area'>
  <div class='card card-main'>
    <div class='ring-container'>
      <button class='btn-toggle' onclick=""window.chrome.webview.postMessage('toggle')"" aria-label='{Strings.AccessibleToggle}' aria-pressed='{on.ToString().ToLower()}'>
        {touchIcon}
      </button>
    </div>
    <div class='status'>{statusText}</div>
    <div class='hotkey-container' role='button' tabindex='0' onclick=""window.chrome.webview.postMessage('change_hotkey')"" onkeydown=""if(event.key==='Enter'||event.key===' ') window.chrome.webview.postMessage('change_hotkey')"">
      ⌨ {hotkeyText} ✏️
    </div>
  </div>

  <div class='card card-startup' role='switch' tabindex='0' aria-checked='{_config.GetStartWithWindows().ToString().ToLower()}' onclick=""window.chrome.webview.postMessage('startup')"" onkeydown=""if(event.key==='Enter'||event.key===' ') window.chrome.webview.postMessage('startup')"">
    <div class='text-col'>
      <span class='text-title'>Inicialização</span>
      <span class='text-sub'>Iniciar com o Windows</span>
    </div>
    <div class='toggle-switch'>
      <div class='toggle-knob'></div>
    </div>
  </div>
</div>

</body>
</html>";

            _webView.NavigateToString(html);
        }

        private string GetTouchIconSvg() => @"
<svg viewBox='0 0 64 64' fill='white' xmlns='http://www.w3.org/2000/svg'>
  <path d='M 18 16 C 9.187124 16 2 23.187124 2 32 C 2 40.812876 9.187124 48 18 48 L 46 48 C 54.812876 48 62 40.812876 62 32 C 62 23.187124 54.812876 16 46 16 L 18 16 z M 46 20 C 46.414187 20 46.823177 20.021529 47.226562 20.0625 C 47.227844 20.062629 47.229188 20.062371 47.230469 20.0625 C 52.874744 20.637623 57.362377 25.125256 57.9375 30.769531 C 57.978731 31.174171 58 31.584485 58 32 C 58 32.414187 57.978471 32.823177 57.9375 33.226562 C 57.937371 33.227844 57.937629 33.229188 57.9375 33.230469 C 57.362377 38.874744 52.874744 43.362377 47.230469 43.9375 C 46.825829 43.978731 46.415515 44 46 44 C 39.373 44 34 38.627 34 32 C 34 25.373 39.373 20 46 20 z'/>
</svg>";

        private string GetTouchOffIconSvg() => @"
<svg viewBox='0 0 64 64' fill='#ff5050' style='transform: scaleX(-1);' xmlns='http://www.w3.org/2000/svg'>
  <path d='M 18 16 C 9.187124 16 2 23.187124 2 32 C 2 40.812876 9.187124 48 18 48 L 46 48 C 54.812876 48 62 40.812876 62 32 C 62 23.187124 54.812876 16 46 16 L 18 16 z M 46 20 C 46.414187 20 46.823177 20.021529 47.226562 20.0625 C 47.227844 20.062629 47.229188 20.062371 47.230469 20.0625 C 52.874744 20.637623 57.362377 25.125256 57.9375 30.769531 C 57.978731 31.174171 58 31.584485 58 32 C 58 32.414187 57.978471 32.823177 57.9375 33.226562 C 57.937371 33.227844 57.937629 33.229188 57.9375 33.230469 C 57.362377 38.874744 52.874744 43.362377 47.230469 43.9375 C 46.825829 43.978731 46.415515 44 46 44 C 39.373 44 34 38.627 34 32 C 34 25.373 39.373 20 46 20 z'/>
</svg>";

        private void OnWebMessage(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string msg = e.TryGetWebMessageAsString();
            this.Invoke(() =>
            {
                switch (msg)
                {
                    case "toggle": ToggleTouch(); break;
                    case "startup": ToggleStartup(); UpdateUI(); break;
                    case "exit": ExitApp(); break;
                    case "change_hotkey": ChangeHotkeyPrompt(); break;
                    case "close":
                        this.Hide();
                        this.ShowInTaskbar = false;
                        this.Opacity = 0;
                        break;
                }
            });
        }

        private void ChangeHotkeyPrompt()
        {
            using var form = new HotkeyPromptForm();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                _hotkey?.Unregister();
                bool ok = true;
                if (_hotkey != null)
                    ok = _hotkey.Register(form.Modifier, form.Key);

                if (ok)
                {
                    _config.HotkeyModifier = form.Modifier;
                    _config.HotkeyKey = form.Key;
                    _config.Save();
                    LoadPanel();
                }
                else
                {
                    MessageBox.Show("Não foi possível registrar este atalho.", "Erro de Atalho", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _hotkey?.Register(_config.HotkeyModifier, _config.HotkeyKey);
                }
            }
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

        private void UpdateUI()
        {
            if (_webViewReady)
                this.Invoke(() => LoadPanel());
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Hide();
                this.ShowInTaskbar = false;
                this.Opacity = 0;
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
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