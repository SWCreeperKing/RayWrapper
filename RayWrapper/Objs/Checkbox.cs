using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    public class Checkbox : GameObject
    {
        public Action<bool> checkChange;
        public Color checkedColor = new(60, 170, 80, 255);
        public Color emptyColor = new(170, 170, 170, 255);
        public Color hoverColor = new(255, 255, 255, 255);
        public bool isChecked;
        public bool isCircle;
        public string text;
        public Color textColor = new(192, 192, 198, 255);

        public Checkbox(Vector2 pos, string text = "Untitled Checkbox") : base(pos) => this.text = text;

        public override void Update()
        {
            var textLeng = GameBox.font.MeasureText(text).X;
            if (!RectWrapper.AssembleRectFromVec(Position, new Vector2(35 + textLeng, 40)).IsMouseIn() ||
                !Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON)) return;
            isChecked = !isChecked;
            checkChange?.Invoke(isChecked);
        }

        public override void Render()
        {
            var textLeng = GameBox.font.MeasureText(text).X;
            var rect = RectWrapper.AssembleRectFromVec(Position + new Vector2(5, 5), new Vector2(20, 20));
            var mouseIsIn = RectWrapper.AssembleRectFromVec(Position, new Vector2(35 + textLeng, 40)).IsMouseIn();
            if (!isCircle)
            {
                if (isChecked) rect.Draw(checkedColor);
                rect.DrawHallowRect(emptyColor);
                if (mouseIsIn) rect.DrawHallowRect(hoverColor);
            }
            else
            {
                if (isChecked) rect.DrawCircle(checkedColor);
                rect.DrawHallowCircle(emptyColor);
                if (mouseIsIn) rect.DrawHallowCircle(hoverColor);
            }

            GameBox.font.DrawText(text, Position + new Vector2(35, 5), textColor);
        }

        public override void PositionChange(Vector2 v2)
        {
        }
    }
}