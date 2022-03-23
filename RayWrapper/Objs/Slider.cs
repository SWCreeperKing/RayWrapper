using System;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Var_Interfaces;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;
using static RayWrapper.GameBox;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs
{
    public class Slider : GameObject
    {
        public static Style defaultStyle = new();

        public Style style = defaultStyle.Copy();
        public bool isVertical;
        public Action<float> onDone;
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

        public override Vector2 Size => _size + new Vector2(style.outlineStyle.thickness);

        protected override void UpdateCall()
        {
            if (IsMouseOccupied && mouseOccupier != this) return;
            if (IsMouseButtonDown(MOUSE_LEFT_BUTTON) && AssembleRectFromVec(Position, _size).IsMouseIn())
            {
                mouseOccupier = this;
            }
            else if (!IsMouseButtonDown(MOUSE_LEFT_BUTTON) && mouseOccupier == this)
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
            var back = AssembleRectFromVec(Position, _size);
            var rect = AssembleRectFromVec(Position, newS);
            style.Draw(back, rect, back.IsMouseIn() || mouseOccupier == this);
        }

        public class Style : IStyle<Style>
        {
            public RectStyle backStyle = new() { color = BLACK };
            public RectStyle fillStyle = new() { color = RAYWHITE };
            public OutlineStyle outlineStyle = new() { color = new ColorModule(150) };

            public void Draw(Rectangle back, Rectangle rect, bool hover)
            {
                backStyle.Draw(back);
                fillStyle.Draw(rect);
                if (hover) outlineStyle.Draw(back.Grow(5));
            }

            public Style Copy()
            {
                return new Style
                {
                    backStyle = backStyle.Copy(), fillStyle = fillStyle.Copy(), outlineStyle = outlineStyle.Copy()
                };
            }
        }
    }
}