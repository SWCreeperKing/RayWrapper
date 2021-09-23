using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static Raylib_cs.Raylib;
using static RayWrapper.Objs.Label;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs
{
    public class Button : GameObject
    {
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

        public Rectangle Rect => baseL.Rect;

        public Label baseL;
        public ColorModule baseColor = new(56, 73, 99);
        public ColorModule fontColor = new(174, 177, 181);
        public Actionable<bool> isDisabled = new(false);
        public Sound clickSound;
        public bool randomPitch = true;

        private List<Action> _clickEvent = new();

        /// <summary>
        /// this event will invoke all subscribers on button click 
        /// </summary>
        public event Action Clicked
        {
            add => _clickEvent.Add(value);
            remove => _clickEvent.Remove(value);
        }

        public Button(Vector2 pos, string text = "Untitled Button") : this(AssembleRectFromVec(pos, Vector2.Zero),
            text, TextMode.SizeToText)
        {
        }

        public Button(Rectangle rect, string text = "Untitled Button", TextMode textMode = TextMode.AlignCenter) :
            base(rect.Pos()) =>
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

        public override void Update() => baseL.Update();
        protected override void RenderCall() => baseL.Render();
        public override void PositionChange(Vector2 v2) => baseL.PositionChange(v2);

        public override Vector2 Size() => baseL.Size();
        //
        // public void ChangeColor(ColorModule color, ColorModule fontColor) =>
        //     (this.fontColor, baseColor) = (fontColor, color);

        private Color GetColor(ColorModule c) =>
            isDisabled
                ? ((Color)c).MakeDarker()
                : Rect.IsMouseIn()
                    ? ((Color)c).MakeLighter()
                    : c;

        /// <summary>
        /// Execute all methods subscribed to the on click event
        /// </summary>
        public void Click()
        {
            foreach (var a in _clickEvent) a.Invoke();
        }
    }
}