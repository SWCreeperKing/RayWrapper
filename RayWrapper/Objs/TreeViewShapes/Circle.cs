using System;
using System.Numerics;
using Raylib_cs;

namespace RayWrapper.Objs.TreeViewShapes
{
    public class Circle: TreeViewShape
    {
        public Vector2 size;

        public Circle(Rectangle rect, Func<bool> isMarked, string toolTip, string id = "", Func<bool> isVisible = null) :
            base(rect.Pos(), isMarked, toolTip, id, isVisible) =>
            size = rect.Size();

        public override Rectangle DrawShape(Vector2 pos, Vector2 change, float scale, bool isMarked, bool isVisible)
        {
            var rect = RectWrapper.AssembleRectFromVec(pos, size * scale);

            if (!isVisible) return rect;
            rect.DrawCircle(GetColor());
            if (rect.IsMouseIn()) rect.DrawHallowCircle(isMarked ? markColor.unMarked : markColor.marked);

            return rect;
        }
    }
}