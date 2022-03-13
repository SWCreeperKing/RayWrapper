﻿using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;

namespace RayWrapper.Objs
{
    public class Text : GameObject
    {
        public static Style defaultStyle = new();

        public override Vector2 Position
        {
            get => rect.Pos();
            set => rect.MoveTo(value);
        }

        public override Vector2 Size => rect.Size();

        public Style style = defaultStyle.Copy();
        public Actionable<string> text;
        public Rectangle rect;

        public Text(Actionable<string> text, Vector2 pos, ColorModule color = null, int fontSize = 24)
        {
            (this.text, style.fontSize) = (text, fontSize);
            if (color is not null) style.color = color;
            rect = RectWrapper.AssembleRectFromVec(pos, MeasureText());
        }

        public Text(Actionable<string> text, Rectangle rect, ColorModule color = null, int fontSize = 24)
        {
            (this.rect, this.text, style.fontSize) = (rect, text, fontSize);
            if (color is not null) style.color = color;
        }

        [Obsolete("Not Used")]
        protected override void UpdateCall()
        {
        }

        protected override void RenderCall() => style.Draw(text, rect);
        public Vector2 MeasureText() => style.MeasureText(text);

        public class Style : IStyle<Style>
        {
            public static Font? DefaultFont { get; private set; }

            public Font Font
            {
                get => _font ?? (DefaultFont ?? GetFontDefault());
                set => SetFont(value);
            }

            public TextureFilter Filter { get; private set; } = TextureFilter.TEXTURE_FILTER_BILINEAR;

            public ColorModule color = SKYBLUE;
            public float fontSize = 24;
            public float spacing = 1.5f;
            public bool useWordWrap = true;
            public DrawMode drawMode = DrawMode.Normal;

            private Font? _font;

            public enum DrawMode
            {
                Normal,
                Wrap,
                Center
            }

            public void Draw(string text, Rectangle rect)
            {
                switch (drawMode)
                {
                    case DrawMode.Normal:
                        Font.DrawText(text, rect.Pos(), color, fontSize, spacing);
                        break;
                    case DrawMode.Wrap:
                        Font.DrawTextRec(text, rect, color, fontSize, spacing, useWordWrap);
                        break;
                    case DrawMode.Center:
                        Font.DrawCenterText(rect.Center(), text, color, fontSize, spacing);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public void Draw(string text, Vector2 pos)
            {
                switch (drawMode)
                {
                    case DrawMode.Wrap:
                    case DrawMode.Normal:
                        Font.DrawText(text, pos, color, fontSize, spacing);
                        break;
                    case DrawMode.Center:
                        Font.DrawCenterText(pos, text, color, fontSize, spacing);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public static void SetDefaultFont(string fileName) => SetDefaultFont(LoadFont(fileName));
            public static void SetDefaultFont(Font font) => DefaultFont = font;

            public void SetFont(string fileName) => SetFont(LoadFont(fileName));

            public void SetFont(Font font)
            {
                _font = font;
                SetTextureFilter(Font.texture, Filter);
            }

            public void SetFilter(TextureFilter filter)
            {
                Filter = filter;
                SetTextureFilter(Font.texture, Filter);
            }

            public Vector2 MeasureText(string text) => Font.MeasureText(text, fontSize, spacing);

            public Style Copy()
            {
                var clone = new Style
                {
                    color = color.Copy(), spacing = spacing, drawMode = drawMode, fontSize = fontSize
                };
                clone.SetFont(Font);
                clone.SetFilter(Filter);
                return clone;
            }
        }
    }
}