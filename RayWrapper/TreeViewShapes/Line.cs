using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Objs;
using RayWrapper.Vars;

namespace RayWrapper.TreeViewShapes
{
    public class Line : TreeViewShape
    {
        public bool progression = true;
        public string to = "null";
        public bool usePos;
        public Func<bool> posMarked = null;

        private string[] toCarry = Array.Empty<string>();

        public Line(string fromId, string toId, bool usePos = false) : base(fromId) =>
            (to, this.usePos) = (toId, usePos);

        public void Init(TreeView tv)
        {
            var bNode = tv.GetNodeWithId(id);
            carry = bNode.carry;
            if (!tv.HasNode(to)) return;
            var toN = tv.GetNodeWithId(to);
            toCarry = toN.carry;
        }

        public override Rectangle DrawShape(Vector2 pos, Vector2 change, float scale, bool isMarked, bool isVisible,
            Color getMarkColor, TreeViewControl tvc, TreeView tv)
        {
            var rect = RectWrapper.AssembleRectFromVec(pos, new Vector2());
            var halfUnit = .5f * scale;
            var realVis = progression ? isMarked : isVisible;
            var realMark = progression
                ? usePos
                    ? posMarked?.Invoke() ?? true
                    : tvc.GetVisual(to, toCarry)
                : isMarked;

            if (!realVis) return rect;
            Raylib.DrawLineEx(pos + new Vector2(halfUnit),
                (change + tvc.GetPos(to, toCarry)) * scale + new Vector2(halfUnit),
                halfUnit / 2, realMark ? markColor.marked : markColor.unMarked);

            return rect;
        }

        public override int GetDrawOrder() => -1;
    }
}