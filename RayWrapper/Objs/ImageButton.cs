using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base;
using RayWrapper.Base.GameObject;
using RayWrapper.Var_Interfaces;
using RayWrapper.Vars;
using ZimonIsHimUtils.ExtensionMethods;
using Rectangle = RayWrapper.Base.Rectangle;

namespace RayWrapper.Objs;

public class ImageButton : GameObject
{
    public static Style defaultStyle = new();

    public Style style = defaultStyle.Copy();
    public ImageObj image;
    public Rectangle? sizeOverride;
    public Actionable<bool> isDisabled = false;
    public Tooltip tooltip;

    private readonly IList<Action> _clickEvent = new List<Action>();

    /// <summary>
    ///     this event will invoke all subscribers on button click
    /// </summary>
    public event Action Clicked
    {
        add => _clickEvent.Add(value);
        remove => _clickEvent.Remove(value);
    }

    public ImageButton(ImageObj image, Vector2 position)
    {
        this.image = image;
        this.image.Position = position;
        this.image.ImageAlpha = 170;
    }

    public ImageButton(ImageObj image, Rectangle sizeOverride)
    {
        this.image = image;
        this.sizeOverride = sizeOverride;
        this.image.ImageAlpha = 170;
    }

    protected override void UpdateCall()
    {
        image.Update();
        var rect = sizeOverride ?? image.GetRect();
        var over = rect.Grow(style.imageMargin);

        if (!over.IsMouseIn()) return;
        tooltip?.Draw(over);
        if (!Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) return;
        Click();
    }

    protected override void RenderCall()
    {
        var rect = sizeOverride ?? image.GetRect();
        var over = rect.Grow(style.imageMargin);

        style.Draw(over, isDisabled);
        if (sizeOverride is null) image.Render();
        else image.RenderTo(sizeOverride);

        if (!over.IsMouseIn()) return;
        if (_clickEvent.Any() && !isDisabled) GameBox.SetMouseCursor(MouseCursor.MOUSE_CURSOR_POINTING_HAND);
        else if (_clickEvent.Any()) GameBox.SetMouseCursor(MouseCursor.MOUSE_CURSOR_NOT_ALLOWED);
    }

    protected override Vector2 GetPosition() => image.Position;
    protected override Vector2 GetSize() => image.Size;
    protected override void UpdatePosition(Vector2 newPos) => image.Position = newPos;
    protected override void UpdateSize(Vector2 newSize) => image.Size = newSize;

    public void Click() => _clickEvent.Each(a => a.Invoke());

    public class Style : IStyle<Style>
    {
        public ColorModule BackColor
        {
            get => _backColor;
            set => _backColor = value;
        }

        public RectStyle backStyle = new();
        public OutlineStyle outlineStyle = new();
        public int imageMargin = 7;

        private ColorModule _backColor = new(1, 89, 99);

        public void Draw(Rectangle rect, bool isDisabled)
        {
            backStyle.color = isDisabled
                ? _backColor.ReturnDarker()
                : rect.IsMouseIn()
                    ? _backColor.ReturnLighter()
                    : _backColor;

            backStyle.Draw(rect);
            outlineStyle.Draw(rect);
        }

        public Style Copy()
        {
            return new Style
            {
                backStyle = backStyle.Copy(), outlineStyle = outlineStyle.Copy()
            };
        }
    }
}