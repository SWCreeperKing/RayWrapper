using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static RayWrapper.GameBox;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs
{
    public class Checkbox : GameObject
    {
        public string Text
        {
            get => _text;
            set => _textLeng = MeasureText(_text = value).X;
        }
        
        public Action<bool> checkChange;
        public Color checkedColor = new(60, 170, 80, 255);
        public Color emptyColor = new(170, 170, 170, 255);
        public Color hoverColor = new(255, 255, 255, 255);
        public bool isChecked;
        public bool isCircle;
        public Color textColor = new(192, 192, 198, 255);

        private readonly Vector2 _posOff = new(5);
        private readonly Vector2 _size = new(20);
        private readonly Vector2 _textOff = new(35, 5);
        private string _text;
        private float _textLeng;

        public Checkbox(Vector2 pos, string text = "Untitled Checkbox") : base(pos) => Text = text;

        public override void Update()
        {
            if (!AssembleRectFromVec(Position, new Vector2(35 + _textLeng, 40)).IsMouseIn() ||
                !Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON)) return;
            isChecked = !isChecked;
            checkChange?.Invoke(isChecked);
        }

        protected override void RenderCall()
        {
            var rect = AssembleRectFromVec(Position + _posOff, _size);
            var mouseIsIn = AssembleRectFromVec(Position, new Vector2(35 + _textLeng, 40)).IsMouseIn();
            if (!isCircle)
            {
                if (isChecked) rect.DrawRounded(checkedColor, .35f);
                rect.DrawRoundedLines(emptyColor, .35f);
                if (mouseIsIn) rect.DrawRoundedLines(hoverColor, .35f);
            }
            else
            {
                if (isChecked) rect.DrawCircle(checkedColor);
                rect.DrawHallowCircle(emptyColor);
                if (mouseIsIn) rect.DrawHallowCircle(hoverColor);
            }

            Text(_text, Position + _textOff, textColor);
        }

        public override void PositionChange(Vector2 v2)
        {
        }

        public override Vector2 Size() => _size + MeasureText(_text);
    }
}