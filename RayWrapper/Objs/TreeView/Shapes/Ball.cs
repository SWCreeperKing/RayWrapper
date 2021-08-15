using System;
using System.Numerics;

namespace RayWrapper.Objs.TreeView.Shapes
{
    public class Ball : TreeNode
    {
        public Ball(string name, Vector2 pos, TreeNodeTrigger isComplete, float radius = 1,
            TreeNodeTrigger isVisible = null, Action<string> onClick = null, string tooltip = "") : base(name, pos,
            new Vector2(radius), isComplete, isVisible, onClick, tooltip)
        {
        }

        public Ball(string name, Vector2 pos, Vector2 size, TreeNodeTrigger isComplete,
            TreeNodeTrigger isVisible = null, Action<string> onClick = null, string tooltip = "") : base(name, pos,
            size, isComplete, isVisible, onClick, tooltip)
        {
        }

        public override void DrawShape(Vector2 change, float scale, bool isCheck) =>
            rect.MoveBy(change).Multi(new Vector2(scale)).DrawCircle(isCheck ? markColor.marked : markColor.unMarked);
    }
}