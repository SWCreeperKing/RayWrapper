using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Objs.TreeView.TreeNodeChain;
using RayWrapper.Vars;
using static Raylib_CsLo.MouseButton;
using static Raylib_CsLo.Raylib;
using static RayWrapper.GameBox;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs.TreeView
{
    public class TreeView : GameObject
    {
        public override Vector2 Position { get; set; }
        public override Vector2 Size => mask.IsEqualTo(Zero) ? WindowSize : mask.Size();

        public Rectangle mask = Zero;
        public Vector2 axisOffset = Vector2.Zero;
        public readonly List<NodeChain> chains = new();

        private Vector2 _lastPos;
        private Vector2 _moveChange;
        private float _scale = 32;

        public TreeView(params NodeChain[] chains) => this.chains.AddRange(chains);

        protected override void UpdateCall()
        {
            if (alertBox is not null || IsMouseOccupied && mouseOccupier != this) return;
            if (IsMouseButtonPressed(MOUSE_MIDDLE_BUTTON)) ResetPos();
            if (IsMouseButtonDown(MOUSE_LEFT_BUTTON) && (mask.IsEqualTo(Zero)
                ? AssembleRectFromVec(Vector2.Zero, WindowSize)
                : mask).IsMouseIn())
            {
                var curMouse = mousePos;
                if (mouseOccupier != this) _lastPos = curMouse;
                _moveChange += (curMouse - _lastPos) / (_scale / 1.4f);
                (_lastPos, mouseOccupier) = (curMouse, this);
            }
            else mouseOccupier = null;

            var scroll = GetMouseWheelMove();
            if (scroll != 0) _scale = Math.Min(Math.Max(15, _scale + scroll), 55);
        }

        protected override void RenderCall()
        {
            if (!chains.Any()) return;
            (mask.IsEqualTo(Zero)
                ? AssembleRectFromVec(Vector2.Zero, WindowSize)
                : mask).MaskDraw(() =>
            {
                foreach (var nodeChain in chains) nodeChain.Draw((_moveChange + axisOffset) * _scale, _scale);
            });
        }

        public void ResetPos() => (_moveChange, _scale) = (Vector2.Zero, _scale = 32);
    }
}