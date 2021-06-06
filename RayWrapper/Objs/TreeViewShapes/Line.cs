using System;
using System.Numerics;
using Raylib_cs;

namespace RayWrapper.Objs.TreeViewShapes
{
    public class Line : TreeViewShape
    {
        public Vector2 pos2;

        public Line(Vector2 pos1, Vector2 pos2, Func<bool> isMarked, Func<bool> isVisible = null) : base(pos1, isMarked,
            "", "", isVisible) =>
            this.pos2 = pos2;

        public override Rectangle DrawShape(Vector2 pos, Vector2 change, float scale, bool isMarked, bool isVisible)
        {
            var rect = RectWrapper.AssembleRectFromVec(pos, new Vector2());
            var halfUnit = .5f * scale;

            if (!isVisible) return rect;
            Raylib.DrawLineEx(pos + new Vector2(halfUnit), (change + pos2) * scale + new Vector2(halfUnit),
                halfUnit / 2, GetColor());

            return rect;
        }
    }
}