using System.Numerics;
using RayWrapper.Base;
using Rectangle = RayWrapper.Base.Rectangle;

namespace RayWrapper.Objs.TreeView.TreeNodeChain.NodeShapes;

public class Ball : NodeShape
{
    public Ball(Vector2 position, float radius, Actionable<string> tooltip = null) :
        this(position, new Vector2(radius)) =>
        this.tooltip = tooltip;

    public Ball(Vector2 position, Vector2 size, Actionable<string> tooltip = null) : base(position, size) =>
        this.tooltip = tooltip;

    public Ball(Vector2 position, Actionable<string> tooltip = null) : base(position, Vector2.One) =>
        this.tooltip = tooltip;

    public override void DrawShape(Vector2 off, float scale) =>
        new Rectangle(position * scale + off, size * scale).DrawRounded(completed ? completeColor : color);

    public override void DrawOnHover(Vector2 off, float scale) =>
        new Rectangle(position * scale + off, size * scale)
            .DrawRoundedLines(completed ? color : completeColor, (int) (scale / 8f));
}