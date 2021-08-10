using System.Numerics;
using Raylib_cs;

namespace RayWrapper.Vars
{
    public abstract class GameObject : IRayObject
    {
        public Vector2 Position => initPosition + addedPosition;
        public bool isVisible = true;
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
            if (!isVisible) return;
            RenderCall();
        }

        public abstract void Update();
        protected abstract void RenderCall();
        public abstract void PositionChange(Vector2 v2);

        public void Text(string text, Vector2 pos, Color color, int fontSize = 24, float spacing = 1.5f) =>
            GameBox.font.DrawText(text, pos, color, fontSize, spacing);

        public void TextWrap(string text, Rectangle rect, Color fontColor) =>
            GameBox.font.DrawTextWrap(text, rect, fontColor);

        public void TextCenter(string text, Rectangle rect, Color fontColor) =>
            GameBox.font.DrawCenterText(rect.Center(), text, fontColor);
    }
}