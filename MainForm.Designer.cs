using Microsoft.Web.WebView2.WinForms;

namespace TouchToggle
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel _panelTop;
        private Label _lblTitle;
        private Label _lblSubtitle;
        private Button _btnClose;
        private Panel _panelCenter;
        private Button _btnToggle;
        private Label _lblStatus;
        private Panel _panelBottom;
        private Label _lblHotkey;
        private Panel _cardCenter;
        private WebView2 _webView;

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
            this.ClientSize = new System.Drawing.Size(360, 480);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(26, 26, 46);
            this.Name = "MainForm";
            this.Text = Strings.AppName;

            _webView = new WebView2
            {
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(360, 480),
                Dock = System.Windows.Forms.DockStyle.Fill
            };

            // Controles ocultos — mantidos para compatibilidade com MainForm.cs
            _panelTop = new Panel { Visible = false, Size = new System.Drawing.Size(1, 1) };
            _lblTitle = new Label { Visible = false };
            _lblSubtitle = new Label { Visible = false };
            _btnClose = new Button { Visible = false };
            _panelCenter = new Panel { Visible = false, Size = new System.Drawing.Size(1, 1) };
            _cardCenter = new Panel { Visible = false, Size = new System.Drawing.Size(1, 1) };
            _btnToggle = new Button { Visible = false };
            _lblStatus = new Label { Visible = false };
            _panelBottom = new Panel { Visible = false, Size = new System.Drawing.Size(1, 1) };
            _lblHotkey = new Label { Visible = false };

            this.Controls.Add(_webView);
            this.Controls.Add(_panelTop);
            this.Controls.Add(_panelCenter);
            this.Controls.Add(_panelBottom);

            this.ResumeLayout(false);
        }
    }
}