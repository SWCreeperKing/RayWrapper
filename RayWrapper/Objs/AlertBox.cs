using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static RayWrapper.GameBox;

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

        public Button close;
        public string message;
        public Color messageColor = new(70, 140, 140, 255);
        public Button no;
        public Action<Result> onResult = null;
        public Color rectColor = new Color(60, 60, 60, 255);
        public string title;
        public Color titleColor = new(70, 170, 70, 255);
        public Button yes;

        private Rectangle _rect;

        public AlertBox(string title, string message) : base(new Vector2())
        {
            (this.title, this.message) = (title, message);
            close = new Button(new Rectangle(0, 0, 0, 0), "x", Button.ButtonMode.SizeToText);
            no = new Button(new Rectangle(0, 0, 0, 0), "no", Button.ButtonMode.SizeToText);
            yes = new Button(new Rectangle(0, 0, 0, 0), "yes", Button.ButtonMode.SizeToText);

            close.Clicked += () =>
            {
                Hide();
                onResult?.Invoke(Result.Close);
            };
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
            
            close.Update();
            no.Update();
            yes.Update();
            
            var halfScreen = WindowSize / 2;
            var messageSize = font.MeasureText(message, 30);
            var titleSize = font.MeasureText(title);
            var closeSize = close.adjustedRect.Size();
            var noSize = no.adjustedRect.Size();
            var yesSize = yes.adjustedRect.Size();
            var backLeng = Math.Max(Math.Max(titleSize.X, messageSize.X), yesSize.X + noSize.X + 20) + closeSize.X;
            var backHeight = titleSize.Y + messageSize.Y + Math.Max(yesSize.Y, noSize.Y) + closeSize.Y;
            _rect = new Rectangle(halfScreen.X - backLeng / 2, halfScreen.Y - backHeight / 2, backLeng, backHeight);
            
            close.MoveTo(new Vector2(_rect.x + (_rect.width - closeSize.X) + 4, _rect.y + 4));
            var bottom = _rect.Pos() + new Vector2(_rect.width / 2, _rect.height - 30);
            no.MoveTo(bottom + new Vector2(10, 0));
            yes.MoveTo(bottom - new Vector2(yesSize.X + 10, 0));
        }

        public override void Update()
        {
            close.Update();
            no.Update();
            yes.Update();
        }

        public override void Render()
        {
            var screen = WindowSize;
            var halfScreen = screen / 2;
            new Rectangle(0, 0, screen.X, screen.Y).Draw(new Color(0, 0, 0, 150));
            _rect.Draw(rectColor);
            font.DrawCenterText(halfScreen - new Vector2(0, _rect.height/2 - 15), title, titleColor, 30);
            font.DrawCenterText(halfScreen - new Vector2(0, _rect.height/2 - 50), message, messageColor);
            close.Render();
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