using System;
using System.Windows.Forms;

namespace Programming_technologies_6
{
    public class MainMenuForm : Form
    {
        public Button btnStart;
        public Button btnExit;

        public MainMenuForm()
        {
            this.Text = "Добро пожаловать в игру Space War!";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Width = 300;
            this.Height = 200;

            btnStart = new Button();
            btnStart.Text = "Начать игру";
            btnStart.Width = 200;
            btnStart.Height = 40;
            btnStart.Top = 30;
            btnStart.Left = 40;
            btnStart.DialogResult = DialogResult.OK;
            this.Controls.Add(btnStart);

            btnExit = new Button();
            btnExit.Text = "Выход из игры";
            btnExit.Width = 200;
            btnExit.Height = 40;
            btnExit.Top = 90;
            btnExit.Left = 40;
            btnExit.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnExit);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainMenuForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Name = "MainMenuForm";
            this.ResumeLayout(false);

        }
    }
} 