using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Var_Interfaces;
using RayWrapper.Vars;
using ZimonIsHimUtils.ExtensionMethods;
using static Raylib_CsLo.Raylib;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs
{
    public class Button : GameObject
    {
        public static Style defaultStyle = new();

        public Style style { get; private set; } = defaultStyle.Copy();
        public Label baseL;
        public Sound clickSound;
        public Actionable<bool> isDisabled = false;
        public bool randomPitch = true;

        private readonly IList<Action> _clickEvent = new List<Action>();

        public Actionable<string> Text
        {
            get => baseL.text;
            set => baseL.text = value;
        }

        public Tooltip Tooltip
        {
            get => baseL.tooltip;
            set => baseL.tooltip = value;
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

        public Button(Vector2 pos, string text = "Untitled Button") : this(AssembleRectFromVec(pos, Vector2.Zero), text,
            Label.Style.DrawMode.SizeToText)
        {
        }

        public Button(Vector2 pos, Actionable<string> text) : this(AssembleRectFromVec(pos, Vector2.Zero), text,
            Label.Style.DrawMode.SizeToText)
        {
            Text = text;
        }

        public Button(Rectangle rect, string text = "Untitled Button",
            Label.Style.DrawMode drawMode = Label.Style.DrawMode.AlignCenter)
        {
            style.labelStyle.drawMode = drawMode;
            style.labelStyle.drawColor = (c, b) =>
            {
                if (isDisabled) return c.MakeDarker();
                return b ? c.MakeLighter() : c;
            };

            baseL = new Label(rect, text)
            {
                style = style.labelStyle,
                clicked = () =>
                {
                    if (isDisabled) return;
                    if (randomPitch) SetSoundPitch(clickSound, GameBox.Random.Next(.75f, 1.25f));
                    PlaySound(clickSound);
                    Click();
                }
            };
        }

        protected override void UpdateCall() => baseL.Update();

        protected override void RenderCall()
        {
            baseL.Render();
            if (!Rect.IsMouseIn()) return;
            if (_clickEvent.Any() && !isDisabled) GameBox.SetMouseCursor(MouseCursor.MOUSE_CURSOR_POINTING_HAND);
            else if (_clickEvent.Any()) GameBox.SetMouseCursor(MouseCursor.MOUSE_CURSOR_NOT_ALLOWED);
        }

        /// <summary>
        ///     Execute all methods subscribed to the on click event
        /// </summary>
        public void Click()
        {
            _clickEvent.Each(a => a.Invoke());
        }

        public void SetStyle(Style style)
        {
            this.style = style.Copy();
            this.style.labelStyle.drawColor = (c, b) =>
            {
                if (isDisabled) return c.MakeDarker();
                return b ? c.MakeLighter() : c;
            };

            baseL.style = this.style.labelStyle;
        }

        public class Style : IStyle<Style>
        {
            public ColorModule FontColor
            {
                get => _fontColor;
                set => _fontColor = value;
            }

            public ColorModule BackColor
            {
                get => _backColor;
                set => _backColor = value;
            }

            public Label.Style labelStyle = new();

            private ColorModule _fontColor = new(174, 177, 181);
            private ColorModule _backColor = new(1, 89, 99);

            public Style(Label.Style.DrawMode drawMode = Label.Style.DrawMode.SizeToText)
            {
                labelStyle.drawMode = drawMode;
                ReInit();
            }

            public Style ReInit()
            {
                labelStyle.backColor = new ColorModule(() => BackColor);
                labelStyle.fontColor = new ColorModule(() => FontColor);
                return this;
            }

            public Style Copy()
            {
                return new Style
                {
                    FontColor = FontColor.Copy(), BackColor = BackColor.Copy(), labelStyle = labelStyle.Copy()
                }.ReInit();
            }
        }
    }
}