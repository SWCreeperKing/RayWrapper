using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_cs;
using RayWrapper.TreeViewShapes;
using RayWrapper.Vars;
using static Raylib_cs.MouseButton;

namespace RayWrapper.Objs
{
    public class TreeView : GameObject
    {
        public Rectangle mask = new(0, 0, 0, 0);
        public Vector2 axisOffset = Vector2.Zero;
        public TreeViewControl tvc;

        private Vector2 _lastPos;
        private bool _hold;
        private Vector2 _moveChange;
        private TreeViewShape[] _nodes;
        private float _scale = 32;

        public TreeView(TreeViewControl tvc) : base(new Vector2())
        {
            this.tvc = tvc;
            var allNodes = tvc.GetNodes();
            var lines = allNodes.Where(n => n.GetType() == typeof(Line)).Select(l => (Line)l).ToArray();
            _nodes = allNodes.Except(lines).ToArray();
            foreach (var l in lines) l.Init(this);
            _nodes = _nodes.Concat(lines.Select(l => (TreeViewShape)l)).ToArray().OrderBy(n => n.GetDrawOrder())
                .ToArray();
        }

        public override void Update()
        {
            if (GameBox.alertBox is not null || GeneralWrapper.MouseOccupied) return;
            if (Raylib.IsMouseButtonPressed(MOUSE_RIGHT_BUTTON)) ResetPos();
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
            var idRaw = _nodes.Select(n =>
            {
                var (s, r, t) = ("", new Rectangle(), "");
                rect.MaskDraw(() => (s, r, t) = n.Draw(_moveChange + axisOffset, _scale, tvc, this));
                return (s, r, t, n);
            }).ToList();
            foreach (var (_, r, t, _) in idRaw.Where(s => s.t != "")) r.DrawTooltip(t);
            idRaw.RemoveAll(s => s.s == "");
            if (idRaw.Count < 1) return;
            var node = idRaw.Select(s => s.n).First();
            if (!GeneralWrapper.MouseOccupied) tvc.Click(node.id, node.carry);
        }

        public bool HasNode(string id) => _nodes.Any(n => n.id == id);

        public TreeViewShape GetNodeWithId(string id)
        {
            if (HasNode(id)) return _nodes.Where(n => n.id == id).First();
            var before = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"DEV ERROR: NODE `{id}` DOES NOT EXIST");
            Console.ForegroundColor = before;
            return new Box(id);
        }

        public void ResetPos()
        {
            _moveChange = new();
            _scale = 32;
        }

        // useless
        public override void PositionChange(Vector2 v2)
        {
        }
    }
}