using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Objs.TreeView.Shapes;
using RayWrapper.Vars;
using static Raylib_cs.MouseButton;
using static RayWrapper.GameBox;

namespace RayWrapper.Objs.TreeView
{
    public class TreeView : GameObject
    {
        public Rectangle mask = new(0, 0, 0, 0);
        public Vector2 axisOffset = Vector2.Zero;

        private Vector2 _lastPos;
        private bool _hold;
        private Vector2 _moveChange;
        private Dictionary<string, TreeNode> _nodeDict = new();
        private List<Line> _lines = new();
        private float _scale = 32;

        public TreeView(params TreeNodeBase[] nodeBases) : base(new Vector2())
        {
            _lines.AddRange(nodeBases.Where(n => n.GetType() == typeof(Line)).Select(n => (Line)n));
            foreach (var node in nodeBases.Except(_lines).Select(n => (TreeNode)n)) _nodeDict.Add(node.name, node);
        }

        public override void Update()
        {
            if (alertBox is not null || GeneralWrapper.MouseOccupied) return;
            if (Raylib.IsMouseButtonPressed(MOUSE_RIGHT_BUTTON)) ResetPos();
            
            var rect = mask.IsEqualTo(new(0, 0, 0, 0))
                ? RectWrapper.AssembleRectFromVec(new Vector2(0), WindowSize)
                : mask;
            if (Raylib.IsMouseButtonDown(MOUSE_LEFT_BUTTON) && rect.IsMouseIn())
            {
                var curMouse = MousePos;
                if (!_hold) _lastPos = curMouse;
                _moveChange += (curMouse - _lastPos) / (_scale / 1.4f);
                _lastPos = curMouse;
                _hold = true;
            }
            else _hold = false;

            var scroll = Raylib.GetMouseWheelMove();
            if (scroll != 0) _scale = Math.Min(Math.Max(15, _scale + scroll), 55);
        }

        protected override void RenderCall()
        {
            var rect = mask.IsEqualTo(new(0, 0, 0, 0))
                ? RectWrapper.AssembleRectFromVec(new Vector2(0), WindowSize)
                : mask;

            var t = "";
            foreach (var line in _lines) rect.MaskDraw(() => line.Draw(_nodeDict, _moveChange + axisOffset, _scale));
            foreach (var n in _nodeDict.Values)
                rect.MaskDraw(() =>
                {
                    var s = n.Draw(_nodeDict, _moveChange + axisOffset, _scale);
                    if (s != "") t = s;
                });
            if (t != "") rect.DrawTooltip(t);
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