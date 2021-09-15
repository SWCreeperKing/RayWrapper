using System;
using Raylib_cs;

namespace RayWrapper.Vars
{
    public class ColorModule
    {
        public Actionable<Color> color;
        public ColorModule(Color color) => this.color = new Actionable<Color>(color);
        public ColorModule(Func<Color> color) => this.color = new Actionable<Color>(color);

        public ColorModule(float r = 0, float g = 0, float b = 0, float a = 255) =>
            color = new Actionable<Color>(new Color((int)r, (int)g, (int)b, (int)a));

        public ColorModule(float rgb) : this(rgb, rgb, rgb, 255)
        {
        }

        public static implicit operator Color(ColorModule cm) => cm.color;
        public static implicit operator ColorModule(Color cm) => new(cm);
        public static implicit operator ColorModule(Func<Color> cm) => new(cm);
    }
}