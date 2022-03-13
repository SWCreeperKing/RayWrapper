﻿using System;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;
using static RayWrapper.GameBox;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs
{
    public class Label : GameObject
    {
        public static Style defaultStyle = new();
        public Rectangle Rect => _clickCheck ?? _back;

        public override Vector2 Position
        {
            get => Rect.Pos();
            set => _back.MoveTo(value);
        }

        public override Vector2 Size => Rect.Size();

        public Style style = defaultStyle.Copy();
        public Action clicked;
        public Actionable<string> text;
        public Actionable<string> tooltip;

        private Rectangle _back;
        private Rectangle? _clickCheck;

        // caching
        private bool _hoverCache;

        public Label(Vector2 pos, string text = "Untitled Label") : this(
            AssembleRectFromVec(pos, defaultStyle.textStyle.MeasureText(text)).Grow(4), text)
        {
        }

        public Label(Rectangle back, string text = "Untitled Label") => (_back, this.text) = (back, text);

        protected override void UpdateCall()
        {
            if (Rect.IsMouseIn() && IsMouseButtonPressed(MOUSE_LEFT_BUTTON)) clicked?.Invoke();
        }

        protected override void RenderCall()
        {
            style.Draw(text, _back, out _clickCheck);
            var t = (string) (tooltip ?? string.Empty);
            if (Rect.IsMouseIn() && t != string.Empty && !IsMouseOccupied) Rect.DrawTooltip(t);
        }

        public class Style : IStyle<Style>
        {
            public ColorModule fontColor = new(192);
            public ColorModule backColor = new(70);
            public Text.Style textStyle = new();
            public RectStyle backStyle = new();
            public OutlineStyle outline = new();
            public bool drawOutline = true;
            public Actionable<bool> drawHover = false;
            public Func<Color, bool, Color> drawColor;

            public DrawMode drawMode
            {
                get => _drawMode;
                set
                {
                    textStyle.drawMode = value switch
                    {
                        DrawMode.AlignLeft => Text.Style.DrawMode.Normal,
                        DrawMode.AlignCenter => Text.Style.DrawMode.Center,
                        DrawMode.AlignRight => Text.Style.DrawMode.Normal,
                        DrawMode.SizeToText => Text.Style.DrawMode.Normal,
                        DrawMode.WrapText => Text.Style.DrawMode.Wrap,
                        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
                    };
                    _drawMode = value;
                }
            }

            private DrawMode _drawMode = DrawMode.AlignLeft;
            private bool hasOgDrawColor;

            public enum DrawMode
            {
                AlignLeft,
                AlignCenter,
                AlignRight,
                SizeToText,
                WrapText
            }

            public Style(Func<Color, bool, Color> drawColor = null)
            {
                if (drawColor is not null)
                {
                    this.drawColor = drawColor;
                    return;
                }

                this.drawColor = (c, b) =>
                {
                    if (!drawHover) return c;
                    return b ? c.MakeLighter() : c;
                };
                hasOgDrawColor = true;
            }

            public void Draw(string text, Rectangle back, out Rectangle? check)
            {
                var textSize = textStyle.MeasureText(text);
                var shrink = back.Shrink(4);
                if (drawMode is DrawMode.SizeToText) back = AssembleRectFromVec(shrink.Pos(), textSize).Grow(4);
                backStyle.Draw(back);
                if (drawOutline) outline.Draw(back);

                var hover = back.IsMouseIn();
                check = back;
                backStyle.color = drawColor.Invoke(backColor, hover);
                textStyle.color = drawColor.Invoke(fontColor, hover);
                
                switch (drawMode)
                {
                    case DrawMode.AlignLeft:
                        textStyle.Draw(text,
                            new Vector2(shrink.x, shrink.y + (shrink.height / 2 - textSize.Y / 2)));
                        break;
                    case DrawMode.AlignCenter:
                        textStyle.Draw(text, shrink.Center());
                        break;
                    case DrawMode.AlignRight:
                        textStyle.Draw(text,
                            new Vector2(shrink.x + shrink.width - textSize.X,
                                shrink.y + (shrink.height / 2 - textSize.Y / 2)));
                        break;
                    case DrawMode.SizeToText:
                        textStyle.Draw(text, shrink.Pos());
                        break;
                    case DrawMode.WrapText:
                        textStyle.Draw(text, shrink);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public Style Copy()
            {
                return new Style(hasOgDrawColor ? null : drawColor)
                {
                    outline = outline.Copy(), backColor = backColor.Copy(), backStyle = backStyle.Copy(),
                    drawMode = drawMode, fontColor = fontColor.Copy(), textStyle = textStyle.Copy(),
                    drawOutline = drawOutline, drawHover = drawHover
                };
            }
        }
    }
}