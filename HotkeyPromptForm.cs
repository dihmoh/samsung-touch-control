using System;
using System.Drawing;
using System.Windows.Forms;

namespace TouchToggle
{
    public class HotkeyPromptForm : Form
    {
        public string Modifier { get; private set; } = "";
        public string Key { get; private set; } = "";

        private Label _lblPrompt;

        public HotkeyPromptForm()
        {
            Text = "Configurar Atalho";
            Size = new Size(320, 160);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            KeyPreview = true;
            BackColor = Color.FromArgb(26, 26, 46);
            ForeColor = Color.White;
            ShowInTaskbar = false;

            _lblPrompt = new Label
            {
                Text = "Pressione a nova combinação de teclas...\n(Ex: Ctrl + Shift + A)",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            Controls.Add(_lblPrompt);

            this.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;

            // Ignorar se apenas modificar for pressionado sozinho
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey || 
                e.KeyCode == Keys.Menu || e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
                return;

            string mods = "";
            if (e.Control) mods += "Ctrl+";
            if (e.Alt) mods += "Alt+";
            if (e.Shift) mods += "Shift+";

            if (mods.EndsWith("+"))
                mods = mods.Substring(0, mods.Length - 1);

            // Requerer pelo menos um modificador
            if (string.IsNullOrEmpty(mods))
            {
                _lblPrompt.Text = "Por favor, inclua Ctrl, Alt ou Shift na combinação.";
                return;
            }

            string keyStr = e.KeyCode.ToString();
            
            // Limpar números (Ex: D1 vira 1)
            if (keyStr.StartsWith("D") && keyStr.Length == 2 && char.IsDigit(keyStr[1]))
            {
                keyStr = keyStr[1].ToString();
            }

            Modifier = mods;
            Key = keyStr;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
