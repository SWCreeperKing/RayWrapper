using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static Raylib_cs.MouseButton;
using static Raylib_cs.Raylib;

namespace RayWrapper.Objs
{
    public class Button : GameObject
    {
        public enum ButtonMode
        {
            SizeToText,
            CenterText,
            WrapText
        }

        public Rectangle adjustedRect;
        public Rectangle rect;
        public Color baseColor = new(56, 73, 99, 255);
        public Color hoverColor = new(99, 129, 175, 255);
        public Color disabledColor = new(13, 17, 23, 255);
        public Color fontColor = new(174, 177, 181, 255);
        public ButtonMode buttonMode;
        public string text;
        public bool isDisabled;

        private List<Action> _clickEvent = new();

        public event Action Clicked
        {
            add => _clickEvent.Add(value);
            remove => _clickEvent.Remove(value);
        }

        public Button(Rectangle rect, string text = "Untitled Button", ButtonMode buttonMode = ButtonMode.CenterText)
            : base(rect.Pos())
        {
            ChangeColor(new Color(56, 73, 99, 255), new Color(174, 177, 181, 255));
            (this.rect, this.text, this.buttonMode) = (rect, text, buttonMode);
        }

        public override void Update()
        {
            adjustedRect = buttonMode == ButtonMode.SizeToText
                ? rect.AdjustWh(GameBox.font.MeasureText(text)).Shrink(-4)
                : new Rectangle(rect.x, rect.y, rect.width, rect.height);
            if (isDisabled) return;
            if (!adjustedRect.IsMouseIn() || !IsMouseButtonPressed(MOUSE_LEFT_BUTTON)) return;
            Click();
        }

        public override void Render()
        {
            if (isDisabled) adjustedRect.Draw(disabledColor);
            else if (adjustedRect.IsMouseIn()) adjustedRect.Draw(hoverColor);
            else adjustedRect.Draw(baseColor);
            adjustedRect.MaskDraw(() =>
            {
                switch (buttonMode)
                {
                    case ButtonMode.SizeToText:
                        DrawTextEx(GameBox.font, text, new Vector2(rect.x, rect.y), 24, 1.5f, fontColor);
                        break;
                    case ButtonMode.WrapText:
                        GameBox.font.DrawTextWrap(text, adjustedRect, fontColor);
                        break;
                    case ButtonMode.CenterText:
                        GameBox.font.DrawCenterText(rect.Center(), text, fontColor);
                        break;
                }
            });
        }

        public override void PositionChange(Vector2 v2) => rect = rect.MoveTo(v2);

        public void ChangeColor(Color color, Color fontColor)
        {
            this.fontColor = fontColor;
            baseColor = color;
            hoverColor = new Color((int) Math.Min(color.r * 1.5, 255), (int) Math.Min(color.g * 1.5, 255),
                (int) Math.Min(color.b * 1.5, 255), color.a);
            disabledColor = new Color((int) (color.r / 1.7), (int) (color.g / 1.7), (int) (color.b / 1.7), color.a);
        }

        public void Click() => _clickEvent.ForEach(a => a.Invoke());
    }
}