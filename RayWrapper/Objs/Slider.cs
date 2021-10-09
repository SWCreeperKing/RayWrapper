using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static Raylib_cs.Raylib;
using static RayWrapper.GameBox;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs
{
    public class Slider : GameObject
    {
        public ColorModule backColor = new(Color.BLACK);
        public ColorModule fillColor = new(Color.RAYWHITE);
        public ColorModule hoverColor = new(150);
        public bool isVertical;
        public Action<float> onDone;
        public int outlineThickness = 3;
        public float value;

        private readonly Vector2 _size;
        private Vector2 _pos;

        public Slider(Rectangle rect) => (_pos, _size) = (rect.Pos(), rect.Size());

        public Slider(float x, float y, float width, float height) =>
            (_pos, _size) = (new Vector2(x, y), new Vector2(width, height));

        public override Vector2 Position
        {
            get => _pos;
            set => _pos = value;
        }

        public override Vector2 Size => _size + new Vector2(outlineThickness);

        public override void UpdateCall()
        {
            if (IsMouseOccupied && mouseOccupier != this) return;
            if (IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) && AssembleRectFromVec(Position, _size).IsMouseIn())
                mouseOccupier = this;
            else if (!IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) && mouseOccupier == this)
            {
                mouseOccupier = null;
                onDone?.Invoke(value);
            }

            if (mouseOccupier != this) return;
            var mouse = mousePos;
            value = Math.Clamp(mouse.X - Position.X, 0, _size.X) / _size.X;
        }

        protected override void RenderCall()
        {
            var newS = new Vector2(_size.X * (isVertical ? 1 : value), _size.Y * (isVertical ? value : 1));
            var rect = AssembleRectFromVec(Position, _size).Grow(outlineThickness);
            rect.DrawRounded(backColor);
            AssembleRectFromVec(Position, newS).DrawRounded(fillColor);
            if (IsMouseOccupied && mouseOccupier != this) return;
            if (rect.IsMouseIn() || mouseOccupier == this) rect.DrawRoundedLines(hoverColor);
        }
    }
}