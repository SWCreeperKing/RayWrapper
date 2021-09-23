using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Objs.TreeView.TreeNodeChain;
using RayWrapper.Vars;
using static Raylib_cs.MouseButton;
using static RayWrapper.GameBox;

namespace RayWrapper.Objs.TreeView
{
    public class TreeView : GameObject
    {
        public Rectangle mask = new(0, 0, 0, 0);
        public Vector2 axisOffset = Vector2.Zero;
        public List<NodeChain> chains = new();

        private Vector2 _lastPos;
        private bool _hold;
        private Vector2 _moveChange;
        private float _scale = 32;

        public TreeView(params NodeChain[] chains) : base(new Vector2()) => this.chains.AddRange(chains);

        public override void Update()
        {
            if (alertBox is not null || GeneralWrapper.MouseOccupied) return;
            if (Raylib.IsMouseButtonPressed(MOUSE_MIDDLE_BUTTON)) ResetPos();

            var rect = mask.IsEqualTo(new(0, 0, 0, 0))
                ? RectWrapper.AssembleRectFromVec(new Vector2(0), WindowSize)
                : mask;
            if (Raylib.IsMouseButtonDown(MOUSE_LEFT_BUTTON) && rect.IsMouseIn())
            {
                var curMouse = MousePos;
                if (!_hold) _lastPos = curMouse;
                _moveChange += (curMouse - _lastPos) / (_scale / 1.4f);
                (_lastPos, _hold) = (curMouse, true);
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
            rect.MaskDraw(() =>
            {
                foreach (var s in chains.Select(nc => nc.Draw((_moveChange + axisOffset) * _scale, _scale))
                    .Where(s => s != "")) t = s;
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

        public override Vector2 Size() => mask.IsEqualTo(new(0, 0, 0, 0)) ? WindowSize : mask.Size();
    }
}