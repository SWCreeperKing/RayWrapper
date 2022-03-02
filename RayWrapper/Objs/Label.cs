using System;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;
using static RayWrapper.FontManager;
using static RayWrapper.GameBox;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs
{
    public class Label : GameObject
    {
        public enum TextMode
        {
            AlignLeft,
            AlignCenter,
            AlignRight,
            SizeToText,
            WrapText
        }

        public Rectangle Rect => _rect;

        public override Vector2 Position
        {
            get => _back.Pos();
            set
            {
                _back.MoveTo(value);
                ReCalc();
            }
        }

        public override Vector2 Size =>
            textMode == TextMode.SizeToText ? _textSizeCache + new Vector2(8) : _back.Size();

        public ColorModule backColor = new(50);
        public ColorModule fontColor = new(192);
        public ColorModule outlineColor = new(BLACK);
        public Action clicked;
        public int fontSize = 24;
        public bool outline = false;
        public float spacing = 1.5f;
        public Actionable<string> text;
        public Actionable<TextMode> textMode;
        public Actionable<string> tooltip;
        public Actionable<bool> useBaseHover = false;

        private Rectangle _back;
        private string _textCache;
        private Vector2 _textSizeCache;

        // textmode cache
        private Rectangle _rect;
        private Rectangle _sizedRect;
        private Rectangle _adj;
        private Vector2 _sizedPos;
        private Vector2 _alignLeft;
        private Vector2 _alignCenter;
        private Vector2 _alignRight;

        public Label(Rectangle back, string text = "Untitled Label", TextMode textMode = TextMode.AlignLeft)
        {
            (_back, this.text, this.textMode) = (back, text, textMode);
        }

        public Label(Vector2 pos, string text = "Untitled Label", TextMode textMode = TextMode.AlignLeft)
        {
            (this.text, this.textMode) = (text, textMode);
            CheckText();
            _back = AssembleRectFromVec(pos, _textSizeCache).Grow(4);
        }

        protected override void UpdateCall()
        {
            if (!Rect.IsMouseIn() || !IsMouseButtonPressed(MOUSE_LEFT_BUTTON)) return;
            clicked?.Invoke();
        }

        protected override void RenderCall()
        {
            var hover = useBaseHover && Rect.IsMouseIn() && !IsMouseOccupied;
            Color realFc = hover ? ((Color) fontColor).MakeLighter() : fontColor;
            Color realBc = hover ? ((Color) backColor).MakeLighter() : backColor;
            CheckText();

            void DrawTxt(Vector2 pos) => DrawTextEx(GetDefFont(fontSize), _textCache, pos, fontSize, spacing, realFc);

            void DrawBack(Rectangle rect)
            {
                rect.DrawRounded(realBc, .25f);
                if (outline) rect.DrawRoundedLines(outlineColor, .25f);
            }

            if (textMode != TextMode.SizeToText) DrawBack(_back);

            switch ((TextMode) textMode)
            {
                case TextMode.AlignLeft:
                    DrawTxt(_alignLeft);
                    break;
                case TextMode.AlignCenter:
                    GetDefFont(fontSize).DrawCenterText(_alignCenter, _textCache, realFc, fontSize, spacing);
                    break;
                case TextMode.AlignRight:
                    DrawTxt(_alignRight);
                    break;
                case TextMode.WrapText:
                    GetDefFont(fontSize).DrawTextRec(_textCache, _adj, realFc, fontSize, spacing, true);
                    break;
                case TextMode.SizeToText:
                    DrawBack(_sizedRect);
                    DrawTxt(_sizedPos);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var t = (string) (tooltip ?? string.Empty);
            if (Rect.IsMouseIn() && t != string.Empty && !IsMouseOccupied) Rect.DrawTooltip(t);
        }

        public void CheckText()
        {
            if (_textCache is not null && _textCache == text) return;
            _textSizeCache = MeasureTextEx(GetDefFont(fontSize), _textCache = text, fontSize, spacing);
            ReCalc();
        }

        public void ReCalc()
        {
            _adj = _back.Shrink(4);
            _sizedRect = AssembleRectFromVec(_adj.Pos(), _textSizeCache).Grow(4);
            _sizedPos = _adj.Pos();
            _alignLeft = new Vector2(_adj.x, _adj.y + (_adj.height / 2 - _textSizeCache.Y / 2));
            _alignCenter = _adj.Center();
            _alignRight = new Vector2(_adj.x + _adj.width - _textSizeCache.X,
                _adj.y + (_adj.height / 2 - _textSizeCache.Y / 2));
            _rect = AssembleRectFromVec(_back.Pos(), Size);
        }
    }
}