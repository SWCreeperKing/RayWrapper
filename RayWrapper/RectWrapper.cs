using RayWrapper.Base;
using RayWrapper.Var_Interfaces;

namespace RayWrapper;

public class RectStyle : IStyle<RectStyle>
{
    public ColorModule color = new(70);
    public bool rounded;
    public float roundness = .5f;
    public int segments = 10;

    public void Draw(Rectangle rect)
    {
        rect.color = color;
        if (rounded) rect.DrawRounded(segments, roundness);
        else rect.Draw();
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
        rect.color = color;
        if (rounded) rect.DrawRoundedLines(segments, roundness, thickness);
        else rect.DrawLines();
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