using System.Numerics;
using Raylib_CsLo;

namespace RayWrapper.Vars;

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
    public void Draw() => GameBox.tooltips.Add(this);

    public void Draw(Rectangle bounds) => Draw(bounds, GameBox.mousePos);

    public void Draw(Rectangle bounds, Vector2 position)
    {
        if (bounds.IsV2In(position)) GameBox.tooltips.Add(this);
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