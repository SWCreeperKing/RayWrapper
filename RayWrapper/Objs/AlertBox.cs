using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static RayWrapper.FontManager;
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
            Close,
        }

        public bool informationBox;
        public Action<Result> onResult = null;
        public Vector2 size;
        public string title;
        public string message;
        public ColorModule titleColor = new(70, 170, 70);
        public ColorModule messageColor = new(70, 140, 140);
        public ColorModule rectColor = new(60, 60, 60);
        public Button yes;
        public Button no;
        public Button close;

        private readonly Rectangle _rect;

        public AlertBox(string title, string message, bool informationBox = false) : this(title, message, Vector2.Zero,
            informationBox)
        {
        }

        // TODO: Update docs.
        /// <param name="size">Custom size for the Message Text</param>
        public AlertBox(string title, string message, Vector2 size, bool informationBox = false)
        {
            (this.title, this.message, this.size, this.informationBox) = (title, message, size, informationBox);

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
                no.baseL.CheckText();
                yes.baseL.CheckText();
            }

            close = new Button(Vector2.Zero, informationBox ? "close" : "x");
            close.Clicked += () => Clicked(Result.Close);
            close.baseL.CheckText();

            var halfScreen = WindowSize / 2;
            var messageSize = size == Vector2.Zero ? GetDefFont(30).MeasureText(message, 30) : size;
            var titleSize = GetDefFont().MeasureText(title);
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
            GetDefFont(30).DrawCenterText(halfScreen - new Vector2(0, _rect.height / 2 - 15), title, titleColor, 30);
            if (size == Vector2.Zero)
            {
                GetDefFont().DrawCenterText(halfScreen - new Vector2(0, 10), message, messageColor);
            }
            else
            {
                GetDefFont().DrawTextRec(message,
                    AssembleRectFromVec(halfScreen - new Vector2(size.X / 2, _rect.height / 2 - 25), size),
                    messageColor);
            }

            close.Render();
            if (informationBox) return;
            no.Render();
            yes.Render();
        }

        public void Show() => alertBox = this;
        public void Hide() => alertBox = null;
    }
}