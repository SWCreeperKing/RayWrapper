using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.Extras;
using RayWrapper.Base.GameBox;
using RayWrapper.Base.Primitives;
using static RayWrapper.Base.GameBox.AttributeManager;
using static RayWrapper.Base.GameBox.GameBox;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.LegacyUI.UI;

public abstract class Tooltip
{
    public enum ScreenQuadrant
    {
        TopLeft = 1,
        TopRight = 2,
        BottomLeft = 3,
        BottomRight = 4
    }

    private Actionable<string> _data;

    public Tooltip(Actionable<string> data) => _data = data;

    /// <summary>
    /// Use this to draw the tooltip
    /// </summary>
    public void Draw() => TooltipController.tooltips.Add(this);

    public void Draw(Rectangle bounds) => Draw(bounds, Input.MousePosition.currentPosition);

    public void Draw(Rectangle bounds, Vector2 position)
    {
        if (bounds.IsV2In(position)) TooltipController.tooltips.Add(this);
    }

    /// <summary>
    /// Do not call this to draw, use <see cref="Tooltip.Draw()"/>
    /// This is for the process of rendering the tooltip
    /// </summary>
    public void RenderTooltip(ScreenQuadrant screenQuad)
    {
        if (_data is null) return;
        RenderTooltip(screenQuad, _data);
    }

    /// <summary>
    /// Do not call this to draw, use <see cref="Tooltip.Draw()"/>
    /// This is for the process of rendering the tooltip
    /// </summary>
    protected abstract void RenderTooltip(ScreenQuadrant screenQuad, string data);
}

public class DefaultTooltip : Tooltip
{
    public static ColorModule baseTooltipBackColor = new Color(0, 0, 0, 200);
    public static ColorModule baseTooltipColor = new Color(170, 170, 255, 220);

    public DefaultTooltip(Actionable<string> data) : base(data)
    {
    }

    protected override void RenderTooltip(ScreenQuadrant screenQuad, string data)
    {
        var mousePos = Input.MousePosition.currentPosition;
        var text = string.Join("\n", data);
        var defFont = Text.defaultStyle.Font;
        var textSize = defFont.MeasureText(text);
        Vector2 pos = new(mousePos.X - ((int) screenQuad % 2 != 0 ? textSize.X : 0),
            mousePos.Y - ((int) screenQuad > 2 ? textSize.Y : -33));
        var rect = new Rectangle(pos, textSize).GrowThis(4);
        rect.Draw(baseTooltipBackColor);
        rect.DrawLines(color: baseTooltipColor.ReturnDarker());
        defFont.DrawText(text, pos, baseTooltipColor);
    }
}

public static class TooltipController
{
    public static int tooltipLayers = 1;
    public static List<Tooltip> tooltips = new();
    public static ColorModule baseTooltipBackColor = new Color(0, 0, 0, 200);
    public static ColorModule baseTooltipColor = new Color(170, 170, 255, 220);

    [GameBoxWedge(PlacerType.AfterRender, 99999999)]
    public static void RenderUpdate()
    {
        if (!tooltips.Any()) return;
        var mousePos = Input.MousePosition.currentPosition;
        var quad = mousePos.X > WindowSize.X / 2 ? 1 : 2;
        if (mousePos.Y > WindowSize.Y / 2) quad += 2;
        
        foreach (var tt in tooltips.GetRange(tooltips.Count - tooltipLayers, tooltipLayers))
        {
            tt.RenderTooltip((Tooltip.ScreenQuadrant) quad);
        }

        tooltips.Clear();
    }
}