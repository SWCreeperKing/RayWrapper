using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;
using static RayWrapper.GameBox;
using static RayWrapper.Objs.Label;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs
{
    public class Button : GameObject
    {
        public Label baseL;
        public Sound clickSound;
        public ColorModule baseColor = new(1, 89, 99);
        public ColorModule fontColor = new(174, 177, 181);
        public Actionable<bool> isDisabled = new(false);
        public bool randomPitch = true;

        private readonly IList<Action> _clickEvent = new List<Action>();

        public Button(Vector2 pos, string text = "Untitled Button") : this(AssembleRectFromVec(pos, Vector2.Zero),
            text, TextMode.SizeToText)
        {
        }

        public Button(Rectangle rect, string text = "Untitled Button", TextMode textMode = TextMode.AlignCenter) =>
            baseL = new Label(rect, text)
            {
                textMode = new Actionable<TextMode>(() => textMode), outline = true,
                backColor = new ColorModule(() => GetColor(baseColor)),
                fontColor = new ColorModule(() => GetColor(fontColor)),
                clicked = () =>
                {
                    if (isDisabled) return;
                    if (randomPitch) SetSoundPitch(clickSound, GameBox.Random.Next(.75f, 1.25f));
                    PlaySound(clickSound);
                    Click();
                }
            };

        public bool Outline
        {
            get => baseL.outline;
            set => baseL.outline = value;
        }

        public Actionable<string> Text
        {
            get => baseL.text;
            set => baseL.text = value;
        }

        public Actionable<TextMode> Mode
        {
            get => baseL.textMode;
            set => baseL.textMode = value;
        }

        public Actionable<string> Tooltip
        {
            get => baseL.tooltip;
            set => baseL.tooltip = value;
        }

        public ColorModule OutlineColor
        {
            get => baseL.outlineColor;
            set => baseL.outlineColor = value;
        }

        public override Vector2 Position
        {
            get => baseL.Position;
            set => baseL.Position = value;
        }

        public override Vector2 Size => baseL.Size;
        public Rectangle Rect => baseL.Rect;

        /// <summary>
        ///     this event will invoke all subscribers on button click
        /// </summary>
        public event Action Clicked
        {
            add => _clickEvent.Add(value);
            remove => _clickEvent.Remove(value);
        }

        protected override void UpdateCall() => baseL.Update();
        protected override void RenderCall() => baseL.Render();

        private Color GetColor(ColorModule c) =>
            isDisabled
                ? ((Color) c).MakeDarker()
                : Rect.IsMouseIn() && !IsMouseOccupied
                    ? ((Color) c).MakeLighter()
                    : c;

        /// <summary>
        ///     Execute all methods subscribed to the on click event
        /// </summary>
        public void Click()
        {
            foreach (var a in _clickEvent) a.Invoke();
        }
    }
}