using System.Numerics;
using Raylib_cs;

namespace RayWrapper.Vars
{
    public abstract class GameObject
    {
        public Actionable<bool> isVisible = true;
        public abstract Vector2 Position { get; set; }
        public abstract Vector2 Size { get; }

        public float FullLength => Position.X + Size.X;
        public float FullHeight => Position.Y + Size.Y;
        
        public void Render()
        {
            if (!isVisible) return;
            RenderCall();
        }

        public abstract void Update();
        protected abstract void RenderCall();

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