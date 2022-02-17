using System;
using System.Numerics;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs.TreeView.TreeNodeChain.NodeShapes
{
    public class Ball : NodeShape
    {
        public Ball(Vector2 position, float radius, Func<string> tooltip = null) :
            this(position, new Vector2(radius)) =>
            this.tooltip = tooltip;

        public Ball(Vector2 position, Vector2 size, Func<string> tooltip = null) : base(position, size) =>
            this.tooltip = tooltip;

        public Ball(Vector2 position, Func<string> tooltip = null) : base(position, Vector2.One) =>
            this.tooltip = tooltip;

        public override void DrawShape(Vector2 off, float scale) =>
            AssembleRectFromVec(position * scale + off, size * scale).DrawCircle(completed ? completeColor : color);

        public override void DrawOnHover(Vector2 off, float scale) =>
            AssembleRectFromVec(position * scale + off, size * scale)
                .DrawHallowCircle(completed ? color : completeColor, (int) (scale / 8f));
    }
}