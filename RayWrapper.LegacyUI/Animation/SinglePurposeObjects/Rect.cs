using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.Extras;
using RayWrapper.Base.GameObject;
using RayWrapper.Base.Primitives;
using static Raylib_CsLo.Raylib;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.LegacyUI.Animation.SinglePurposeObjects;

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
            rect.DrawRounded(ColorMod, 10, roundness);
            if (outline) rect.DrawRoundedLines(OutlineMod, 10, roundness, outlineThickness);
        }
        else
        {
            rect.Draw(ColorMod);
            if (outline) rect.DrawLines(OutlineMod, outlineThickness);
        }
    }

    protected override Vector2 GetPosition() => rect.Pos;
    protected override Vector2 GetSize() => rect.Size;
    protected override void UpdatePosition(Vector2 newPos) => rect.Pos = newPos;
    protected override void UpdateSize(Vector2 newSize) => rect.Size = newSize;

    public void SetSize(Vector2 size) => Size = size;
    public void AddSize(Vector2 size) => Size += size;
    public int GetAlpha() => alpha;
    public void SetAlpha(int alpha) => this.alpha = alpha;

    public static implicit operator Rectangle(Rect rect) => rect.GetRect();
    public static implicit operator Rect(Rectangle rect) => new(rect);
}