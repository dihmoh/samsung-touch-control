namespace TouchToggle
{
    public partial class OverlayPopup : Form
    {
        private System.Windows.Forms.Timer _timer = null!;
        private int _opacity100 = 0;
        private bool _fadingOut = false;

        public OverlayPopup(bool touchEnabled)
        {
            InitializeComponent();
            SetupForm(touchEnabled);
            SetupTimer();
        }

        private void SetupForm(bool touchEnabled)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.Opacity = 0;
            this.Size = new Size(320, 90);

            var screen = Screen.PrimaryScreen!.WorkingArea;
            this.Location = new Point(
                screen.Right - this.Width - 20,
                screen.Bottom - this.Height - 20);

            string emoji = touchEnabled ? "👆" : "🚫";
            string status = touchEnabled ? "Touch Ativado" : "Touch Desativado";
            string sub = touchEnabled ? "Touchscreen habilitado" : "Touchscreen desabilitado";

            var lblEmoji = new Label
            {
                Text = emoji,
                Font = new Font("Segoe UI Emoji", 24f),
                ForeColor = Color.White,
                Location = new Point(15, 15),
                Size = new Size(55, 55),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblTitle = new Label
            {
                Text = status,
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(75, 15),
                Size = new Size(230, 28),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblSub = new Label
            {
                Text = sub,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(180, 180, 180),
                Location = new Point(77, 45),
                Size = new Size(230, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var accent = new Panel
            {
                BackColor = touchEnabled
                    ? Color.FromArgb(0, 120, 215)
                    : Color.FromArgb(180, 0, 0),
                Location = new Point(0, 0),
                Size = new Size(5, 90)
            };

            this.Controls.Add(accent);
            this.Controls.Add(lblEmoji);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSub);
        }

        private void SetupTimer()
        {
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 30;
            _timer.Tick += FadeIn_Tick;
            _timer.Start();
        }

        private void FadeIn_Tick(object? sender, EventArgs e)
        {
            _opacity100 += 5;
            this.Opacity = Math.Min(_opacity100 / 100.0, 0.95);
            if (_opacity100 >= 95)
            {
                _timer.Tick -= FadeIn_Tick;
                _timer.Interval = 2500;
                _timer.Tick += Hold_Tick;
            }
        }

        private void Hold_Tick(object? sender, EventArgs e)
        {
            _timer.Tick -= Hold_Tick;
            _timer.Interval = 30;
            _timer.Tick += FadeOut_Tick;
        }

        private void FadeOut_Tick(object? sender, EventArgs e)
        {
            _opacity100 -= 4;
            this.Opacity = Math.Max(_opacity100 / 100.0, 0);
            if (_opacity100 <= 0)
            {
                _timer.Stop();
                this.Close();
            }
        }

        public static void ShowPopup(bool touchEnabled)
        {
            var popup = new OverlayPopup(touchEnabled);
            popup.Show();
        }
    }
}