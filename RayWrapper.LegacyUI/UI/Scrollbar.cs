using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.GameBox;
using RayWrapper.Base.GameObject;
using RayWrapper.Base.Primitives;
using ZimonIsHimUtils.ExtensionMethods;
using static Raylib_CsLo.MouseCursor;
using static Raylib_CsLo.Raylib;
using static RayWrapper.Base.GameBox.Input;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.LegacyUI.UI;

public class Scrollbar : GameObject
{
    public static Style defaultStyle = new();

    public Style style = defaultStyle.Copy();
    public Rectangle bar = new();
    public int minSizePercent = 20;
    public bool isVertical;
    public Func<float> amountInvoke;

    private readonly IList<Action<float>> _onMove = new List<Action<float>>();
    private float _trueSize;
    private float _visibleSize;
    private Vector2 _lastMouse = Vector2.Zero;

    public Scrollbar(Rectangle rect, bool isVertical = true)
    {
        (pos, size) = (rect.Pos, rect.Size);
        this.isVertical = isVertical;
    }

    public float Value { get; private set; }

    public float GetOffset => isVertical ? pos.Y - bar.Y : pos.X - bar.X;

    public event Action<float> OnMoveEvent
    {
        add => _onMove.Add(value);
        remove => _onMove.Remove(value);
    }

    public void MoveBar(float offset)
    {
        CalcBarSize();
        if (isVertical) bar.Y -= offset;
        else bar.X -= offset;
        ClampBounds();
        _onMove.Each(a => a(Value));
    }

    public void ClampBounds()
    {
        CalcBarSize();
        (bar.W, bar.H) = isVertical ? (size.X, _visibleSize) : (_visibleSize, size.Y);
        var (hMax, wMax) = (pos.Y + size.Y, pos.X + size.X);
        bar.Y = Math.Clamp(bar.Y, pos.Y, (int) (isVertical ? hMax - _visibleSize : hMax));
        bar.X = Math.Clamp(bar.X, pos.X, (int) (isVertical ? wMax : wMax - _visibleSize));
    }

    public void CalcVal()
    {
        var sub1 = isVertical ? bar.Y - pos.Y : bar.X - pos.X;
        var sub2 = Math.Max((isVertical ? size.Y : size.X) - _visibleSize, 1);
        Value = Math.Clamp(sub1 / sub2 * (Amount() - 1), 0, float.MaxValue - 1);
    }

    public void CalcBarSize()
    {
        var rSize = isVertical ? size.Y : size.X;
        _trueSize = rSize / Amount();
        _visibleSize = Math.Max(_trueSize, rSize * (minSizePercent / 100f));
    }

    protected override void UpdateCall(float dt)
    {
        var mousePos = MousePosition.currentPosition;
        CalcBarSize();
        ClampBounds();
        CalcVal();

        var isLeft = IsMouseButtonDown(MOUSE_LEFT_BUTTON);

        if (bar.IsMouseIn() && isLeft && !IsMouseOccupied)
        {
            _lastMouse = mousePos;
            MouseOccupier = this;
        }
        else if (MouseOccupier == this && !isLeft) MouseOccupier = null;

        if (MouseOccupier != this)
        {
            if (!IsMouseOccupied && IsMouseButtonPressed(MOUSE_LEFT_BUTTON) && GetRect().IsMouseIn())
                MoveBar((isVertical ? bar.Y - mousePos.Y : bar.X - mousePos.X) + _visibleSize / 2);

            _lastMouse = Vector2.Zero;
            return;
        }

        MoveBar(isVertical ? _lastMouse.Y - mousePos.Y : _lastMouse.X - mousePos.X);
        _lastMouse = mousePos;
    }

    protected override void RenderCall()
    {
        if (Amount() == 1) return; // ignore loss of precision
        var container = GetRect();
        if (container.IsMouseIn()) GameBox.SetMouseCursor(MOUSE_CURSOR_POINTING_HAND);
        style.Draw(container, bar,
            IsMouseOccupied && MouseOccupier == this || !IsMouseOccupied && container.IsMouseIn());
    }

    protected override void UpdatePosition(Vector2 newPos)
    {
        var offset = GetOffset;
        bar.Pos = newPos;
        MoveBar(offset);
        base.UpdatePosition(newPos);
    }

    public float Amount() => Math.Max(amountInvoke?.Invoke() ?? 0, 1);
    public override MouseCursor GetOccupiedCursor() => isVertical ? MOUSE_CURSOR_RESIZE_NS : MOUSE_CURSOR_RESIZE_EW;

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