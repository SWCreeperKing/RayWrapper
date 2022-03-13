using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;

namespace RayWrapper.Objs
{
    public class ScrollView : GameObject
    {
        public override Vector2 Position
        {
            get => _pos;
            set
            {
                _rect.MoveTo(_pos = value);
                _yScroll.Position = new Vector2(_rect.x, _rect.y);
                _xScroll.Position = new Vector2(_rect.x + 20, _rect.y + _rect.height - 20);
            }
        }

        public override Vector2 Size => _size;

        private readonly Scrollbar _yScroll;
        private readonly Scrollbar _xScroll;
        private readonly List<IGameObject> _gos = new();
        private IList<IGameObject> _renderList = new List<IGameObject>();
        private Rectangle _rect;
        private Vector2 _size;
        private Vector2 _pos;
        private Vector2 _posOffset = Vector2.Zero;
        private Vector2 _trueSize;
        
        //todo: scrollbars borked
        public ScrollView(Rectangle rect)
        {
            _rect = new Rectangle(rect.x + 20, rect.y, rect.width - 20, rect.height - 20);
            _yScroll = new Scrollbar(new Rectangle(rect.x, rect.y, 20, rect.height - 20));
            _xScroll = new Scrollbar(new Rectangle(rect.x + 20, rect.y + rect.height - 20, rect.width - 20, 20))
                { isVertical = false };
            _pos = _rect.Pos();
            _trueSize = _size = _rect.Size();
            _xScroll.amountInvoke = () => Math.Abs(_trueSize.X - _size.X) + 1;
            _yScroll.amountInvoke = () => Math.Abs(_trueSize.Y - _size.Y) + 1;
            _xScroll.OnMoveEvent += _ => Recalc();
            _yScroll.OnMoveEvent += _ => Recalc();
        }

        protected override void UpdateCall()
        {
            if (!_gos.Any()) return;
            if (_trueSize.X >= _size.X) _xScroll.Update();
            if (_trueSize.Y >= _size.Y) _yScroll.Update();
            foreach (var obj in _renderList) obj.Update();
        }

        protected override void RenderCall()
        {
            if (!_gos.Any()) return;

            _rect.MaskDraw(() =>
            {
                foreach (var obj in _renderList) obj.Render();
            });

            if (_trueSize.X >= _size.X) _xScroll.Render();
            if (_trueSize.Y >= _size.Y) _yScroll.Render();
            _rect.DrawHallowRect(BLACK, 1);
        }

        public void Recalc()
        {
            if (!_gos.Any()) return;
            foreach (var go in _renderList) go.SetPositionAsReserveV2();
            var pos = Position;
            foreach (var go in _gos) go.Update();
            var v2s = _gos.Select(g => g.Position - pos + g.Size);
            _trueSize = new Vector2(Math.Max(_trueSize.X, v2s.Max(g => g.X) + 6),
                Math.Max(_trueSize.Y, v2s.Max(g => g.Y) + 6));
            _posOffset = new Vector2(_xScroll.Value, _yScroll.Value) - new Vector2(3);
            var tempRect = new Rectangle(_rect.x + _posOffset.X, _rect.y + _posOffset.Y, _rect.width, _rect.height);
            _renderList.Clear();
            _renderList = _gos.Where(g => CheckCollisionRecs(g.GetDebugRect(), tempRect)).ToList();
            foreach (var go in _renderList) go.Position -= _posOffset;
        }

        public void AddObj(params IGameObject[] objs)
        {
            var pos = Position;

            foreach (var t in objs)
            {
                t.Position += pos;
                t.ReserveV2();
            }

            _gos.AddRange(objs);
            Recalc();
        }
    }
}