using System;
using System.Windows.Forms;

namespace Programming_technologies_6
{
    public enum GameMode
    {
        Novice,
        Experienced,
        Pro,
        Nightmare
    }

    public class GameModeForm : Form
    {
        public GameMode? SelectedMode = null;
        public GameModeForm()
        {
            this.Text = "Выберите режим игры";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Width = 350;
            this.Height = 300;

            var btnNovice = new Button { Text = "Новичок", Width = 250, Height = 40, Top = 20, Left = 40 };
            var btnExperienced = new Button { Text = "Бывалый", Width = 250, Height = 40, Top = 70, Left = 40 };
            var btnPro = new Button { Text = "Про", Width = 250, Height = 40, Top = 120, Left = 40 };
            var btnNightmare = new Button { Text = "КОШМАР!", Width = 250, Height = 40, Top = 170, Left = 40 };

            btnNovice.Click += (s, e) => { SelectedMode = GameMode.Novice; this.DialogResult = DialogResult.OK; };
            btnExperienced.Click += (s, e) => { SelectedMode = GameMode.Experienced; this.DialogResult = DialogResult.OK; };
            btnPro.Click += (s, e) => { SelectedMode = GameMode.Pro; this.DialogResult = DialogResult.OK; };
            btnNightmare.Click += (s, e) => {
                var result = MessageBox.Show("Ты уверен?", "КОШМАР!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    SelectedMode = GameMode.Nightmare;
                    this.DialogResult = DialogResult.OK;
                }
            };

            this.Controls.Add(btnNovice);
            this.Controls.Add(btnExperienced);
            this.Controls.Add(btnPro);
            this.Controls.Add(btnNightmare);
        }
    }
} 