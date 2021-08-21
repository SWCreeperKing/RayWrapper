using System;
using System.Numerics;

namespace RayWrapper.Objs.TreeView.Shapes
{
    public class Box : TreeNode
    {
        public Box(string name, Vector2 pos, Vector2 size, TreeNodeTrigger isComplete, TreeNodeTrigger isVisible = null,
            Action<string> onClick = null, Func<string> tooltip = null) : base(name, pos, size, isComplete, isVisible, onClick,
            tooltip)
        {
        }

        public Box(string name, Vector2 pos, TreeNodeTrigger isComplete, TreeNodeTrigger isVisible = null,
            Action<string> onClick = null, Func<string> tooltip = null) : base(name, pos, new Vector2(1), isComplete, isVisible,
            onClick,
            tooltip)
        {
        }

        public Box(string name, TreeNodeTrigger isComplete, TreeNodeTrigger isVisible = null,
            Action<string> onClick = null, Func<string> tooltip = null) : base(name, new Vector2(1), new Vector2(1), isComplete,
            isVisible, onClick,
            tooltip)
        {
        }

        public override void DrawShape(Vector2 change, float scale, bool isCheck) =>
            rect.MoveBy(change).Multi(new Vector2(scale)).Draw(isCheck ? markColor.marked : markColor.unMarked);
    }
}