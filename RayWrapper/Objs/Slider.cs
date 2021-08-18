using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static Raylib_cs.Raylib;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs
{
    public class Slider : GameObject
    {
        public static bool isInUse;
        public bool isUsing;
        public bool isVertical;
        public Vector2 size;
        public float value;
        public int outlineThickness = 3;
        public Color backColor = Color.BLACK;
        public Color fillColor = Color.RAYWHITE;
        public Action<float> onDone;

        public Slider(Rectangle rect) : base(rect.Pos()) => size = rect.Size();

        public override void Update()
        {
            if (isInUse && !isUsing) return;
            if (IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) && AssembleRectFromVec(Position, size).IsMouseIn())
                isInUse = isUsing = true;
            else if (!IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) && isUsing)
            {
                isInUse = isUsing = false;
                onDone?.Invoke(value);
            }
            if (!isInUse) return;
            var mouse = GameBox.MousePos;
            value = Math.Clamp(mouse.X - Position.X, 0, size.X)/size.X;
        }

        protected override void RenderCall()
        {
            var newS = new Vector2(size.X * (isVertical ? 1 : value), size.Y * (isVertical ? value : 1));
            AssembleRectFromVec(Position, size).Grow(outlineThickness).Draw(backColor);
            AssembleRectFromVec(Position, newS).Draw(fillColor);
        }

        public override void PositionChange(Vector2 v2)
        {
        }
    }
}