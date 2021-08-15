using System;
using System.Collections.Generic;
using System.Numerics;

namespace RayWrapper.Objs.TreeView.Shapes
{
    public class Line : TreeNodeBase
    {
        public static readonly Vector2 nill = new(float.MaxValue, float.MinValue);

        public bool prog = true;
        public string id1;
        public string id2;
        public Vector2 id2f = nill;
        public TreeNodeTrigger id2fT;

        public Line(string from, string to, bool prog = true) => (id1, id2, this.prog) = (from, to, prog);
        public Line(string from, Vector2 to, TreeNodeTrigger trigger) => (id1, id2f, id2fT) = (from, to, trigger);

        public override string Draw(Dictionary<string, TreeNode> nodes, Vector2 change, float scale)
        {
            if (prog && !nodes[id1].isComplete.Invoke(nodes)) return "";
            Vector2 GetPos(string id) => (change + nodes[id].Pos().Add(.5f)) * scale;
            Vector2 GetPosV2(Vector2 v2) => (change + v2.Add(.5f)) * scale;
            (GetPos(id1), id2f == nill ? GetPos(id2) : GetPosV2(id2f)).DrawLine(
                (id2f == nill ? nodes[id2].isComplete : id2fT).Invoke(nodes) ? markColor.marked : markColor.unMarked,
                .5f * scale / 2);
            return "";
        }
    }
}