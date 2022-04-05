using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Objs.TreeView.TreeNodeChain;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;
using static RayWrapper.GameBox;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs.TreeView
{
    // todo: VVVVVV 
    // - tree view: image node
    public class TreeView : GameObject
    {
        public readonly List<NodeChain> chains = new();

        public override Vector2 Position { get; set; }
        public override Vector2 Size => mask.IsEqualTo(Zero) ? WindowSize : mask.Size();

        public bool enableScaling = true;
        public bool verticalMovement = true;
        public bool horizontalMovement = true;
        public float defaultScale = 32;
        public float distanceThreshold = .15f;
        public string selected;
        public Rectangle bounds = Max;
        public Rectangle mask = Zero;
        public Vector2 axisOffset = Vector2.Zero;
        public Tooltip tooltip;

        private Vector2 _lastPos;
        private Vector2 _moveChange;
        private float _scale;
        private List<string> _tooltipList = new();

        public TreeView(params NodeChain[] chains)
        {
            _scale = defaultScale;
            this.chains.AddRange(chains);
            tooltip = new DefaultTooltip(new Actionable<string>(() => selected));
        }

        protected override void UpdateCall()
        {
            if (alertQueue.Count > 0 || IsMouseOccupied && mouseOccupier != this) return;
            if (IsMouseButtonPressed(MOUSE_MIDDLE_BUTTON)) ResetPos();
            if (IsMouseButtonDown(MOUSE_LEFT_BUTTON) && (mask.IsEqualTo(Zero)
                    ? AssembleRectFromVec(Vector2.Zero, WindowSize)
                    : mask).IsMouseIn())
            {
                var curMouse = mousePos;
                if (mouseOccupier != this) _lastPos = curMouse;
                var toMove = (curMouse - _lastPos) / (_scale / 1.4f);

                if (Vector2.Zero.Distance(toMove) <= distanceThreshold)
                {
                    (_lastPos, mouseOccupier) = (curMouse, this);
                    return;
                }

                _moveChange += new Vector2(horizontalMovement ? toMove.X : 0, verticalMovement ? toMove.Y : 0);
                _moveChange = new Vector2(Math.Clamp(_moveChange.X, bounds.x, bounds.width),
                    Math.Clamp(_moveChange.Y, bounds.y, bounds.height));
                (_lastPos, mouseOccupier) = (curMouse, this);
            }
            else mouseOccupier = null;

            if (!enableScaling) return;
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
                _tooltipList.AddRange(chains
                    .Select(nodeChain => nodeChain.Draw((_moveChange + axisOffset) * _scale, _scale)));
            });

            var remain = _tooltipList.Where(i => i is not null).ToArray();
            _tooltipList.Clear();
            if (!remain.Any()) return;
            selected = remain.First();
            tooltip.Draw();
        }

        public void ResetPos() => (_moveChange, _scale) = (Vector2.Zero, _scale = defaultScale);
    }
}