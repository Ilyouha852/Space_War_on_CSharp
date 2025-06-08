using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Programming_technologies_6.Emitter;
using static Programming_technologies_6.Particle;
using Programming_technologies_6;

namespace Programming_technologies_6
{
    public partial class Form1 : Form
    {
        List<Emitter> emitters = new List<Emitter>();
        Emitter emitter;
        Emitter playerEmitter;
        ExplosionEmitter explosionEmitter;
        private bool isExplosionActive = false;
        private int explosionTimerTicks = 0;
        private bool isPlayerVisible = true; // Добавляем флаг видимости игрока

        private float playerX;
        private float playerY;
        private const float playerSpeed = 10f;

        private const float playerSize = 20f * 3;
        private const float enemySize = 15f * 3;
        private const float bulletSize = 5f * 4;
        private const float bombSize = 7f * 4;

        private int score = 0;
        private int health = 3;
        private bool isGameOver = false;
        private System.Windows.Forms.Timer enemySpawnTimer;
        private System.Windows.Forms.Timer countdownTimer;
        private int countdown = 5;
        private bool isCountdownActive = true;
        private bool isGameStarted = false;
        private DateTime gameStartTime;
        private GameMode currentMode = GameMode.Novice;
        private int enemyParticlesPerTick = 1;
        private int enemySpeedY = 10;
        private int enemySpawnInterval = 1000;
        private bool moveLeft = false;
        private bool moveRight = false;
        private Image backgroundImage;
        private int backgroundOffsetY = 0;
        private int backgroundScrollSpeed = 2;

        private Image playerImage;
        private Image enemyImage;
        private Image bulletImage;
        private Image bombImage;

        public Form1(GameMode mode) : this()
        {
            currentMode = mode;
            switch (mode)
            {
                case GameMode.Novice:
                    enemySpawnInterval = 1000;
                    enemySpeedY = 10;
                    break;
                case GameMode.Experienced:
                    enemySpawnInterval = 350;
                    enemySpeedY = 20;
                    break;
                case GameMode.Pro:
                    enemySpawnInterval = 150;
                    enemySpeedY = 25;
                    break;
                case GameMode.Nightmare:
                    enemySpawnInterval = 50;
                    enemySpeedY = 35;
                    break;
            }
            if (enemySpawnTimer != null)
                enemySpawnTimer.Interval = enemySpawnInterval;
        }

        public Form1()
        {
            InitializeComponent();
            picDisplay.Image = new Bitmap(picDisplay.Width, picDisplay.Height);

            playerX = picDisplay.Width / 2;
            playerY = picDisplay.Height - 50;

            this.emitter = new Emitter
            {
                SpeedMin = 2,
                SpeedMax = 5,
                ColorFrom = Color.Red,
                ColorTo = Color.FromArgb(0, Color.Red),
                ParticlesPerTick = 0,
                X = 0,
                Y = 0,
            };

            this.playerEmitter = new Emitter
            {
                SpeedMin = 10,
                SpeedMax = 10,
                ColorFrom = Color.White,
                ColorTo = Color.FromArgb(0, Color.White),
                ParticlesPerTick = 0,
                X = (int)playerX,
                Y = (int)playerY,
            };

            this.explosionEmitter = new ExplosionEmitter
            {
                SpeedMin = 5,
                SpeedMax = 15,
                ColorFrom = Color.Yellow,
                ColorTo = Color.Orange,
                ParticlesPerTick = 0,
                X = (int)playerX,
                Y = (int)playerY,
                RadiusMin = 2,
                RadiusMax = 5,
                LifeMin = 50,
                LifeMax = 100,
            };

            emitters.Add(this.emitter);
            emitters.Add(this.playerEmitter);

            this.emitter.ParticlesPerTick = 0;
            this.emitter.ParticlesCount = 0;
            this.playerEmitter.ParticlesPerTick = 0;
            this.playerEmitter.ParticlesCount = 0;

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
            this.MouseDown += Form1_MouseDown;
            this.picDisplay.MouseDown += Form1_MouseDown;

            try
            {
                playerImage = Image.FromFile("mainship.png");
                // Масштабируем изображение игрока
                playerImage = ResizeImage(playerImage, (int)playerSize, (int)playerSize);

                enemyImage = Image.FromFile("enemyship.png");
                // Масштабируем изображение врага
                enemyImage = ResizeImage(enemyImage, (int)enemySize, (int)enemySize);

                bulletImage = Image.FromFile("bullet.png");
                // Масштабируем изображение пули
                bulletImage = ResizeImage(bulletImage, (int)bulletSize, (int)bulletSize);

                bombImage = Image.FromFile("rocket_bomb.png");
                // Масштабируем изображение бомбы
                bombImage = ResizeImage(bombImage, (int)bombSize, (int)bombSize);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображений: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Если не удалось загрузить изображения, можно использовать дефолтное
            }

            enemySpawnTimer = new System.Windows.Forms.Timer();
            enemySpawnTimer.Interval = enemySpawnInterval;
            enemySpawnTimer.Tick += (s, e) => {
                if (!isGameOver && !isCountdownActive)
                {
                    if (isGameStarted && (DateTime.Now - gameStartTime).TotalSeconds >= 2)
                    {
                        var particle = new ImageParticle();
                        particle.Pic = enemyImage;
                        particle.X = Particle.rand.Next(0, picDisplay.Width);
                        particle.Y = 0;
                        particle.SpeedX = Particle.rand.Next(-2, 3);
                        particle.SpeedY = enemySpeedY;
                        particle.Life = 100;
                        particle.Radius = (int)(enemySize / 2); // Радиус как у врага
                        emitter.particles.Add(particle);
                    }
                }
            };
            enemySpawnTimer.Start();

            countdownTimer = new System.Windows.Forms.Timer();
            countdownTimer.Interval = 1000;
            countdownTimer.Tick += (s, e) => {
                countdown--;
                if (countdown <= 0)
                {
                    countdownTimer.Stop();
                    isCountdownActive = false;
                    isGameStarted = true;
                    gameStartTime = DateTime.Now;
                    timer1.Start();
                    emitter.particles.Clear();
                    playerEmitter.particles.Clear();
                }
            };
            countdownTimer.Start();

            backgroundImage = Image.FromFile("background.jpg");
        }

