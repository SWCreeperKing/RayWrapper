using System;
using System.Collections.Generic;
using Raylib_cs;
using static RayWrapper.RayWrapper;

namespace RayWrapper.Objs
{
    public class Scrollbar : IGameObject
    {
        public float Value { get; private set; }

        public Rectangle container;
        public Rectangle bar;
        public Color containerColor = new(78, 78, 78, 255);
        public Color barColor = new(116, 116, 116, 255);
        public int amount;
        public bool isVertical;
        private float _visibleSize;
        private float _trueSize;
        private bool _occupier;
        private List<Action<float>> _onMove = new();

        public event Action<float> OnMoveEvent
        {
            add => _onMove.Add(value);
            remove => _onMove.Remove(value);
        }

        public Scrollbar(Rectangle rect, bool isVertical = true)
        {
            container = rect.Clone();
            bar = new();
            this.isVertical = isVertical;
        }

        public void MoveBar(int offset)
        {
            if (isVertical) bar.y -= offset;
            else bar.x -= offset;
            BoundsCheck();
            _onMove.ForEach(a => a.Invoke(Value));
        }

        public void BoundsCheck()
        {
            var (size, pos) = (container.Size(), container.Pos());
            (bar.width, bar.height) = isVertical ? (size.X, _visibleSize) : (_visibleSize, size.Y);
            var (hMax, wMax) = (pos.Y + size.Y, pos.X + size.X);
            if (isVertical)
            {
                if (bar.y < pos.Y) bar.y = pos.Y;
                else if (bar.y > hMax - _visibleSize) bar.y = (int) (hMax - _visibleSize);
                if (bar.x < pos.X) bar.x = pos.X;
                else if (bar.x > wMax) bar.x = wMax;
            }
            else
            {
                if (bar.y < pos.Y) bar.y = pos.Y;
                else if (bar.y > hMax) bar.y = hMax;
                if (bar.x < pos.X) bar.x = pos.X;
                else if (bar.x > wMax - _visibleSize) bar.x = (int) (wMax - _visibleSize);
            }
        }

        public void Update()
        {
            var (size, pos, mousePos) = (container.Size(), container.Pos(), Raylib.GetMousePosition());
            BoundsCheck();
            if (amount <= 0) amount = 1;
            _trueSize = (isVertical ? size.Y : size.X) / amount;
            _visibleSize = _trueSize;
            if (_visibleSize < 10) _visibleSize = 10;
            Value = (isVertical ? bar.y - pos.Y : bar.x - pos.X) /
                ((isVertical ? size.Y : size.X) - _visibleSize) * (amount - 1);
            if (Value < 0) Value = 0;

            var isLeft = Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON);
            if (container.IsMouseIn() && isLeft && !MouseOccupied) MouseOccupied = _occupier = true;
            else if (_occupier && !isLeft) MouseOccupied = _occupier = false;
            if (!_occupier) return;
            if (isVertical) MoveBar((int) (bar.y - mousePos.Y + _visibleSize / 2));
            else MoveBar((int) (bar.x - mousePos.X + _visibleSize / 2));
        }

        public void Render()
        {
            container.Draw(containerColor);
            bar.Draw(barColor);
        }
    }
}