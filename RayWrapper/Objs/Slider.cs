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
        public ColorModule backColor = new(Color.BLACK);
        public ColorModule fillColor = new(Color.RAYWHITE);
        public ColorModule hoverColor = new(150);
        public Action<float> onDone;

        public Slider(Rectangle rect) : base(rect.Pos()) => size = rect.Size();

        public Slider(float x, float y, float width, float height) : base(new Vector2(x, y)) =>
            size = new Vector2(width, height);

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
            value = Math.Clamp(mouse.X - Position.X, 0, size.X) / size.X;
        }

        protected override void RenderCall()
        {
            var newS = new Vector2(size.X * (isVertical ? 1 : value), size.Y * (isVertical ? value : 1));
            var rect = AssembleRectFromVec(Position, size).Grow(outlineThickness);
            rect.DrawRounded(backColor);
            AssembleRectFromVec(Position, newS).DrawRounded(fillColor);
            if (isInUse && !isUsing) return;
            if (rect.IsMouseIn() || isUsing) rect.DrawRoundedLines(hoverColor);
        }

        public override void PositionChange(Vector2 v2)
        {
        }

        public override Vector2 Size() => AssembleRectFromVec(Position, size).Grow(outlineThickness).Size();
    }
}