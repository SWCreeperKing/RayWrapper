using System;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;

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
        public float spacing;

        private int _fontSize;
        private Font _font;
        private Vector2 _pos;
        private string _cachedText;

        public Text(Actionable<string> text, Vector2 pos, ColorModule color = null, int fontSize = 24,
            float spacing = 1.5f)
        {
            (this.text, _pos, this.color, _font, this.spacing, _cachedText, _fontSize) = (text, pos,
                color ?? SKYBLUE, FontManager.GetDefFont(fontSize), spacing, text, fontSize);
            rect = RectWrapper.AssembleRectFromVec(pos, MeasureText());
        }

        public Text(Actionable<string> text, Rectangle rect, ColorModule color = null, int fontSize = 24,
            float spacing = 1.5f)
        {
            (this.rect, this.text, _pos, this.color, _font, this.spacing, _cachedText, _fontSize) = (rect, text,
                rect.Pos(), color ?? SKYBLUE, FontManager.GetDefFont(fontSize), spacing, text, fontSize);
        }

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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetFont(string fontName, int size) => _font = FontManager.GetFont(fontName, _fontSize = size);
        public void SetSize(Vector2 newSize) => rect = rect.SetSize(newSize);
        public Vector2 MeasureText() => _font.MeasureText(_cachedText, _fontSize, spacing);
        public void Draw() => _font.DrawText(_cachedText, _pos, color, _fontSize, spacing);
        public void DrawWrap() => _font.DrawTextRec(_cachedText, rect, color, _fontSize, spacing);
        public void DrawCenter() => _font.DrawCenterText(rect.Center(), _cachedText, color, _fontSize, spacing);
    }
}