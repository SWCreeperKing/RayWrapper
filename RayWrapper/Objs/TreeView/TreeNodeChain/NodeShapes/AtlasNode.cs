using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static RayWrapper.Objs.TextureAtlas;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs.TreeView.TreeNodeChain.NodeShapes;

public class AtlasNode : NodeShape
{
    public Vector2 size;
    public CompoundAtlasTexture image;
    public ColorModule outLineColor = Raylib.WHITE;

    public AtlasNode(CompoundAtlasTexture image, Vector2 pos, Vector2 size, Actionable<string> tooltip = null) :
        base(pos, size)
    {
        this.size = size;
        this.image = image;
        this.tooltip = tooltip;
    }

    public AtlasNode(CompoundAtlasTexture image, Vector2 pos, Actionable<string> tooltip = null) : base(pos,
        Vector2.One)
    {
        size = Vector2.One;
        this.image = image;
        this.tooltip = tooltip;
    }

    public override void DrawShape(Vector2 off, float scale)
    {
        var rect = AssembleRectFromVec(position * scale + off, size * scale);
        rect.Draw(completed ? completeColor : color);
        image.Draw(rect, scale);
    }

    public override void DrawOnHover(Vector2 off, float scale)
    {
        var rect = AssembleRectFromVec(position * scale + off, size * scale);
        rect.Grow((int) (scale / 8f * 1.5f)).DrawHallowRect(outLineColor);
    }
}