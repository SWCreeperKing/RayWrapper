﻿using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.GameBox;
using RayWrapper.Base.GameObject;
using RayWrapper.Base.Primitives;
using static Raylib_CsLo.Raylib;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.LegacyUI.UI;

public class Label : GameObject
{
    public static Style defaultStyle = new();

    public Rectangle Rect =>
        style.drawMode is Style.DrawMode.SizeToText
            ? new Rectangle(_back.Pos, style.textStyle.MeasureText(text)).Grow(4)
            : _back;

    public Style style = defaultStyle.Copy();
    public Action clicked;
    public Actionable<string> text;
    public Tooltip tooltip;

    private Rectangle _back;
    private Rectangle _backWithText;

    public Label(Vector2 pos, string text = "Untitled Label") : this(
        new Rectangle(pos, defaultStyle.textStyle.MeasureText(text)).Grow(4), text)
    {
    }

    public Label(Rectangle back, string text = "Untitled Label") => (_back, this.text) = (back, text);

    protected override void UpdateCall(float dt)
    {
        if (clicked is not null) ClickCheck(clicked);
    }

    protected override void RenderCall()
    {
        style.Draw(text, Rect);
        if (tooltip is not null && !Input.IsMouseOccupied) tooltip.Draw(Rect);
    }

    protected override Vector2 GetPosition() => Rect.Pos;
    protected override Vector2 GetSize() => Rect.Size;
    protected override void UpdatePosition(Vector2 newPos) => _back.Pos = newPos;
    protected override void UpdateSize(Vector2 newSize) => _back.Size = newSize;

    public void ClickCheck(Action onClick)
    {
        if (Rect.IsMouseIn() && IsMouseButtonPressed(MOUSE_LEFT_BUTTON)) onClick?.Invoke();
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
                    DrawMode.AlignLeft or DrawMode.AlignRight or DrawMode.SizeToText => Text.Style.DrawMode.Normal,
                    DrawMode.AlignCenter => Text.Style.DrawMode.Center,
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

        public void Draw(string text, Rectangle back)
        {
            var textSize = textStyle.MeasureText(text);
            var shrink = back.Shrink(4);
            var hover = back.IsMouseIn();
            textStyle.color = drawColor(fontColor, hover);
            backStyle.color = drawColor(backColor, hover);

            backStyle.Draw(back);
            if (drawOutline) outline.Draw(back);

            switch (drawMode)
            {
                case DrawMode.AlignLeft:
                    textStyle.Draw(text,
                        new Vector2(shrink.X, shrink.Y + (shrink.H / 2 - textSize.Y / 2)));
                    break;
                case DrawMode.AlignCenter:
                    textStyle.Draw(text, shrink);
                    break;
                case DrawMode.AlignRight:
                    textStyle.Draw(text,
                        new Vector2(shrink.X + shrink.W - textSize.X,
                            shrink.Y + (shrink.H / 2 - textSize.Y / 2)));
                    break;
                case DrawMode.SizeToText:
                    textStyle.Draw(text, shrink.Pos);
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