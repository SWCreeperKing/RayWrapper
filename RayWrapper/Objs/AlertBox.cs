using System;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static RayWrapper.GameBox;

namespace RayWrapper.Objs
{
    public class AlertBox : GameObject
    {
        // todo: add styles a bit later
        public enum Result
        {
            Yes,
            No,
            Close,
        }

        public bool informationBox;
        public Action<Result> onResult = null;
        public string title;
        public string message;
        public ColorModule titleColor = new(70, 170, 70);
        public ColorModule messageColor = new(70, 140, 140);
        public ColorModule rectColor = new(60, 60, 60);
        public Button yes;
        public Button no;
        public Button close;

        private readonly Rectangle _rect;

        /// <summary>
        /// Create an alert box (used for warning and information checks)
        /// </summary>
        /// <param name="title">title of the alert</param>
        /// <param name="message">message of the alert</param>
        /// <param name="informationBox">if it should be an information box (close) or prompt box (yes/no/cancel)</param>
        public AlertBox(string title, string message, bool informationBox = false)
        {
            // should make one with an input box in the future
            (this.title, this.message, this.informationBox) = (title, message, informationBox);

            void Clicked(Result res)
            {
                Hide();
                onResult?.Invoke(res);
            }

            if (!informationBox)
            {
                no = new Button(Vector2.Zero, "no");
                yes = new Button(Vector2.Zero, "yes");
                no.Clicked += () => Clicked(Result.No);
                yes.Clicked += () => Clicked(Result.Yes);
            }

            close = new Button(Vector2.Zero, informationBox ? "close" : "x");
            close.Clicked += () => Clicked(Result.Close);

            var font = Text.Style.DefaultFont ?? Raylib.GetFontDefault();
            var halfScreen = WindowSize / 2;
            var messageSize = font.MeasureText(message, 30);
            var titleSize = font.MeasureText(title);
            var closeSize = close.Size;
            var noSize = informationBox ? Vector2.Zero : no.Size;
            var yesSize = informationBox ? Vector2.Zero : yes.Size;

            var backLength = informationBox
                ? Math.Max(Math.Max(titleSize.X, messageSize.X), closeSize.X)
                : Math.Max(Math.Max(titleSize.X, messageSize.X), yesSize.X + noSize.X + 20) + closeSize.X;

            var backHeight = informationBox
                ? titleSize.Y + messageSize.Y + closeSize.Y * 2
                : titleSize.Y + messageSize.Y + Math.Max(yesSize.Y, noSize.Y) + closeSize.Y;

            backHeight += 12;
            _rect = new Rectangle(halfScreen.X - backLength / 2, halfScreen.Y - backHeight / 2, backLength, backHeight);
            var bottom = _rect.Pos() + new Vector2(_rect.width / 2, _rect.height - 30);

            if (informationBox)
                close.Position = bottom - new Vector2(closeSize.X / 2, 3);
            else
            {
                close.Position = new Vector2(_rect.x + (_rect.width - closeSize.X) - 3, _rect.y + 3);
                no.Position = bottom + new Vector2(10, -3);
                yes.Position = bottom - new Vector2(yesSize.X + 10, 3);
            }
        }

        // yes, the set does nothing
        public override Vector2 Position
        {
            get => _rect.Pos();
            set => _rect.NewMoveTo(value);
        }

        public override Vector2 Size => _rect.Size();

        protected override void UpdateCall()
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
            var font = Text.Style.DefaultFont ?? Raylib.GetFontDefault();
            font.DrawCenterText(halfScreen - new Vector2(0, _rect.height / 2 - 15), title, titleColor, 30);
            font.DrawCenterText(halfScreen - new Vector2(0, 10), message, messageColor);

            close.Render();
            if (informationBox) return;
            no.Render();
            yes.Render();
        }

        /// <summary>
        /// shows the alertbox
        /// </summary>
        public void Show() => alertBox = this;

        /// <summary>
        /// hides the alertbox
        /// </summary>
        public void Hide() => alertBox = null;
    }
}