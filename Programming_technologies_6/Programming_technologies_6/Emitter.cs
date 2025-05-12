using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Programming_technologies_6.Particle;

namespace Programming_technologies_6
{
    class Emitter
    {
        public int X; // координата X центра эмиттера, будем ее использовать вместо MousePositionX
        public int Y; // соответствующая координата Y 
        public int Direction = 0; // вектор направления в градусах куда сыпет эмиттер
        public int Spreading = 360; // разброс частиц относительно Direction
        public int SpeedMin = 1; // начальная минимальная скорость движения частицы
        public int SpeedMax = 10; // начальная максимальная скорость движения частицы
        public int RadiusMin = 2; // минимальный радиус частицы
        public int RadiusMax = 10; // максимальный радиус частицы
        public int LifeMin = 20; // минимальное время жизни частицы
        public int LifeMax = 100; // максимальное время жизни частицы
        public int ParticlesPerTick = 1;

        public Color ColorFrom = Color.White; // начальный цвет частицы
        public Color ColorTo = Color.FromArgb(0, Color.Black); // конечный цвет частиц
        public List<Particle> particles = new List<Particle>();
        public List<IImpactPoint> impactPoints = new List<IImpactPoint>();
        public int MousePositionX;
        public int MousePositionY;
        public float GravitationX = 0;
        public float GravitationY = 1; // пусть гравитация будет силой один пиксель за такт, нам хватит

        public virtual Particle CreateParticle()
        {
            var particle = new ParticleColorful();
            particle.FromColor = ColorFrom;
            particle.ToColor = ColorTo;

            return particle;
        }

        public virtual void UpdateState()
        {
            int particlesToCreate = ParticlesPerTick;

            foreach (var particle in particles)
            {
                particle.Life -= 1; // уменьшаю здоровье
                                    // если здоровье кончилось
                if (particle.Life == 0)
                {
                    if (particlesToCreate > 0)
                    {
                        /* у нас как сброс частицы равносилен созданию частицы */
                        particlesToCreate -= 1; // поэтому уменьшаем счётчик созданных частиц на 1
                        ResetParticle(particle);
                    }
                }
                else
                {
                    particle.X += particle.SpeedX;
                    particle.Y += particle.SpeedY;

                    particle.Life -= 1;
                    foreach (var point in impactPoints)
                    {
                        point.ImpactParticle(particle);
                    }

                    // это не трогаем
                    particle.SpeedX += GravitationX;
                    particle.SpeedY += GravitationY;

                }
            }
            for (var i = 0; i < 10; ++i)
            {
                if (particles.Count < ParticlesCount)
                {
                    var particle = CreateParticle(); // и собственно теперь тут его вызываем
                    ResetParticle(particle);
                    particles.Add(particle);
                }
                else
                {
                    break;
                }
            }
            while (particlesToCreate >= 1)
            {
                particlesToCreate -= 1;
                var particle = CreateParticle();
                ResetParticle(particle);
                particles.Add(particle);
            }

        }
        public int ParticlesCount = 500;

        // добавил новый метод, виртуальным, чтобы переопределять можно было
        public virtual void ResetParticle(Particle particle)
        {
            particle.Life = Particle.rand.Next(LifeMin, LifeMax);

            // Случайная позиция по X в пределах ширины экрана
            particle.X = Particle.rand.Next(0, 800); // 800 - ширина формы
            particle.Y = 0; // Всегда сверху

            // Движение сверху вниз
            particle.SpeedX = Particle.rand.Next(-2, 3); // Небольшой разброс по горизонтали
            particle.SpeedY = Particle.rand.Next(SpeedMin, SpeedMax); // Движение вниз

            particle.Radius = Particle.rand.Next(RadiusMin, RadiusMax);
        }

        public abstract class IImpactPoint
        {
            public float X; // ну точка же, вот и две координаты
            public float Y;

            // абстрактный метод с помощью которого будем изменять состояние частиц
            // например притягивать
            public abstract void ImpactParticle(Particle particle);

            // базовый класс для отрисовки точечки
            public virtual void Render(Graphics g)
            {
                g.FillEllipse(
                        new SolidBrush(Color.Red),
                        X - 5,
                        Y - 5,
                        10,
                        10
                    );
            }
        }
        public class GravityPoint : IImpactPoint
        {
            public int Power = 100; // сила притяжения

            // а сюда по сути скопировали с минимальными правками то что было в UpdateState

