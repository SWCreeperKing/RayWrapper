using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Objs;
using RayWrapper.Vars;

namespace RayWrapper.TreeViewShapes
{
    public class Circle : TreeViewShape
    {
        public Vector2 size;

        public Circle(string id, float radius = 1) : this(new Vector2(radius), id)
        {
        }
        
        public Circle(Vector2 size, string id) : base(id) =>
            this.size = size;

        public override Rectangle DrawShape(Vector2 pos, Vector2 change, float scale, bool isMarked, bool isVisible,
            Color getMarkColor, TreeViewControl tvc, TreeView tv)
        {
            var rect = RectWrapper.AssembleRectFromVec(pos, size * scale);

            if (!isVisible) return rect;
            rect.DrawCircle(getMarkColor);
            if (rect.IsMouseIn()) rect.DrawHallowCircle(isMarked ? markColor.unMarked : markColor.marked);

            return rect;
        }

        public override int GetDrawOrder() => 1;
    }
}