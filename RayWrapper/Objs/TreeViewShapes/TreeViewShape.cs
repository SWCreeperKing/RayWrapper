using System;
using System.Numerics;
using Raylib_cs;

namespace RayWrapper.Objs.TreeViewShapes
{
    public abstract class TreeViewShape
    {
        public string id;
        public Func<bool> isMarked;
        public Func<bool> isVisible;
        public (Color unMarked, Color marked) markColor = (new(0, 0, 0, 255), new(255, 255, 255, 255));
        public Vector2 pos;
        public string toolTip;

        public TreeViewShape(Vector2 pos, Func<bool> isMarked, string toolTip, string id = "",
            Func<bool> isVisible = null) =>
            (this.pos, this.isMarked, this.toolTip, this.id, this.isVisible) =
            (pos, isMarked, toolTip, id, isVisible ?? (() => true));

        public string Draw(Vector2 change, float scale)
        {
            var size = DrawShape((change + pos) * scale, change, scale, isMarked.Invoke(), isVisible.Invoke());
            if (!size.IsMouseIn()) return "";
            if (toolTip != "") size.DrawTooltip(toolTip);
            return Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON) ? id : "";
        }

        public Color GetColor() => isMarked.Invoke() ? markColor.marked : markColor.unMarked;
        public abstract Rectangle DrawShape(Vector2 pos, Vector2 change, float scale, bool isMarked, bool isVisible);
    }
}