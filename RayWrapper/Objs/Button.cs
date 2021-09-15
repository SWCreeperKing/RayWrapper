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
        public ColorModule baseColor = new(56, 73, 99);
        public ColorModule fontColor = new(174, 177, 181);
        public ColorModule outlineColor = new(Color.BLACK);
        public Actionable<string> tooltip;
        public Actionable<string> text;
        public Actionable<bool> isDisabled = new(false);
        public Sound clickSound;
        public ButtonMode buttonMode;
        public bool randomPitch = true;
        public bool outline = true;

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
            : base(rect.Pos()) =>
            (this.rect, this.text, this.buttonMode) = (rect, new Actionable<string>(text), buttonMode);

        public override void Update()
        {
            if (!isVisible.Invoke()) return;
            adjustedRect = buttonMode == ButtonMode.SizeToText
                ? rect.AdjustWh(GameBox.Font.MeasureText(text)).Shrink(-4)
                : new Rectangle(rect.x, rect.y, rect.width, rect.height);
            if (isDisabled) return;
            if (!adjustedRect.IsMouseIn() || !IsMouseButtonPressed(MOUSE_LEFT_BUTTON)) return;
            if (randomPitch) SetSoundPitch(clickSound, GameBox.Random.Next(.75f, 1.25f));
            PlaySound(clickSound);
            Click();
        }

        protected override void RenderCall()
        {
            Color bc = baseColor;
            Color fc = fontColor;

            var clr = isDisabled
                ? bc.MakeLightDark().dark
                : adjustedRect.IsMouseIn()
                    ? bc.MakeLightDark().light
                    : bc;

            adjustedRect.DrawRounded(clr, .25f);
            if (outline) adjustedRect.DrawRoundedLines(outlineColor, .25f);
            adjustedRect.MaskDraw(() =>
            {
                switch (buttonMode)
                {
                    case ButtonMode.SizeToText:
                        Text(text, rect.Pos(), fc);
                        break;
                    case ButtonMode.WrapText:
                        TextWrap(text, adjustedRect, fc);
                        break;
                    case ButtonMode.CenterText:
                        TextCenter(text, rect, fc);
                        break;
                }
            });
            var t = tooltip ?? "";
            if (adjustedRect.IsMouseIn() && t != "") adjustedRect.DrawTooltip(t);
        }

        /// <summary>
        /// changes position of a vector2; 
        /// </summary>
        /// <param name="v2">the vector to move it to</param>
        public override void PositionChange(Vector2 v2) => rect = rect.MoveTo(v2);

        public override Vector2 Size() => adjustedRect.Size();

        /// <summary>
        /// change button colors
        /// </summary>
        /// <param name="color">main color, auto makes hover and disabled color</param>
        /// <param name="fontColor">color of the text</param>
        public void ChangeColor(ColorModule color, ColorModule fontColor)
        {
            this.fontColor = fontColor;
            baseColor = color;
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