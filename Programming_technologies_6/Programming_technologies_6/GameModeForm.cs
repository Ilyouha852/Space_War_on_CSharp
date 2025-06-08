using System;
using System.Drawing;
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
        private TextBox descriptionTextBox;
        private Button playButton;

        public GameModeForm()
        {
            this.Text = "Выберите режим игры";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Width = 600;
            this.Height = 350;

            // Кнопки режимов
            var btnNovice = CreateModeButton("Новичок", GameMode.Novice, 20);
            var btnExperienced = CreateModeButton("Бывалый", GameMode.Experienced, 70);
            var btnPro = CreateModeButton("Про", GameMode.Pro, 120);
            var btnNightmare = CreateModeButton("КОШМАР!", GameMode.Nightmare, 170);

            // Текстовое поле для описания
            descriptionTextBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Width = 200,
                Height = 220,
                Top = 20,
                Left = 320,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Arial", 10),
                BackColor = SystemColors.Control,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Кнопка "Играть"
            playButton = new Button
            {
                Text = "Играть",
                Width = 250,
                Height = 40,
                Top = 230,
                Left = 40,
                Enabled = false
            };
            playButton.Click += PlayButton_Click;

            // Обработчики событий для кнопок
            btnNovice.Click += (s, e) => SelectMode(GameMode.Novice);
            btnExperienced.Click += (s, e) => SelectMode(GameMode.Experienced);
            btnPro.Click += (s, e) => SelectMode(GameMode.Pro);
            btnNightmare.Click += (s, e) => SelectMode(GameMode.Nightmare);

            this.Controls.Add(btnNovice);
            this.Controls.Add(btnExperienced);
            this.Controls.Add(btnPro);
            this.Controls.Add(btnNightmare);
            this.Controls.Add(descriptionTextBox);
            this.Controls.Add(playButton);
        }

        // Создание кнопки режима игры
        private Button CreateModeButton(string text, GameMode mode, int top)
        {
            var button = new Button
            {
                Text = text,
                Width = 250,
                Height = 40,
                Top = top,
                Left = 40
            };
            button.Click += (s, e) => SelectMode(mode);
            return button;
        }

        // Обновление описания режима и активация кнопки "Играть"
        private void SelectMode(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.Novice:
                    descriptionTextBox.Text = "Самый легкий, братан. Для тех, кто вообще не шарит. Ну ты понял, для самых зеленых.";
                    break;
                case GameMode.Experienced:
                    descriptionTextBox.Text = "Уже че-то знаешь, братан. Не совсем чайник, но и не профи. Так, середнячок.";
                    break;
                case GameMode.Pro:
                    descriptionTextBox.Text = "Ты в теме, братан! Все знаешь, все умеешь. Уже можешь советы раздавать.";
                    break;
                case GameMode.Nightmare:
                    descriptionTextBox.Text = "КОШМАР, братан! Это для тех, у кого вообще нет тормозов. Уровень твоего уважения будет зависеть от времени выживания!";
                    break;
                default:
                    descriptionTextBox.Text = "";
                    break;
            }

            SelectedMode = mode;
            playButton.Enabled = true;
        }

        // Обработчик для кнопки "Играть"
        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (SelectedMode.HasValue)
            {
                if (SelectedMode == GameMode.Nightmare)
                {
                    var result = MessageBox.Show("Ты уверен?", "КОШМАР!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if (result == DialogResult.No)
                    {
                        return; // Не продолжаем, если пользователь нажал "Нет"
                    }
                }
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите режим игры.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}