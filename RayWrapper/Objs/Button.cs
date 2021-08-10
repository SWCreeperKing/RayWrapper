using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static Raylib_cs.MouseButton;
using static Raylib_cs.Raylib;
using static RayWrapper.RectWrapper;

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

        /// <summary>
        /// this event will invoke all subscribers on button click 
        /// </summary>
        public event Action Clicked
        {
            add => _clickEvent.Add(value);
            remove => _clickEvent.Remove(value);
        }

        public Button(Vector2 pos, string text = "Untitled Button") : this(AssembleRectFromVec(pos, new Vector2()),
            text, ButtonMode.SizeToText)
        {
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

        protected override void RenderCall()
        {
            if (isDisabled) adjustedRect.Draw(disabledColor);
            else if (adjustedRect.IsMouseIn()) adjustedRect.Draw(hoverColor);
            else adjustedRect.Draw(baseColor);
            adjustedRect.MaskDraw(() =>
            {
                switch (buttonMode)
                {
                    case ButtonMode.SizeToText:
                        Text(text, rect.Pos(), fontColor);
                        break;
                    case ButtonMode.WrapText:
                        TextWrap(text, adjustedRect, fontColor);
                        break;
                    case ButtonMode.CenterText:
                        TextCenter(text, rect, fontColor);
                        break;
                }
            });
        }

        /// <summary>
        /// changes position of a vector2; 
        /// </summary>
        /// <param name="v2">the vector to move it to</param>
        public override void PositionChange(Vector2 v2) => rect = rect.MoveTo(v2);

        /// <summary>
        /// change button colors
        /// </summary>
        /// <param name="color">main color, auto makes hover and disabled color</param>
        /// <param name="fontColor">color of the text</param>
        public void ChangeColor(Color color, Color fontColor)
        {
            this.fontColor = fontColor;
            baseColor = color;
            (hoverColor, disabledColor) = color.MakeLightDark();
        }

        /// <summary>
        /// Execute all methods subscribed to the on click event
        /// </summary>
        public void Click()
        {
            foreach (var a in _clickEvent) a.Invoke();
        }
    }
}