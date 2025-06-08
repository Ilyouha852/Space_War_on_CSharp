// Класс для частиц с картинкой
using Programming_technologies_6;
using System.Drawing;

public class ImageParticle : Particle
{
    public Image Pic;

    public override void Draw(Graphics g)
    {
        if (Pic != null)
        {
            g.DrawImage(Pic, X - Radius, Y - Radius, Radius * 2, Radius * 2); //  Рисуем картинку вместо эллипса
        }
        else
        {
            base.Draw(g); // Если картинка не задана, рисуем обычную частицу
        }
    }
}