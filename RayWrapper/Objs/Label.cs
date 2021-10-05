using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static Raylib_cs.Raylib;
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

        public ColorModule backColor = new(50);
        public Action clicked;
        public ColorModule fontColor = new(192);
        public float fontSize = 24;
        public bool outline = false;
        public ColorModule outlineColor = new(Color.BLACK);
        public float spacing = 1.5f;
        public Actionable<string> text;
        public Actionable<TextMode> textMode;
        public Actionable<string> tooltip;
        public Actionable<bool> useBaseHover = false;

        private Rectangle _back;
        private string _textCache;
        private Vector2 _textSizeCache;

        public Label(Rectangle back, string text = "Untitled Label", TextMode textMode = TextMode.AlignLeft) =>
            (_back, this.text, this.textMode) = (back, text, textMode);

        public Rectangle Rect => AssembleRectFromVec(_back.Pos(), Size);

        public override Vector2 Position
        {
            get => _back.Pos();
            set => _back = _back.MoveTo(value);
        }

        public override Vector2 Size => textMode == TextMode.SizeToText ? _textSizeCache : _back.Size();

        public override void Update()
        {
            CheckText();
            if (!Rect.IsMouseIn() || !IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON)) return;
            clicked?.Invoke();
        }

        protected override void RenderCall()
        {
            var adjust = _back.Shrink(4);
            var hover = useBaseHover && Rect.IsMouseIn() && !IsMouseOccupied;
            Color realFc = hover ? ((Color)fontColor).MakeLighter() : fontColor;
            Color realBc = hover ? ((Color)backColor).MakeLighter() : backColor;
            CheckText();

            void DrawTxt(Vector2 pos) => DrawTextEx(GameBox.Font, _textCache, pos, fontSize, spacing, realFc);

            void DrawBack(Rectangle rect)
            {
                rect.DrawRounded(realBc, .25f);
                if (outline) rect.DrawRoundedLines(outlineColor, .25f);
            }

            if (textMode != TextMode.SizeToText) DrawBack(_back);

            switch ((TextMode)textMode)
            {
                case TextMode.AlignLeft:
                    DrawTxt(new Vector2(adjust.x, adjust.y + (adjust.height / 2 - _textSizeCache.Y / 2)));
                    break;
                case TextMode.AlignCenter:
                    GameBox.Font.DrawCenterText(adjust.Center(), _textCache, realFc, fontSize, spacing);
                    break;
                case TextMode.AlignRight:
                    DrawTxt(new Vector2(adjust.x + adjust.width - _textSizeCache.X,
                        adjust.y + (adjust.height / 2 - _textSizeCache.Y / 2)));
                    break;
                case TextMode.WrapText:
                    DrawTextRec(GameBox.Font, _textCache, adjust, fontSize, spacing, true, realFc);
                    break;
                case TextMode.SizeToText:
                    var rect = AssembleRectFromVec(adjust.Pos(), _textSizeCache).Grow(4);
                    DrawBack(rect);
                    DrawTxt(adjust.Pos());
                    break;
            }

            var t = (string)(tooltip ?? "");
            if (Rect.IsMouseIn() && t != "" && !IsMouseOccupied) Rect.DrawTooltip(t);
        }

        public void CheckText()
        {
            if (_textCache is not null && _textCache == text) return;
            _textSizeCache = MeasureTextEx(GameBox.Font, _textCache = text, fontSize, spacing);
        }
    }
}