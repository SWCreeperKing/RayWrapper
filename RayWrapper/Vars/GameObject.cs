using System;
using System.Numerics;
using Raylib_cs;

namespace RayWrapper.Vars
{
    public abstract class GameObject : IRayObject
    {
        public Vector2 Position => initPosition + addedPosition;
        public Func<bool> isVisible = () => true;
        public Vector2 SizeFrom0 => Position + Size();
        protected Vector2 initPosition;
        protected Vector2 addedPosition;

        public GameObject(Vector2 pos) => initPosition = pos;

        public void MoveBy(Vector2 v2)
        {
            addedPosition += v2;
            PositionChange(Position);
        }

        public void MoveTo(Vector2 v2)
        {
            addedPosition = v2 - initPosition;
            PositionChange(Position);
        }

        public void Render()
        {
            if (!isVisible.Invoke()) return;
            RenderCall();
        }

        public abstract void Update();
        protected abstract void RenderCall();
        public abstract void PositionChange(Vector2 v2);
        public abstract Vector2 Size();

        public void Text(string text, Vector2 pos, Color color, int fontSize = 24, float spacing = 1.5f) =>
            GameBox.Font.DrawText(text, pos, color, fontSize, spacing);

        public void TextWrap(string text, Rectangle rect, Color fontColor) =>
            GameBox.Font.DrawTextWrap(text, rect, fontColor);

        public void TextCenter(string text, Rectangle rect, Color fontColor) =>
            GameBox.Font.DrawCenterText(rect.Center(), text, fontColor);

        public Vector2 MeasureText(string text, float fontSize = 24f, float spacing = 1.5f) =>
            GameBox.Font.MeasureText(text, fontSize, spacing);
    }
}