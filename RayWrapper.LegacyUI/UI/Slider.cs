using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.GameObject;
using RayWrapper.Base.Primitives;
using static Raylib_CsLo.MouseCursor;
using static Raylib_CsLo.Raylib;
using static RayWrapper.Base.GameBox.Input;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.LegacyUI.UI;

public class Slider : GameObject
{
    public static Style defaultStyle = new();

    public Style style = defaultStyle.Copy();
    public bool isVertical;
    public Action<float> onDone;
    public float value;

    private Rectangle _rect;
    private Rectangle _rect2;

    public Slider(Vector2 pos, Vector2 size) : this(new Rectangle(pos, size))
    {
    }

    public Slider(float x, float y, float width, float height) : this(new Rectangle(x, y, width, height))
    {
    }

    public Slider(Rectangle rect)
    {
        _rect = new Rectangle(rect);
        _rect2 = new Rectangle(rect);
    }

    protected override void UpdateCall(float dt)
    {
        if (IsMouseOccupied && MouseOccupier != this) return;
        if (IsMouseButtonDown(MOUSE_LEFT_BUTTON) && _rect.IsMouseIn()) MouseOccupier = this;
        else if (!IsMouseButtonDown(MOUSE_LEFT_BUTTON) && MouseOccupier == this)
        {
            MouseOccupier = null;
            onDone?.Invoke(value);
        }

        if (MouseOccupier != this) return;
        value = Math.Clamp(MousePosition.currentPosition.X - Position.X, 0, Size.X) / Size.X;
    }

    protected override void RenderCall()
    {
        _rect2.Size = new Vector2(Size.X * (isVertical ? 1 : value), Size.Y * (isVertical ? value : 1));
        style.Draw(_rect, _rect2, _rect.IsMouseIn() || MouseOccupier == this);
    }

    protected override Vector2 GetPosition() => _rect.Pos;
    protected override Vector2 GetSize() => _rect.Size + new Vector2(style.outlineStyle.thickness);
    protected override void UpdatePosition(Vector2 newPos) => _rect.Pos = _rect2.Pos = newPos;

    protected override void UpdateSize(Vector2 newSize)
    {
        _rect.Size = newSize - new Vector2(style.outlineStyle.thickness);
    }

    public override MouseCursor GetOccupiedCursor() => isVertical ? MOUSE_CURSOR_RESIZE_NS : MOUSE_CURSOR_RESIZE_EW;

    public class Style : IStyle<Style>
    {
        public RectStyle backStyle = new() { color = BLACK };
        public RectStyle fillStyle = new() { color = RAYWHITE };
        public OutlineStyle outlineStyle = new() { color = new ColorModule(150) };

        public void Draw(Rectangle back, Rectangle rect, bool hover)
        {
            backStyle.Draw(back);
            fillStyle.Draw(rect);
            if (hover) outlineStyle.Draw(back.Grow(5));
        }

        public Style Copy()
        {
            return new Style
            {
                backStyle = backStyle.Copy(), fillStyle = fillStyle.Copy(), outlineStyle = outlineStyle.Copy()
            };
        }
    }
}