        // Метод для масштабирования изображения
        private Image ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isGameOver)
            {
                if (e.KeyCode == Keys.A)
                    moveLeft = true;
                if (e.KeyCode == Keys.D)
                    moveRight = true;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
                moveLeft = false;
            if (e.KeyCode == Keys.D)
                moveRight = false;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isGameOver && e.Button == MouseButtons.Left && !isCountdownActive)
            {
                var particle = new ImageParticle();
                particle.Pic = bulletImage;
                particle.X = playerX;
                particle.Y = playerY;
                particle.SpeedX = 0;
                particle.SpeedY = -30;
                particle.Life = 100;
                particle.Radius = (int)(bulletSize / 2); //  bulletSize
                playerEmitter.particles.Add(particle);
            }
        }

        private void StartExplosion()
        {
            isExplosionActive = true;
            explosionTimerTicks = 0;
            explosionEmitter.particles.Clear();
            explosionEmitter.X = (int)playerX;
            explosionEmitter.Y = (int)playerY;
            for (int i = 0; i < 50; i++)
            {
                var particle = new ParticleColorful();
                particle.FromColor = Color.Yellow;
                particle.ToColor = Color.Orange;
                particle.X = playerX;
                particle.Y = playerY;

                var direction = (double)Particle.rand.Next(360);
                var speed = 10 + Particle.rand.Next(20);
                particle.SpeedX = (float)(Math.Cos(direction / 180 * Math.PI) * speed);
                particle.SpeedY = (float)(Math.Sin(direction / 180 * Math.PI) * speed);

                particle.Life = 50 + Particle.rand.Next(50);
                particle.Radius = 2 + Particle.rand.Next(3);
                explosionEmitter.particles.Add(particle);
            }
            isPlayerVisible = false; //  скрываем игрока
        }

        private void CheckGameOver()
        {
            if (health <= 0)
            {
                isGameOver = true;
                enemySpawnTimer.Stop();
                timer1.Stop();
                StartExplosion(); // Запускаем взрыв
                timer1.Start(); // Перезапускаем таймер для отрисовки взрыва
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Анимация фона
            if (backgroundImage != null)
            {
                backgroundOffsetY += backgroundScrollSpeed;
                if (backgroundOffsetY >= backgroundImage.Height)
                    backgroundOffsetY = 0;
            }

            if (isCountdownActive)
            {
                using (var g = Graphics.FromImage(picDisplay.Image))
                {
                    g.Clear(Color.Black);
                    // Прокручиваем фон
                    if (backgroundImage != null)
                    {
                        int h = backgroundImage.Height;
                        g.DrawImage(backgroundImage, 0, backgroundOffsetY, picDisplay.Width, h);
                        g.DrawImage(backgroundImage, 0, backgroundOffsetY - h, picDisplay.Width, h);
                    }
                    //  Отрисовка игрока ИЗОБРАЖЕНИЕМ
                    g.DrawImage(playerImage, playerX - playerSize / 2, playerY - playerSize / 2, playerSize, playerSize);
                    var font = new Font("Arial", 16);
                    g.DrawString($"Счет: {score}", font, Brushes.White, picDisplay.Width - 150, 10);
                    g.DrawString($"Здоровье: {health}", font, Brushes.White, picDisplay.Width - 150, 40);
                    var countdownFont = new Font("Arial", 48);
                    var countdownText = countdown.ToString();
                    var textSize = g.MeasureString(countdownText, countdownFont);
                    g.DrawString(countdownText, countdownFont, Brushes.White,
                        (picDisplay.Width - textSize.Width) / 2,
                        (picDisplay.Height - textSize.Height) / 2);
                }
                picDisplay.Invalidate();
                return;
            }

            if (isExplosionActive)
            {
                explosionEmitter.UpdateState();
                using (var g = Graphics.FromImage(picDisplay.Image))
                {
                    g.Clear(Color.Black);
                    // Прокручиваем фон
                    if (backgroundImage != null)
                    {
                        int h = backgroundImage.Height;
                        g.DrawImage(backgroundImage, 0, backgroundOffsetY, picDisplay.Width, h);
                        g.DrawImage(backgroundImage, 0, backgroundOffsetY - h, picDisplay.Width, h);
                    }
                    explosionEmitter.Render(g);
                    var font = new Font("Arial", 16);
                    g.DrawString($"Счет: {score}", font, Brushes.White, picDisplay.Width - 150, 10);
                    g.DrawString($"Здоровье: {health}", font, Brushes.White, picDisplay.Width - 150, 40);
                }
                picDisplay.Invalidate();
                explosionTimerTicks++;
                if (explosionTimerTicks >= 75)
                {
                    isExplosionActive = false;
                    explosionEmitter.particles.Clear();

                    var resultForm = new Form
                    {
                        Text = "Game Over",
                        Size = new Size(300, 150),
                        FormBorderStyle = FormBorderStyle.FixedDialog,
                        StartPosition = FormStartPosition.CenterScreen,
                        MaximizeBox = false,
                        MinimizeBox = false
                    };

                    var label = new Label
                    {
                        Text = $"Игра окончена!\nВаш счет: {score}",
                        AutoSize = true,
                        Location = new Point(20, 20)
                    };

                    var btnRestart = new Button
                    {
                        Text = "Начать заново",
                        DialogResult = DialogResult.Yes,
                        Location = new Point(20, 60),
                        Width = 80
                    };

                    var btnMainMenu = new Button
                    {
                        Text = "Главная",
                        DialogResult = DialogResult.Retry,
                        Location = new Point(110, 60),
                        Width = 80
                    };

                    var btnExit = new Button
                    {
                        Text = "Выход",
                        DialogResult = DialogResult.No,
                        Location = new Point(200, 60),
                        Width = 80
                    };

                    resultForm.Controls.AddRange(new Control[] { label, btnRestart, btnMainMenu, btnExit });
                    var result = resultForm.ShowDialog();

                    if (result == DialogResult.Yes)
                    {
                        score = 0;
                        health = 3;
                        isGameOver = false;
                        emitter.particles.Clear();
                        playerEmitter.particles.Clear();
                        explosionEmitter.particles.Clear();
                        enemySpawnTimer.Start();
                        gameStartTime = DateTime.Now;

                        playerX = picDisplay.Width / 2;
                        playerY = picDisplay.Height - 50;
                        countdown = 5;
                        isCountdownActive = true;
                        isGameStarted = false;
                        countdownTimer.Start();
                        isPlayerVisible = true; //  возвращаем видимость
                    }
                    else if (result == DialogResult.Retry)
                    {
                        this.Hide();
                        var menu = new MainMenuForm();
                        if (menu.ShowDialog() == DialogResult.OK)
                        {
                            var modeForm = new GameModeForm();
                            if (modeForm.ShowDialog() == DialogResult.OK && modeForm.SelectedMode.HasValue)
                            {
                                var newGame = new Form1(modeForm.SelectedMode.Value);
                                newGame.Show();
                            }
                        }
                        this.Close();
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
                return;
            }

            if (!isGameOver)
            {
                if (moveLeft)
                    playerX = Math.Max(playerSize, playerX - playerSpeed);
                if (moveRight)
                    playerX = Math.Min(picDisplay.Width - playerSize, playerX + playerSpeed);

                emitter.UpdateState();
                playerEmitter.UpdateState();

                for (int i = playerEmitter.particles.Count - 1; i >= 0; i--)
                {
                    if (i >= playerEmitter.particles.Count) continue;
                    var bullet = playerEmitter.particles[i];

                    // Проверяем, достигла ли пуля верхней границы экрана
                    if (bullet.Y < 0)  // Предполагаем, что верхний край экрана имеет координату Y = 0
                    {
                        playerEmitter.particles.RemoveAt(i);
                        continue; // Переходим к следующей пуле, чтобы избежать ошибок
                    }

                    for (int j = emitter.particles.Count - 1; j >= 0; j--)
                    {
                        if (j >= emitter.particles.Count) continue;
                        var enemy = emitter.particles[j];
                        float dx = bullet.X - enemy.X;
                        float dy = bullet.Y - enemy.Y;
                        float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                        if (distance < (bullet.Radius + enemy.Radius))
                        {
                            if (i < playerEmitter.particles.Count)
                                playerEmitter.particles.RemoveAt(i);
                            if (j < emitter.particles.Count)
                                emitter.particles.RemoveAt(j);
                            score++;
                            break;
                        }
                    }
                }

                if (!isCountdownActive)
                {
                    for (int i = emitter.particles.Count - 1; i >= 0; i--)
                    {
                        if (i >= emitter.particles.Count) continue;
                        var enemy = emitter.particles[i];
                        float dx = playerX - enemy.X;
                        float dy = playerY - enemy.Y;
                        float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                        // Хитбокс
                        if (distance < (playerSize / 2 + enemy.Radius))
                        {
                            if (i < emitter.particles.Count)
                                emitter.particles.RemoveAt(i);
                            health--;
                            CheckGameOver();
                        }
                    }
                }

                using (var g = Graphics.FromImage(picDisplay.Image))
                {
                    g.Clear(Color.Black);
                    // Прокручиваем фон
                    if (backgroundImage != null)
                    {
                        int h = backgroundImage.Height;
                        g.DrawImage(backgroundImage, 0, backgroundOffsetY, picDisplay.Width, h);
                        g.DrawImage(backgroundImage, 0, backgroundOffsetY - h, picDisplay.Width, h);
                    }

                    // Отрисовка игрока
                    if (isPlayerVisible)
                        g.DrawImage(playerImage, playerX - playerSize / 2, playerY - playerSize / 2, playerSize, playerSize);

                    if (!isCountdownActive)
                    {
                        foreach (var particle in emitter.particles)
                        {
                            if (particle is ImageParticle imageParticle)
                            {
                                g.DrawImage(imageParticle.Pic, imageParticle.X - imageParticle.Radius, imageParticle.Y - imageParticle.Radius, imageParticle.Radius * 2, imageParticle.Radius * 2);
                            }
                            else
                            {
                                particle.Draw(g);
                            }

                        }


                        foreach (var particle in playerEmitter.particles)
                        {
                            if (particle is ImageParticle imageParticle)
                            {
                                g.DrawImage(imageParticle.Pic, imageParticle.X - imageParticle.Radius, imageParticle.Y - imageParticle.Radius, imageParticle.Radius * 2, imageParticle.Radius * 2);
                            }
                            else
                            {
                                particle.Draw(g);
                            }

                        }
                    }
                    var font = new Font("Arial", 16);
                    g.DrawString($"Счет: {score}", font, Brushes.White, picDisplay.Width - 150, 10);
                    g.DrawString($"Здоровье: {health}", font, Brushes.White, picDisplay.Width - 150, 40);
                    if (isCountdownActive)
                    {
                        var countdownFont = new Font("Arial", 48);
                        var countdownText = countdown.ToString();
                        var textSize = g.MeasureString(countdownText, countdownFont);
                        g.DrawString(countdownText, countdownFont, Brushes.White,
                            (picDisplay.Width - textSize.Width) / 2,
                            (picDisplay.Height - textSize.Height) / 2);
                    }
                }

                picDisplay.Invalidate();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
