using System;
using System.Collections.Generic;
using Raylib_cs;
using static Raylib_cs.MouseButton;
using static Raylib_cs.Raylib;

namespace RayWrapper.Objs
{
    public class Button : IGameObject
    {
        public enum ButtonMode
        {
            SizeToText,
            CenterText,
            WrapText
        }

        public Rectangle rect;
        public Color baseColor = new(56, 73, 99, 255);
        public Color hoverColor = new(99, 129, 175, 255);
        public Color disabledColor = new(13, 17, 23, 255);
        public Color fontColor = new(174, 177, 181, 255);
        public ButtonMode buttonMode;
        public string text;
        public bool isDisabled;

        private List<Action> _clickEvent = new();
        private Rectangle adjustedRect;

        public event Action Clicked
        {
            add => _clickEvent.Add(value);
            remove => _clickEvent.Remove(value);
        }

        public Button(Rectangle rect, string text = "Untitled Button", ButtonMode buttonMode = ButtonMode.CenterText) =>
            (this.rect, this.text, this.buttonMode) = (rect, text, buttonMode);

        public void Update()
        {
            adjustedRect = buttonMode == ButtonMode.SizeToText
                ? rect.AdjustWH(GameBox.font.MeasureText(text)).Shrink(-4)
                : new(rect.x, rect.y, rect.width, rect.height);
            if (isDisabled) return;

            if (!adjustedRect.IsMouseIn() || !IsMouseButtonPressed(MOUSE_LEFT_BUTTON)) return;
            _clickEvent.ForEach(a => a.Invoke());
        }

        public void Render()
        {
            if (isDisabled) adjustedRect.Draw(disabledColor);
            else if (adjustedRect.IsMouseIn()) adjustedRect.Draw(hoverColor);
            else adjustedRect.Draw(baseColor);
            switch (buttonMode)
            {
                case ButtonMode.SizeToText:
                    DrawTextEx(GameBox.font, text, new(rect.x, rect.y), 24, 1.5f, fontColor);
                    break;
                case ButtonMode.WrapText:
                    GameBox.font.DrawTextWrap(text, adjustedRect, fontColor);
                    break;
                case ButtonMode.CenterText:
                    GameBox.font.DrawCenterText(rect.Center(), text, fontColor);
                    break;
            }
        }
    }
}