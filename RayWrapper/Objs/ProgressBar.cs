using System;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs;

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
        (this.percent, pos, size) = (percent, rect.Pos(), rect.Size());
        tooltip = new GameBox.DefaultTooltip(new Actionable<string>(() =>
            FixedPercent() >= 1 ? "100%" : $"{percent.Invoke():##0.00%}"));
    }

    public ProgressBar(float x, float y, float width, float height, Func<float> percent)
    {
        (this.percent, pos, size) = (percent, new Vector2(x, y), new Vector2(width, height));
        tooltip = new GameBox.DefaultTooltip(new Actionable<string>(() =>
            FixedPercent() >= 1 ? "100%" : $"{percent.Invoke():##0.00%}"));
    }

    protected override void RenderCall()
    {
        var fill = FixedPercent();
        var back = AssembleRectFromVec(Position, size).Grow(outlineThickness);
        if (!useGradient) back.DrawRounded(backColor);
        else back.Draw(backColor);
        if (fill >= 1) AssembleRectFromVec(Position, size).Draw(finishedColor);
        else
        {
            var newS = new Vector2(size.X * (isVertical ? 1 : fill), size.Y * (isVertical ? fill : 1));
            if (!useGradient)
                AssembleRectFromVec(Position, newS).DrawRounded(((Color) fillColor).Percent(toColor, fill));
            else AssembleRectFromVec(Position, newS).DrawGradient(fillColor, toColor);
        }

        if (!hoverPercent) return;
        tooltip.Draw(back);
    }

    public float FixedPercent()
    {
        var fill = percent.Invoke();
        var fix = fill.IsFixable() ? fill.Fix() : fill;
        return Math.Clamp(fix, 0, 1);
    }
}