using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base;
using RayWrapper.Base.Gameobject;
using RayWrapper.Objs;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;
using Rectangle = Raylib_CsLo.Rectangle;

namespace RayWrapper.Animation.SinglePurposeObjects;

/// <summary>
/// Mostly just use for transitions :p
/// </summary>
public class MaskText : GameObject, ISizeable, IAlphable
{
    public static Text.Style defaultStyle = new();

    public ColorModule ColorMod
    {
        get => ((Color) _color).SetAlpha(alpha);
        set => (_color, alpha) = (value, ((Color) value).a);
    }

    public Text.Style style = defaultStyle.Copy();
    public int alpha = 255;
    public Actionable<string> text;
    public Tooltip tooltip;

    private ColorModule _color = WHITE;

    public MaskText(Rectangle rect) : this("", rect)
    {
    }

    public MaskText(Actionable<string> text, Rectangle rect)
    {
        this.text = text;
        (pos, size) = (rect.Pos(), rect.Size());
        
        style.color = new ColorModule(() =>
        {
            var color = (Color) ColorMod;
            return color.a != alpha ? color.SetAlpha(alpha) : color;
        });
    }

    protected override void RenderCall()
    {
        var rect = GetRect();
        rect.Grow(2).MaskDraw(() => style.Draw(text, rect));
        tooltip?.Draw(rect);
    }

    public void SetSize(Vector2 size) => Size = size;
    public void AddSize(Vector2 size) => Size += size;
    public int GetAlpha() => alpha;
    public void SetAlpha(int alpha) => this.alpha = alpha;
    
    public static implicit operator MaskText(Rect r) => new(r);
}