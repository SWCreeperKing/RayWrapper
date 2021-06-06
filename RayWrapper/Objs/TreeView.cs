using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Objs.TreeViewShapes;
using RayWrapper.Vars;
using static Raylib_cs.MouseButton;

namespace RayWrapper.Objs
{
    public class TreeView : GameObject
    {
        public Rectangle mask = new(0, 0, 0, 0);

        private Vector2 _lastPos = new();
        private bool _hold = false;
        private Vector2 _moveChange = new();
        private List<TreeViewShape> _nodes = new();
        private List<Action<string>> _onClick = new();
        private float _scale = 32;

        public event Action<string> OnClick
        {
            add => _onClick.Add(value);
            remove => _onClick.Remove(value);
        }

        public TreeView() : base(new Vector2())
        {
        }

        public override void Update()
        {
            if (GeneralWrapper.MouseOccupied) return;
            if (Raylib.IsMouseButtonPressed(MOUSE_RIGHT_BUTTON))
            {
                _moveChange = new();
                _scale = 32;
            }

            if (Raylib.IsMouseButtonDown(MOUSE_LEFT_BUTTON))
            {
                var curMouse = Raylib.GetMousePosition();
                if (!_hold) _lastPos = curMouse;
                _moveChange += (curMouse - _lastPos) / (_scale / 1.4f);
                _lastPos = curMouse;
                _hold = true;
            }
            else _hold = false;

            var scroll = Raylib.GetMouseWheelMove();
            if (scroll != 0) _scale = Math.Min(Math.Max(15, _scale + scroll), 55);
        }

        public override void Render()
        {
            var rect = mask.IsEqualTo(new(0, 0, 0, 0))
                ? RectWrapper.AssembleRectFromVec(new Vector2(0), GameBox.WindowSize)
                : mask;
            IEnumerable<string> id = Array.Empty<string>();
            id = _nodes.Select(n =>
            {
                var s = "";
                rect.MaskDraw(() => s = n.Draw(_moveChange, _scale));
                return s;
            }).Where(s => s != "");
            if (id.Any() && !GeneralWrapper.MouseOccupied) _onClick.ForEach(c => c.Invoke(id.First()));
        }

        public void AddNode(params TreeViewShape[] shapes) => _nodes.AddRange(shapes);

        // useless
        public override void PositionChange(Vector2 v2)
        {
        }
    }
}