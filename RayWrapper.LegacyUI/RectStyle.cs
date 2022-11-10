using RayWrapper.Base.Primitives;

namespace RayWrapper.LegacyUI;

public static class RectUtil
{
    public static readonly Raylib_CsLo.Rectangle Zero = new(0, 0, 0, 0);
    public static readonly Raylib_CsLo.Rectangle Max = new(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue);
}

public class RectStyle : IStyle<RectStyle>
{
    public ColorModule color = new(70);
    public bool rounded;
    public float roundness = .5f;
    public int segments = 10;

    public void Draw(Rectangle rect)
    {
        if (rounded) rect.DrawRounded(color, segments, roundness);
        else rect.Draw(color);
    }

    public RectStyle Copy()
    {
        return new RectStyle
        {
            color = color.Copy(), rounded = rounded, roundness = roundness, segments = segments
        };
    }
}

public class OutlineStyle : IStyle<OutlineStyle>
{
    public ColorModule color = new(0);
    public bool rounded;
    public float thickness = .25f;
    public float roundness = .5f;
    public int segments = 10;
    public bool displayOutline = true;

    public void Draw(Rectangle rect)
    {
        if (!displayOutline) return;
        if (rounded) rect.DrawRoundedLines(color, segments, roundness, thickness);
        else rect.DrawLines(color: color);
    }

    // todo: use source gen for copy??????
    public OutlineStyle Copy()
    {
        return new OutlineStyle
        {
            color = color.Copy(), rounded = rounded, roundness = roundness, segments = segments,
            thickness = thickness, displayOutline = displayOutline
        };
    }
}