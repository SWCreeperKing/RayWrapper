using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    public class Text : GameObject
    {
        public enum TextMode
        {
            Normal,
            Wrap,
            Center
        }

        public override Vector2 Position
        {
            get => rect.Pos();
            set
            {
                rect.MoveTo(value);
                _pos = value;
            }
        }

        public override Vector2 Size => rect.Size();

        public TextMode mode = TextMode.Normal;
        public Actionable<string> text;
        public Rectangle rect;
        public ColorModule color;
        public float fontSize;
        public float spacing;

        private Vector2 _pos;
        private string _cachedText;

        public Text(Actionable<string> text, Vector2 pos, ColorModule color = null, float fontSize = 24,
            float spacing = 1.5f)
        {
            (this.text, _pos, this.color, this.fontSize, this.spacing, _cachedText) =
                (text, pos, color ?? Color.SKYBLUE, fontSize, spacing, text);
            rect = RectWrapper.AssembleRectFromVec(pos, MeasureText());
        }

        public Text(Actionable<string> text, Rectangle rect, ColorModule color = null, float fontSize = 24,
            float spacing = 1.5f) =>
            (this.rect, this.text, _pos, this.color, this.fontSize, this.spacing, _cachedText) = (rect, text,
                rect.Pos(), color ?? Color.SKYBLUE, fontSize, spacing, text);

        protected override void UpdateCall()
        {
            if (!isVisible) return;
            _cachedText = text;
        }

        protected override void RenderCall()
        {
            switch (mode)
            {
                case TextMode.Normal:
                    Draw();
                    break;
                case TextMode.Wrap:
                    DrawWrap();
                    break;
                case TextMode.Center:
                    DrawCenter();
                    break;
            }
        }

        public void SetSize(Vector2 newSize) => rect = rect.SetSize(newSize);
        public Vector2 MeasureText() => GameBox.Font.MeasureText(_cachedText, fontSize, spacing);
        public void Draw() => GameBox.Font.DrawText(_cachedText, _pos, color, (int)fontSize, spacing);
        public void DrawWrap() => GameBox.Font.DrawTextWrap(_cachedText, rect, color, (int)fontSize, spacing);

        public void DrawCenter() =>
            GameBox.Font.DrawCenterText(rect.Center(), _cachedText, color, (int)fontSize, spacing);
    }
}