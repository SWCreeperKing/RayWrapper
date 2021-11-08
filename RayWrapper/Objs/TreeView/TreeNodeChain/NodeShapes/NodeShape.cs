using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;

namespace RayWrapper.Objs.TreeView.TreeNodeChain.NodeShapes
{
    public abstract class NodeShape
    {
        protected Vector2 position;
        protected Vector2 size;

        public ColorModule color = new(0);
        public ColorModule completeColor = new(255);
        public Actionable<bool> completed = new(false);
        public Action lClick;
        public Action rClick;
        public Func<string> tooltip;
        public bool isVisibleCompleted = false;

        protected NodeShape(Vector2 position, Vector2 size, Func<string> tooltip = null) =>
            (this.position, this.size, this.tooltip) = (position, size, tooltip);

        public virtual bool IsMouseIn(Vector2 off, float scale) =>
            GameBox.mousePos.IsVectInVects(off + position * scale, size * scale);

        public virtual void DrawOnHover(Vector2 off, float scale) => DrawShape(off, scale);
        public virtual Vector2 Center(Vector2 off, float scale) => position * scale + off + size / 2 * scale;
        public abstract void DrawShape(Vector2 off, float scale);

        public void Update(Vector2 off, float scale, bool context, bool next)
        {
            if (!IsMouseIn(off, scale)) return;
            var isLeft = Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON);
            var isRight = Raylib.IsMouseButtonPressed(MouseButton.MOUSE_RIGHT_BUTTON);
            if (isLeft && context && !completed) lClick?.Invoke();
            if (isRight && completed && !next) rClick?.Invoke();
        }

        public void Draw(Vector2 off, float scale)
        {
            if (isVisibleCompleted && !completed) return;
            DrawShape(off, scale);
            if (!IsMouseIn(off, scale)) return;
            DrawOnHover(off, scale);
            if (tooltip is not null) GameBox.tooltip.Add(tooltip.Invoke());
        }
    }
}