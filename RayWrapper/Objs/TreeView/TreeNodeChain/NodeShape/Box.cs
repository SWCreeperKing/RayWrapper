using System;
using System.Numerics;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs.TreeView.TreeNodeChain.NodeShape
{
    public class Box : NodeShape
    {
        public Box(Vector2 position, Vector2 size, Func<string> tooltip = null) : base(position, size) =>
            this.tooltip = tooltip;
        
        public Box(Vector2 position, Func<string> tooltip = null) : base(position, Vector2.One) =>
            this.tooltip = tooltip;

        public override void DrawShape(Vector2 off, float scale) =>
            AssembleRectFromVec(position * scale + off, size * scale).Draw(completed ? completeColor : color);

        public override void DrawOnHover(Vector2 off, float scale) =>
            AssembleRectFromVec(position * scale + off, size * scale).DrawHallowRect(completed ? color : completeColor, (int)(scale/8f));
    }
}