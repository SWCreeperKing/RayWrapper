using System.Numerics;
using Raylib_CsLo;
using RayWrapper;
using RayWrapper.Objs;
using RayWrapper.Vars;
using RayWrapperTester.Example_Setup;
using Rectangle = RayWrapper.Base.Rectangle;

namespace RayWrapperTester.Examples;

[Example("Toolip & Masking")]
public class TooltipMask : Example
{
    private Tooltip _tooltip;
    private Text _maskText;
    private Color _maskColor = new(255, 255, 255, 10);
    private Rectangle _scissorArea= new(100,  100, 200, 200);

    public TooltipMask(string tabName) : base(tabName)
    {
        _tooltip = new GameBox.DefaultTooltip("Testing Tooltip");
        _scissorArea.Draw(_maskColor);
        _maskText = new Text("Move the mouse around to reveal this text!", new Vector2(190, 200));
    }

    protected override void UpdateCall()
    {
        var mouse = GameBox.mousePos;
        _scissorArea = new Rectangle(mouse.X - 100, mouse.Y - 100, 200, 200);
        _maskText.Update();
    }

    protected override void RenderCall()
    {
        _scissorArea.MaskDraw(() =>
        {
            _scissorArea.Draw(_maskColor);
            _maskText.Render();
        });
        _tooltip.Draw();
    }
}