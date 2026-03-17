namespace TouchToggle
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel _panelTop;
        private Label _lblTitle;
        private Label _lblSubtitle;
        private Panel _panelCenter;
        private Button _btnToggle;
        private Label _lblStatus;
        private Panel _panelBottom;
        private Label _lblHotkey;
        private Button _btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 280);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(24, 24, 24);
            this.Name = "MainForm";
            this.Text = Strings.AppName;

            _panelTop = new Panel
            {
                BackColor = System.Drawing.Color.FromArgb(0, 120, 215),
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(360, 60)
            };

            _lblTitle = new Label
            {
                Text = Strings.AppName,
                Font = new System.Drawing.Font("Segoe UI", 14f, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                Location = new System.Drawing.Point(15, 8),
                Size = new System.Drawing.Size(300, 32),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            _lblSubtitle = new Label
            {
                Text = Strings.Subtitle,
                Font = new System.Drawing.Font("Segoe UI", 8f),
                ForeColor = System.Drawing.Color.FromArgb(200, 230, 255),
                Location = new System.Drawing.Point(17, 38),
                Size = new System.Drawing.Size(250, 16),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            _btnClose = new Button
            {
                Text = "✕",
                Font = new System.Drawing.Font("Segoe UI", 12f),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                Location = new System.Drawing.Point(320, 5),
                Size = new System.Drawing.Size(35, 35),
                BackColor = System.Drawing.Color.Transparent,
                Cursor = System.Windows.Forms.Cursors.Hand,
                AccessibleName = Strings.AccessibleClose,
                AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
            };
            _btnClose.FlatAppearance.BorderSize = 0;
            _btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(120, 255, 255, 255);
            _btnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(80, 255, 255, 255);
            _btnClose.Click += (s, e) => this.Hide();

            _panelTop.Controls.Add(_lblTitle);
            _panelTop.Controls.Add(_lblSubtitle);
            _panelTop.Controls.Add(_btnClose);

            _panelCenter = new Panel
            {
                BackColor = System.Drawing.Color.FromArgb(24, 24, 24),
                Location = new System.Drawing.Point(0, 60),
                Size = new System.Drawing.Size(360, 160)
            };

            _btnToggle = new Button
            {
                Text = "ON",
                Font = new System.Drawing.Font("Segoe UI", 22f, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                Size = new System.Drawing.Size(110, 110),
                Location = new System.Drawing.Point(125, 25),
                BackColor = System.Drawing.Color.FromArgb(0, 120, 215),
                Cursor = System.Windows.Forms.Cursors.Hand,
                AccessibleName = Strings.AccessibleToggle,
                AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
            };
            _btnToggle.FlatAppearance.BorderSize = 0;

            _lblStatus = new Label
            {
                Text = Strings.StatusEnabled,
                Font = new System.Drawing.Font("Segoe UI", 10f),
                ForeColor = System.Drawing.Color.FromArgb(180, 180, 180),
                Location = new System.Drawing.Point(0, 140),
                Size = new System.Drawing.Size(360, 20),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            _panelCenter.Controls.Add(_btnToggle);
            _panelCenter.Controls.Add(_lblStatus);

            _panelBottom = new Panel
            {
                BackColor = System.Drawing.Color.FromArgb(18, 18, 18),
                Location = new System.Drawing.Point(0, 220),
                Size = new System.Drawing.Size(360, 60)
            };

            _lblHotkey = new Label
            {
                Text = Strings.HotkeyLabel("Ctrl+Alt", "T"),
                Font = new System.Drawing.Font("Segoe UI", 9f),
                ForeColor = System.Drawing.Color.FromArgb(140, 140, 140),
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(360, 60),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            _panelBottom.Controls.Add(_lblHotkey);

            this.Controls.Add(_panelTop);
            this.Controls.Add(_panelCenter);
            this.Controls.Add(_panelBottom);

            this.ResumeLayout(false);
        }
    }
}