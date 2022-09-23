using Raylib_CsLo;

namespace RayWrapper.Base;

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

public static class ColorAddon
{
    /// <summary>
    /// makes a <see cref="Color"/> slightly lighter
    /// </summary>
    /// <param name="color"><see cref="Color"/> to make lighter</param>
    /// <returns>the lighter version of <paramref name="color"/></returns>
    public static Color MakeLighter(this Color color)
    {
        return new Color((int) Math.Min(color.r * 1.5, 255), (int) Math.Min(color.g * 1.5, 255),
            (int) Math.Min(color.b * 1.5, 255),
            color.a);
    }

    /// <summary>
    /// makes a <see cref="Color"/> slightly darker
    /// </summary>
    /// <param name="color"><see cref="Color"/> to make darker</param>
    /// <returns>the darker version of <paramref name="color"/></returns>
    public static Color MakeDarker(this Color color)
    {
        return new Color((int) (color.r / 1.7), (int) (color.g / 1.7), (int) (color.b / 1.7), color.a);
    }
}