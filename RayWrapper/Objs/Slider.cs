﻿using System;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Var_Interfaces;
using RayWrapper.Vars;
using static Raylib_CsLo.MouseCursor;
using static Raylib_CsLo.Raylib;
using static RayWrapper.GameBox;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs;

public class Slider : GameObject
{
    public static Style defaultStyle = new();

    public Style style = defaultStyle.Copy();
    public bool isVertical;
    public Action<float> onDone;
    public float value;

    public Slider(Rectangle rect) => (pos, size) = (rect.Pos(), rect.Size());

    public Slider(float x, float y, float width, float height) =>
        (pos, size) = (new Vector2(x, y), new Vector2(width, height));

    protected override void UpdateCall()
    {
        if (IsMouseOccupied && mouseOccupier != this) return;
        if (IsMouseButtonDown(MOUSE_LEFT_BUTTON) && AssembleRectFromVec(pos, size).IsMouseIn())
        {
            mouseOccupier = this;
        }
        else if (!IsMouseButtonDown(MOUSE_LEFT_BUTTON) && mouseOccupier == this)
        {
            mouseOccupier = null;
            onDone?.Invoke(value);
        }

        if (mouseOccupier != this) return;
        var mouse = mousePos;
        value = Math.Clamp(mouse.X - pos.X, 0, size.X) / size.X;
    }

    protected override void RenderCall()
    {
        var newS = new Vector2(size.X * (isVertical ? 1 : value), size.Y * (isVertical ? value : 1));
        var back = AssembleRectFromVec(pos, size);
        var rect = AssembleRectFromVec(pos, newS);
        style.Draw(back, rect, back.IsMouseIn() || mouseOccupier == this);
    }

    protected override Vector2 GetSize() => size + new Vector2(style.outlineStyle.thickness);

    protected override void UpdateSize(Vector2 newSize)
    {
        base.UpdateSize(newSize - new Vector2(style.outlineStyle.thickness));
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