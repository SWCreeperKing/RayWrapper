using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs.TreeView
{
    public abstract class TreeNode : TreeNodeBase
    {
        public string name;
        public Rectangle rect;
        public TreeNodeTrigger isComplete;
        public TreeNodeTrigger isVisible;
        public Action<string> onClick;
        public string tooltip;
        public bool noCompleteClick = true;

        private bool _isMouse;

        public TreeNode(string name, Vector2 pos, Vector2 size, TreeNodeTrigger isComplete,
            TreeNodeTrigger isVisible = null, Action<string> onClick = null, string tooltip = "") =>
            (this.name, rect, this.isComplete, this.isVisible, this.onClick, this.tooltip) = (name,
                AssembleRectFromVec(pos, size), isComplete, isVisible, onClick, tooltip);

        public override string Draw(Dictionary<string, TreeNode> nodes, Vector2 change, float scale)
        {
            if (isVisible is not null && !isVisible.Invoke(nodes)) return "";
            var calRect = rect.MoveBy(change).Multi(new Vector2(scale));
            var mouse = calRect.IsMouseIn();
            var isC = isComplete.Invoke(nodes);
            DrawShape(change, scale, isC);
            if (noCompleteClick && isC) return mouse ? tooltip : "";
            if (mouse && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON)) onClick?.Invoke(name);
            return mouse ? tooltip : "";
        }

        public Vector2 Pos() => rect.Pos();
        
        public abstract void DrawShape(Vector2 change, float scale, bool isCheck);
    }
}