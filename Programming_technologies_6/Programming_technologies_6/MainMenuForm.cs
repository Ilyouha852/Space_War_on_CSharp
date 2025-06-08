using System;
using System.Drawing;
using System.Windows.Forms;

namespace Programming_technologies_6
{
    public class MainMenuForm : Form
    {
        public Button btnStart;
        public Button btnExit;
        private PictureBox logoPictureBox;

        public MainMenuForm()
        {
            this.Text = "Добро пожаловать в игру Space War!";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Width = 300;
            this.Height = 250; // Увеличиваем высоту, чтобы поместился логотип

            // Создаем PictureBox для логотипа
            logoPictureBox = new PictureBox();
            try
            {
                // Загружаем изображение из файла logo.png.  Убедитесь, что файл logo.png находится в папке с исполняемым файлом (bin\Debug или bin\Release).
                logoPictureBox.Image = Image.FromFile("logo.png");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки логотипа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Если не удалось загрузить логотип, можно создать пустой PictureBox или использовать другое изображение.
            }
            logoPictureBox.SizeMode = PictureBoxSizeMode.StretchImage; // Растягиваем изображение по размеру PictureBox
            logoPictureBox.Width = 200;
            logoPictureBox.Height = 80;
            logoPictureBox.Top = 10; // Размещаем в верхней части формы
            logoPictureBox.Left = 40; // Выравниваем по центру с кнопками
            this.Controls.Add(logoPictureBox);

            btnStart = new Button();
            btnStart.Text = "Начать игру";
            btnStart.Width = 200;
            btnStart.Height = 40;
            btnStart.Top = 100; // Размещаем под логотипом
            btnStart.Left = 40;
            btnStart.DialogResult = DialogResult.OK;
            this.Controls.Add(btnStart);

            btnExit = new Button();
            btnExit.Text = "Выход из игры";
            btnExit.Width = 200;
            btnExit.Height = 40;
            btnExit.Top = 150; // Размещаем под кнопкой "Начать игру"
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