using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static RayWrapper.GameBox;

namespace RayWrapper.Objs
{
    public class Scrollbar : GameObject
    {
        public Func<float> amountInvoke;
        public Rectangle bar;
        public ColorModule barColor = new(116, 116, 116);
        public float barScale = 25;
        public Rectangle container;
        public ColorModule containerColor = new(78, 78, 78);
        public bool isVertical;
        public bool outline = true;
        public ColorModule outlineColor = new(Color.BLACK);
        
        private readonly List<Action<float>> _onMove = new();
        private float _trueSize;
        private float _visibleSize;

        public Scrollbar(Rectangle rect, bool isVertical = true)
        {
            container = rect.Clone();
            bar = new Rectangle();
            this.isVertical = isVertical;
        }

        public float Value { get; private set; }

        public override Vector2 Position
        {
            get => container.Pos();
            set
            {
                var offset = GetOffset;
                container = container.MoveTo(value);
                bar = bar.MoveTo(value);
                MoveBar(offset);
            }
        }

        public override Vector2 Size => container.Size();

        public float GetOffset => isVertical ? container.y - bar.y : container.x - bar.x;

        public event Action<float> OnMoveEvent
        {
            add => _onMove.Add(value);
            remove => _onMove.Remove(value);
        }

        public void MoveBar(float offset)
        {
            CalcBarSize();
            if (isVertical) bar.y -= offset;
            else bar.x -= offset;
            CalcBarSize();
            ClampBounds();
            foreach (var a in _onMove) a.Invoke(Value);
        }

        public void ClampBounds()
        {
            var (size, pos) = (container.Size(), container.Pos());
            (bar.width, bar.height) = isVertical ? (size.X, _visibleSize) : (_visibleSize, size.Y);
            var (hMax, wMax) = (pos.Y + size.Y, pos.X + size.X);
            bar.y = Math.Clamp(bar.y, pos.Y, (int)(isVertical ? hMax - _visibleSize : hMax));
            bar.x = Math.Clamp(bar.x, pos.X, (int)(isVertical ? wMax : wMax - _visibleSize));
        }

        public void CalcVal()
        {
            var (size, pos) = (container.Size(), container.Pos());
            var sub1 = isVertical ? bar.y - pos.Y : bar.x - pos.X;
            var sub2 = Math.Max((isVertical ? size.Y : size.X) - _visibleSize, 1);
            Value = Math.Clamp(sub1 / sub2 * (Amount() - 1), 0, float.MaxValue - 1);
        }

        public void CalcBarSize()
        {
            var size = container.Size();
            _trueSize = (isVertical ? size.Y : size.X) / Amount();
            _visibleSize = Math.Max(_trueSize, 10 * barScale);
        }

        public override void UpdateCall()
        {
            var mousePos = GameBox.mousePos;
            CalcBarSize();
            ClampBounds();
            CalcVal();

            var isLeft = Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON);
            if (container.IsMouseIn() && isLeft && !IsMouseOccupied) mouseOccupier = this;
            else if (mouseOccupier == this && !isLeft) mouseOccupier = null;
            if (mouseOccupier != this) return;
            var posDelta = isVertical ? bar.y - mousePos.Y : bar.x - mousePos.X;
            MoveBar(posDelta + _visibleSize / 2);
        }

        protected override void RenderCall()
        {
            var hover = IsMouseOccupied && mouseOccupier == this || !IsMouseOccupied && container.IsMouseIn();
            container.DrawRounded(hover ? ((Color)containerColor).MakeLighter() : containerColor, .4f);
            bar.DrawRounded(hover ? ((Color)barColor).MakeLighter() : barColor, .4f);
            if (!outline) return;
            container.DrawRoundedLines(outlineColor, .4f);
            bar.DrawRoundedLines(outlineColor, .4f);
        }

        public float Amount() => Math.Max(amountInvoke.Invoke(), 1);
    }
}