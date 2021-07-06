using System.Numerics;
using Raylib_cs;
using RayWrapper.Objs;
using RayWrapper.Vars;

namespace RayWrapper.TreeViewShapes
{
    public abstract class TreeViewShape
    {
        public string id;
        public string[] carry = System.Array.Empty<string>();
        public (Color unMarked, Color marked) markColor = (new(0, 0, 0, 255), new(255, 255, 255, 255));

        public TreeViewShape(string id) => this.id = id;

        public (string, Rectangle, string) Draw(Vector2 change, float scale, TreeViewControl tvc, TreeView tv)
        {
            var isMarked = tvc.GetMarked(id, carry);
            var isVisible = tvc.GetVisual(id, carry);
            var size = DrawShape((change + tvc.GetPos(id, carry)) * scale, change, scale, isMarked, isVisible,
                isMarked ? markColor.marked : markColor.unMarked, tvc, tv);
            if (!size.IsMouseIn() || !isVisible) return ("", size, "");
            return (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON) ? id : "", size, tvc.GetTooltip(id, carry));
        }

        public abstract Rectangle DrawShape(Vector2 pos, Vector2 change, float scale, bool isMarked, bool isVisible, Color getMarkColor,
            TreeViewControl tvc, TreeView tv);

        public abstract int GetDrawOrder();
    }
}