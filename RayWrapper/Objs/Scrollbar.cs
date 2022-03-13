using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;
using static RayWrapper.GameBox;

namespace RayWrapper.Objs
{
    public class Scrollbar : GameObject
    {
        public static Style defaultStyle = new();

        public Style style = defaultStyle.Copy();
        public Rectangle bar = RectWrapper.Zero;
        public Rectangle container;
        public int minSizePercent = 20;
        public bool isVertical;
        public Func<float> amountInvoke;

        private readonly IList<Action<float>> _onMove = new List<Action<float>>();
        private float _trueSize;
        private float _visibleSize;
        private Vector2 _lastMouse = Vector2.Zero;

        public Scrollbar(Rectangle rect, bool isVertical = true)
        {
            container = rect.Clone();
            this.isVertical = isVertical;
        }

        public float Value { get; private set; }

        public override Vector2 Position
        {
            get => container.Pos();
            set
            {
                var offset = GetOffset;
                container.MoveTo(value);
                bar.MoveTo(value);
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
            ClampBounds();
            foreach (var a in _onMove) a.Invoke(Value);
        }

        public void ClampBounds()
        {
            CalcBarSize();
            var (size, pos) = (container.Size(), container.Pos());
            (bar.width, bar.height) = isVertical ? (size.X, _visibleSize) : (_visibleSize, size.Y);
            var (hMax, wMax) = (pos.Y + size.Y, pos.X + size.X);
            bar.y = Math.Clamp(bar.y, pos.Y, (int) (isVertical ? hMax - _visibleSize : hMax));
            bar.x = Math.Clamp(bar.x, pos.X, (int) (isVertical ? wMax : wMax - _visibleSize));
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
            var rSize = isVertical ? size.Y : size.X;
            _trueSize = rSize / Amount();
            _visibleSize = Math.Max(_trueSize, rSize * (minSizePercent / 100f));
        }

        protected override void UpdateCall()
        {
            var mousePos = GameBox.mousePos;
            CalcBarSize();
            ClampBounds();
            CalcVal();

            var isLeft = IsMouseButtonDown(MOUSE_LEFT_BUTTON);

            if (bar.IsMouseIn() && isLeft && !IsMouseOccupied)
            {
                _lastMouse = GameBox.mousePos;
                mouseOccupier = this;
            }
            else if (mouseOccupier == this && !isLeft) mouseOccupier = null;

            if (mouseOccupier != this)
            {
                if (!IsMouseOccupied && IsMouseButtonPressed(MOUSE_LEFT_BUTTON) &&
                    container.IsMouseIn())
                    MoveBar((isVertical ? bar.y - mousePos.Y : bar.x - mousePos.X) + _visibleSize / 2);

                _lastMouse = Vector2.Zero;
                return;
            }

            MoveBar(isVertical ? _lastMouse.Y - mousePos.Y : _lastMouse.X - mousePos.X);
            _lastMouse = GameBox.mousePos;
        }

        protected override void RenderCall()
        {
            if (Amount() == 1) return; // ignore loss of precision
            style.Draw(container, bar,
                IsMouseOccupied && mouseOccupier == this || !IsMouseOccupied && container.IsMouseIn());
        }

        public float Amount() => Math.Max(amountInvoke?.Invoke() ?? 0, 1);

        public class Style : IStyle<Style>
        {
            public RectStyle containerStyle = new();
            public RectStyle barStyle = new();
            public OutlineStyle containerOutline = new();
            public OutlineStyle barOutline = new();
            public ColorModule containerColor = new(78, 78, 78);
            public ColorModule barColor = new(116, 116, 116);
            public bool drawOutline = true;

            public void Draw(Rectangle container, Rectangle bar, bool hover)
            {
                containerStyle.color = hover ? ((Color) containerColor).MakeLighter() : (Color) containerColor;
                barStyle.color = hover ? ((Color) barColor).MakeLighter() : (Color) barColor;
                containerStyle.Draw(container);
                containerOutline.Draw(container);
                if (!drawOutline) return;
                barStyle.Draw(bar);
                barOutline.Draw(bar);
            }

            public Style Copy()
            {
                return new Style
                {
                    containerStyle = containerStyle.Copy(), containerOutline = containerOutline.Copy(),
                    containerColor = containerColor.Copy(), barStyle = barStyle.Copy(), barOutline = barOutline.Copy(),
                    barColor = barColor.Copy(), drawOutline = drawOutline
                };
            }
        }
    }
}