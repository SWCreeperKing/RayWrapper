using System;
using Raylib_CsLo;

namespace RayWrapper.Vars;

/// <summary>
/// an extension of <see cref="Actionable{T}"/> for <see cref="Raylib_CsLo.Color"/> 
/// </summary>
public class ColorModule
{
    public Actionable<Color> color = Raylib.BLACK;

    public ColorModule()
    {
        // json constructor
    }
        
    public ColorModule(Color color) => this.color = new Actionable<Color>(color);
    public ColorModule(Func<Color> color) => this.color = new Actionable<Color>(color);

    public ColorModule(int r = 0, int g = 0, int b = 0, int a = 255) =>
        color = new Actionable<Color>(new Color(r, g, b, a));

    public ColorModule(int rgb) : this(rgb, rgb, rgb)
    {
    }

    public void Deconstruct(out int r, out int g, out int b, out int a)
    {
        var c = (Color) this;
        (r, g, b, a) = (c.r, c.g, c.b, c.a);
    }

    public static implicit operator Color(ColorModule cm) => cm.color;
    public static implicit operator ColorModule(Color cm) => new(cm);
    public static implicit operator ColorModule(Func<Color> cm) => new(cm);

    public Color ReturnDarker() => ((Color) this).MakeDarker();
    public Color ReturnLighter() => ((Color) this).MakeLighter(); 
    
    public override string ToString()
    {
        var color = (Color) this;
        return $"({color.r},{color.g},{color.b},{color.a})";
    }

    public ColorModule Copy() => new() { color = color.Copy() };
}