            public override void Render(Graphics g)
            {
                // буду рисовать окружность с диаметром равным Power
                g.DrawEllipse(
                       new Pen(Color.Red),
                       X - Power / 2,
                       Y - Power / 2,
                       Power,
                       Power
                   );

                var stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                // обязательно выносим текст и шрифт в переменные
                var text = $"Я гравитон\nc силой {Power}";
                var font = new Font("Verdana", 10);

                // вызываем MeasureString, чтобы померить размеры текста
                var size = g.MeasureString(text, font);

                // рисуем подложнку под текст
                g.FillRectangle(
                    new SolidBrush(Color.Red),
                    X - size.Width / 2, // так как я выравнивал текст по центру то подложка должна быть центрирована относительно X,Y
                    Y - size.Height / 2,
                    size.Width,
                    size.Height
                );

                g.DrawString(
                       text,
                       font,
                       new SolidBrush(Color.White),
                       X,
                       Y,
                       stringFormat
                );
            }

            public override void ImpactParticle(Particle particle)
            {
                float gX = X - particle.X;
                float gY = Y - particle.Y;

                double r = Math.Sqrt(gX * gX + gY * gY); // считаем расстояние от центра точки до центра частицы
                if (r + particle.Radius < Power / 2) // если частица оказалось внутри окружности
                {
                    // то притягиваем ее
                    float r2 = (float)Math.Max(100, gX * gX + gY * gY);
                    particle.SpeedX += gX * Power / r2;
                    particle.SpeedY += gY * Power / r2;
                }
            }
        }

        public class AntiGravityPoint : IImpactPoint
        {
            public int Power = 100; // сила отторжения

            // а сюда по сути скопировали с минимальными правками то что было в UpdateState
            public override void ImpactParticle(Particle particle)
            {
                float gX = X - particle.X;
                float gY = Y - particle.Y;
                float r2 = (float)Math.Max(100, gX * gX + gY * gY);

                particle.SpeedX -= gX * Power / r2; // тут минусики вместо плюсов
                particle.SpeedY -= gY * Power / r2; // и тут
            }
        }

        public class TopEmitter : Emitter
        {
            public int Width; // длина экрана

            public override void ResetParticle(Particle particle)
            {
                base.ResetParticle(particle); // вызываем базовый сброс частицы, там жизнь переопределяется и все такое

                // а теперь тут уже подкручиваем параметры движения
                particle.X = Particle.rand.Next(Width); // позиция X -- произвольная точка от 0 до Width
                particle.Y = 0;  // ноль -- это верх экрана 

                particle.SpeedY = 1; // падаем вниз по умолчанию
                particle.SpeedX = Particle.rand.Next(-2, 2); // разброс влево и вправа у частиц 
            }
        }

        public void Render(Graphics g)
        {
            foreach (var particle in particles)
            {
                particle.Draw(g);
            }
            foreach (var point in impactPoints)
            {
                point.Render(g);
            }
        }
        public class RadarPoint : IImpactPoint
        {
            public int Radius = 100; // радиус радара
            public int CountTotal = 0;
            public int CountSmall = 0;
            public int CountMedium = 0;
            public int CountLarge = 0;

            public override void ImpactParticle(Particle particle)
            {
                float dx = X - particle.X;
                float dy = Y - particle.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);

                if (distance < Radius)
                {
                    // Подсчет частиц
                    CountTotal++;
                    if (particle.Radius < 4) CountSmall++;
                    else if (particle.Radius < 7) CountMedium++;
                    else CountLarge++;

                    // Подсветка частицы
                    particle.FromColor = Color.Cyan;
                    particle.ToColor = Color.White;
                }
            }

            public override void Render(Graphics g)
            {
                // Сама область радара
                g.DrawEllipse(new Pen(Color.Cyan, 2), X - Radius, Y - Radius, Radius * 2, Radius * 2);

                var font = new Font("Consolas", 10);
                var format = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                string text = $"Всего: {CountTotal}";
                // Можно раскомментировать, если хочешь детализацию
                //text += $"\nМал.: {CountSmall}\nСр.: {CountMedium}\nБол.: {CountLarge}";

                g.DrawString(text, font, Brushes.Cyan, X, Y, format);
            }

            public void ResetCounter()
            {
                CountTotal = 0;
                CountSmall = 0;
                CountMedium = 0;
                CountLarge = 0;
            }

            public void ChangeRadius(int delta)
            {
                Radius = Math.Max(20, Radius + delta);
            }
        }

        public class ExplosionEmitter : Emitter
        {
            public override void UpdateState()
            {
                // Обновляем только существующие частицы, не создаём новые
                for (int i = particles.Count - 1; i >= 0; i--)
                {
                    var particle = particles[i];
                    particle.Life -= 1;
                    if (particle.Life <= 0)
                    {
                        particles.RemoveAt(i);
                        continue;
                    }
                    particle.X += particle.SpeedX;
                    particle.Y += particle.SpeedY;
                    foreach (var point in impactPoints)
                    {
                        point.ImpactParticle(particle);
                    }
                    particle.SpeedX += GravitationX;
                    particle.SpeedY += GravitationY;
                }
            }
        }
    }
}
