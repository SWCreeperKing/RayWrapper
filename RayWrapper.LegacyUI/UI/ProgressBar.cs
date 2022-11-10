using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.Extras;
using RayWrapper.Base.GameObject;
using RayWrapper.Base.Primitives;
using static Raylib_CsLo.Raylib;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.LegacyUI.UI;

// todo: see if to merge progress bar and slider
public class ProgressBar : GameObject
{
    // todo progressbar style
    public ColorModule backColor = new(BLACK);
    public ColorModule fillColor = new(RAYWHITE);
    public ColorModule toColor = new(GOLD);
    public ColorModule finishedColor = new(GREEN);
    public bool hoverPercent = true;
    public bool isVertical;
    public int outlineThickness = 3;
    public Func<float> percent;
    public bool useGradient = true;
    public Tooltip tooltip;

    public ProgressBar(Rectangle rect, Func<float> percent)
    {
        (this.percent, pos, size) = (percent, rect.Pos, rect.Size);
        tooltip = new DefaultTooltip(new Actionable<string>(() =>
            FixedPercent() >= 1 ? "100%" : $"{percent():##0.00%}"));
    }

    public ProgressBar(float x, float y, float width, float height, Func<float> percent)
    {
        (this.percent, pos, size) = (percent, new Vector2(x, y), new Vector2(width, height));
        tooltip = new DefaultTooltip(new Actionable<string>(() =>
            FixedPercent() >= 1 ? "100%" : $"{percent():##0.00%}"));
    }

    protected override void RenderCall()
    {
        var fill = FixedPercent();
        var back = new Rectangle(Position, size).GrowThis(outlineThickness);
        if (!useGradient) back.DrawRounded(color: backColor);
        else back.Draw(backColor);
        
        if (fill >= 1) new Rectangle(Position, size).Draw(finishedColor);
        else
        {
            var newS = new Vector2(size.X * (isVertical ? 1 : fill), size.Y * (isVertical ? fill : 1));
            if (!useGradient)
                new Rectangle(Position, newS).DrawRounded(color: ((Color) fillColor).Percent(toColor, fill));
            else new Rectangle(Position, newS).DrawGradient(fillColor, toColor);
        }

        if (!hoverPercent) return;
        tooltip.Draw(back);
    }

    public float FixedPercent()
    {
        var fill = percent();
        var fix = fill.IsFixable() ? fill.Fix() : fill;
        return Math.Clamp(fix, 0, 1);
    }
}