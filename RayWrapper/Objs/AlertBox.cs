﻿using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static RayWrapper.GameBox;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs
{
    public class AlertBox : GameObject
    {
        public enum Result
        {
            Yes,
            No,
            Close
        }

        public string message;
        public string title;
        public bool informationBox;
        public Button close;
        public Button no;
        public Button yes;
        public Color messageColor = new(70, 140, 140, 255);
        public Color rectColor = new(60, 60, 60, 255);
        public Color titleColor = new(70, 170, 70, 255);
        public Vector2 size;
        public Action<Result> onResult = null;

        private Rectangle _rect;

        public AlertBox(string title, string message, bool informationBox = false) : this(title, message, Vector2.Zero,
            informationBox)
        {
        }

        /// <param name="size">Custom size for the Message Text</param>
        public AlertBox(string title, string message, Vector2 size, bool informationBox = false) : base(new Vector2())
        {
            (this.title, this.message, this.size, this.informationBox) = (title, message, size, informationBox);

            if (!informationBox)
            {
                no = new Button(new Rectangle(0, 0, 0, 0), "no", Button.ButtonMode.SizeToText);
                yes = new Button(new Rectangle(0, 0, 0, 0), "yes", Button.ButtonMode.SizeToText);

                no.Clicked += () =>
                {
                    Hide();
                    onResult?.Invoke(Result.No);
                };
                yes.Clicked += () =>
                {
                    Hide();
                    onResult?.Invoke(Result.Yes);
                };

                no.Update();
                yes.Update();
            }

            close = new Button(new Rectangle(0, 0, 0, 0), informationBox ? "close" : "x", Button.ButtonMode.SizeToText);
            close.Clicked += () =>
            {
                Hide();
                onResult?.Invoke(Result.Close);
            };
            close.Update();

            var halfScreen = WindowSize / 2;
            var messageSize = size == Vector2.Zero ? GameBox.Font.MeasureText(message, 30) : size;
            var titleSize = GameBox.Font.MeasureText(title);
            var closeSize = close.adjustedRect.Size();
            var noSize = informationBox ? Vector2.Zero : no.adjustedRect.Size();
            var yesSize = informationBox ? Vector2.Zero : yes.adjustedRect.Size();
            var backLeng = informationBox
                ? Math.Max(Math.Max(titleSize.X, messageSize.X), closeSize.X)
                : Math.Max(Math.Max(titleSize.X, messageSize.X), yesSize.X + noSize.X + 20) + closeSize.X;
            var backHeight = informationBox
                ? titleSize.Y + messageSize.Y + closeSize.Y * 2
                : titleSize.Y + messageSize.Y + Math.Max(yesSize.Y, noSize.Y) + closeSize.Y;
            _rect = new Rectangle(halfScreen.X - backLeng / 2, halfScreen.Y - backHeight / 2, backLeng, backHeight);
            var bottom = _rect.Pos() + new Vector2(_rect.width / 2, _rect.height - 30);

            if (!informationBox)
            {
                close.MoveTo(new Vector2(_rect.x + (_rect.width - closeSize.X) + 4, _rect.y + 4));
                no.MoveTo(bottom + new Vector2(10, 0));
                yes.MoveTo(bottom - new Vector2(yesSize.X + 10, 0));
            }
            else close.MoveTo(bottom - new Vector2(closeSize.X / 2, 0));
        }

        public override void Update()
        {
            close.Update();
            if (informationBox) return;
            no.Update();
            yes.Update();
        }

        protected override void RenderCall()
        {
            var screen = WindowSize;
            var halfScreen = screen / 2;
            new Rectangle(0, 0, screen.X, screen.Y).Draw(new Color(0, 0, 0, 150));
            _rect.Draw(rectColor);
            GameBox.Font.DrawCenterText(halfScreen - new Vector2(0, _rect.height / 2 - 15), title, titleColor, 30);
            if (size == Vector2.Zero)
                GameBox.Font.DrawCenterText(halfScreen - new Vector2(0, _rect.height / 2 - 55), message, messageColor);
            else
                GameBox.Font.DrawTextWrap(message,
                    AssembleRectFromVec(halfScreen - new Vector2(size.X / 2, _rect.height / 2 - 55), size),
                    messageColor);
            close.Render();
            if (informationBox) return;
            no.Render();
            yes.Render();
        }

        public void Show() => alertBox = this;
        public void Hide() => alertBox = null;

        public override void PositionChange(Vector2 v2)
        {
        }
    }
}