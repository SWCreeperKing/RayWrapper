﻿using System;
using System.Numerics;
using System.Runtime.InteropServices.ComTypes;
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
            get => _text.text;
            set
            {
                _text.text = value;
                _textLeng = _text.MeasureText().X;
                _textBox = new Vector2(35 + _textLeng, 40);
            }
        }

        public override Vector2 Position
        {
            get => _pos;
            set
            {
                _pos = value;
                _text.Position = value + _textOff;
            }
        }

        public override Vector2 Size => _textBox;

        public Action<bool> checkChange;
        public ColorModule checkedColor = new(60, 170, 80);
        public ColorModule emptyColor = new(170);
        public ColorModule hoverColor = new(255);
        public bool isChecked;
        public bool isCircle;
        public ColorModule textColor = new(192);

        private readonly Vector2 _posOff = new(5);
        private readonly Vector2 _size = new(20);
        private readonly Vector2 _textOff = new(35, 5);

        private Vector2 _pos;

        private float _textLeng;
        private Text _text;
        private Vector2 _textBox;

        public Checkbox(Vector2 pos, string text = "Untitled Checkbox")
        {
            _pos = pos;
            _text = new Text(text, pos + _textOff, new ColorModule(() => textColor));
            Text = text;
        }


        protected override void UpdateCall()
        {
            if (!AssembleRectFromVec(Position, _textBox).IsMouseIn() ||
                !Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON)) return;
            isChecked = !isChecked;
            checkChange?.Invoke(isChecked);
        }

        protected override void RenderCall()
        {
            var rect = AssembleRectFromVec(Position + _posOff, _size);
            var mouseIsIn = AssembleRectFromVec(Position, _textBox).IsMouseIn() &&
                            !IsMouseOccupied;
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

            _text.Draw();
        }
    }
}