using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs.TreeView.TreeNodeChain.NodeShapes;

public class ImageNode : NodeShape
{
    public ImageObj image;

    public ImageNode(ImageObj img, Vector2 position, Vector2 size, Actionable<string> tooltip = null) : base(position,
        size)
    {
        image = img;
        this.tooltip = tooltip;
    }

    public ImageNode(ImageObj img, Vector2 position, Actionable<string> tooltip = null) : base(position, Vector2.One)
    {
        image = img;
        this.tooltip = tooltip;
    }

    public override void DrawShape(Vector2 off, float scale)
    {
        var rect = AssembleRectFromVec(position * scale + off, size * scale);
        image.RenderTo(rect);
    }

    public override void DrawOnHover(Vector2 off, float scale)
    {
        var rect = AssembleRectFromVec(position * scale + off, size * scale);
        rect.Grow((int) (scale / 8f) * 2).DrawHallowRect(completed ? color : completeColor);
        rect.Grow((int) (scale / 8f)).DrawHallowRect(completed ? completeColor : color);
        image.RenderTo(rect);
    }
}