using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static RayWrapper.GeneralWrapper;

namespace RayWrapper.Objs
{
    public class Scrollbar : GameObject
    {
        public float Value { get; private set; }

        public Rectangle container;
        public Rectangle bar;
        public float barScale = 25;
        public Color containerColor = new(78, 78, 78, 255);
        public Color barColor = new(116, 116, 116, 255);
        public float amount;
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

        public Scrollbar(Rectangle rect, bool isVertical = true) : base(rect.Pos())
        {
            container = rect.Clone();
            bar = new Rectangle();
            this.isVertical = isVertical;
        }

        public void MoveBar(float offset)
        {
            if (isVertical) bar.y -= offset;
            else bar.x -= offset;
            BoundsCheck();
            foreach (var a in _onMove) a.Invoke(Value);
        }

        public void BoundsCheck()
        {
            var (size, pos) = (container.Size(), container.Pos());
            (bar.width, bar.height) = isVertical ? (size.X, _visibleSize) : (_visibleSize, size.Y);
            var (hMax, wMax) = (pos.Y + size.Y, pos.X + size.X);
            var newHMax = isVertical ? hMax - _visibleSize : hMax;
            var newWMax = isVertical ? wMax : wMax - _visibleSize;
            
            if (bar.y < pos.Y) bar.y = pos.Y;
            else if (bar.y > newHMax) bar.y = (int) newHMax;
            if (bar.x < pos.X) bar.x = pos.X;
            else if (bar.x > newWMax) bar.x = (int) newWMax;
        }

        public override void Update()
        {
            var (size, pos, mousePos) = (container.Size(), container.Pos(), Raylib.GetMousePosition());
            _trueSize = (isVertical ? size.Y : size.X) / amount;
            _visibleSize = _trueSize;
            if (_visibleSize < 10 * barScale) _visibleSize = 10 * barScale;
            BoundsCheck();
            if (amount <= 0) amount = 1;
            Value = (isVertical ? bar.y - pos.Y : bar.x - pos.X) /
                ((isVertical ? size.Y : size.X) - _visibleSize) * (amount - 1);
            if (Value < 0 || double.IsNaN(Value)) Value = 0;

            var isLeft = Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON);
            if (container.IsMouseIn() && isLeft && !MouseOccupied) MouseOccupied = _occupier = true;
            else if (_occupier && !isLeft) MouseOccupied = _occupier = false;
            if (!_occupier) return;
            if (isVertical) MoveBar((int) (bar.y - mousePos.Y + _visibleSize / 2));
            else MoveBar((int) (bar.x - mousePos.X + _visibleSize / 2));
        }

        protected override void RenderCall()
        {
            container.Draw(containerColor);
            bar.Draw(barColor);
        }

        public override void PositionChange(Vector2 v2)
        {
            var offset = GetOffset;
            container = container.MoveTo(v2);
            bar = bar.MoveTo(v2);
            MoveBar(offset);
        }
        
        public float GetOffset => isVertical ? container.y - bar.y : container.x - bar.x;
    }
}