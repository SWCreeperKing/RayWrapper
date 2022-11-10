using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.GameBox;
using RayWrapper.LegacyUI.UI;
using RayWrapperTester.Example_Setup;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

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
        _tooltip = new DefaultTooltip("Testing Tooltip");
        _scissorArea.Draw(_maskColor);
        _maskText = new Text("Move the mouse around to reveal this text!", new Vector2(190, 200));
    }

    protected override void UpdateCall(float dt)
    {
        var mouse = Input.MousePosition.currentPosition;
        _scissorArea = new Rectangle(mouse.X - 100, mouse.Y - 100, 200, 200);
        _maskText.Update(dt);
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