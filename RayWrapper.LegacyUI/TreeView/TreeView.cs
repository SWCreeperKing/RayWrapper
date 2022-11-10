using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.Extras;
using RayWrapper.Base.GameObject;
using RayWrapper.Base.Primitives;
using RayWrapper.LegacyUI.TreeView.TreeNodeChain;
using RayWrapper.LegacyUI.UI;
using static Raylib_CsLo.MouseCursor;
using static Raylib_CsLo.Raylib;
using static RayWrapper.Base.GameBox.GameBox;
using static RayWrapper.Base.GameBox.Input;
using static RayWrapper.LegacyUI.AlertBoxes.AlertBase.AlertController;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.LegacyUI.TreeView;

public class TreeView : GameObject
{
    public readonly List<NodeChain> chains = new();

    public bool enableScaling = true;
    public bool verticalMovement = true;
    public bool horizontalMovement = true;
    public float defaultScale = 32;
    public float distanceThreshold = .12f;
    public string selected;
    public Rectangle bounds = new(Rectangle.Max);
    public Rectangle mask = new(Rectangle.Zero);
    public Vector2 axisOffset = Vector2.Zero;
    public Tooltip tooltip;

    private Vector2 _lastPos;
    private Vector2 _moveChange;
    private float _scale;
    private bool _isOverThreash;
    private List<string> _tooltipList = new();

    public TreeView(params NodeChain[] chains)
    {
        _scale = defaultScale;
        this.chains.AddRange(chains);
        tooltip = new DefaultTooltip(new Actionable<string>(() => selected));
    }

    protected override void UpdateCall(float dt)
    {
        _isOverThreash = false;
        if (alertQueue.Count > 0 || IsMouseOccupied && MouseOccupier != this) return;
        if (IsMouseButtonPressed(MOUSE_MIDDLE_BUTTON)) ResetPos();
        if (IsMouseButtonDown(MOUSE_LEFT_BUTTON) && (mask == Rectangle.Zero
                ? new Rectangle(Vector2.Zero, WindowSize)
                : mask).IsMouseIn())
        {
            var curMouse = MousePosition.currentPosition;
            if (MouseOccupier != this) _lastPos = curMouse;
            var toMove = (curMouse - _lastPos) / (_scale / 1.4f);

            if (Vector2.Zero.Distance(toMove) <= distanceThreshold)
            {
                (_lastPos, MouseOccupier) = (curMouse, this);
                return;
            }

            _isOverThreash = true;
            _moveChange += new Vector2(horizontalMovement ? toMove.X : 0, verticalMovement ? toMove.Y : 0);
            _moveChange = new Vector2(Math.Clamp(_moveChange.X, bounds.X, bounds.W),
                Math.Clamp(_moveChange.Y, bounds.Y, bounds.H));
            (_lastPos, MouseOccupier) = (curMouse, this);
        }
        else MouseOccupier = null;

        if (!enableScaling) return;
        var scroll = GetMouseWheelMove();
        if (scroll != 0) _scale = Math.Min(Math.Max(15, _scale + scroll), 55);
    }

    protected override void RenderCall()
    {
        if (!chains.Any()) return;
        (mask == Rectangle.Zero
            ? new Rectangle(Vector2.Zero, WindowSize)
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

    public override MouseCursor GetOccupiedCursor()
    {
        return _isOverThreash ? MOUSE_CURSOR_RESIZE_ALL : MOUSE_CURSOR_ARROW;
    }
}