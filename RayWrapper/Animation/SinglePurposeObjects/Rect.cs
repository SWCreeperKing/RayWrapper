﻿using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;

namespace RayWrapper.Animation.SinglePurposeObjects;

/// <summary>
/// Mostly just use for transitions :p
/// </summary>
public class Rect : GameObject, ISizeable, IAlphable
{
    public ColorModule ColorMod
    {
        get => ((Color) _color).SetAlpha(alpha);
        set => (_color, alpha) = (value, ((Color) value).a);
    }

    public ColorModule OutlineMod
    {
        get => ((Color) _outlineColor).SetAlpha(alpha);
        set => (_outlineColor, alpha) = (value, ((Color) value).a);
    }

    public int alpha = 255;
    public bool outline;
    public int outlineThickness = 3;
    public bool isRound;
    public float roundness = .5f;
    public Rectangle rect;

    private ColorModule _color = WHITE;
    private ColorModule _outlineColor = BLACK;

    public Rect(Rectangle rect) => this.rect = rect;

    protected override void RenderCall()
    {
        var rect = GetRect();
        if (isRound)
        {
            rect.DrawRounded(ColorMod, roundness);
            if (outline) rect.DrawRoundedLines(OutlineMod, roundness, outlineThickness);
        }
        else
        {
            rect.Draw(ColorMod);
            if (outline) rect.DrawHallowRect(OutlineMod, outlineThickness);
        }
    }

    protected override Vector2 GetPosition()
    {
        return base.GetPosition();
    }

    protected override Vector2 GetSize()
    {
        return base.GetSize();
    }

    protected override void UpdatePosition(Vector2 newPos)
    {
        base.UpdatePosition(newPos);
    }

    protected override void UpdatedSize(Vector2 newSize) => rect.SetSize(newSize);

    public void SetSize(Vector2 size) => this.size = size;
    public void AddSize(Vector2 size) => this.size += size;
    public int GetAlpha() => alpha;
    public void SetAlpha(int alpha) => this.alpha = alpha;
    
    public static implicit operator Rectangle(Rect rect) => rect.GetRect();
    public static implicit operator Rect(Rectangle rect) => new(rect);
}