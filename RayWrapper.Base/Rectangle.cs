using System.Numerics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using Rect = Raylib_CsLo.Rectangle;

namespace RayWrapper.Base;

public class Rectangle
{
    public static readonly Rect Zero = new(0, 0, 0, 0);
    public static readonly Rect Max = new(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue);

    public Rect Rect
    {
        get => _rect;
        set
        {
            _rect = value;
            _pos = _pos with { X = _rect.x, Y = _rect.y };
            _size = _size with { X = _rect.width, Y = _rect.height };
        }
    }

    public Vector2 Pos
    {
        get => _pos;
        set
        {
            _rect = _rect with { X = value.X, Y = value.Y };
            _pos = value;
        }
    }

    public Vector2 Size
    {
        get => _size;
        set
        {
            _rect = _rect with { width = value.X, height = value.Y };
            _size = value;
        }
    }

    public float X
    {
        get => _rect.x;
        set => _rect.x = _pos.X = value;
    }

    public float Y
    {
        get => _rect.y;
        set => _rect.y = _pos.Y = value;
    }

    public float W
    {
        get => _rect.width;
        set => _rect.width = _size.X = value;
    }

    public float H
    {
        get => _rect.height;
        set => _rect.height = _size.Y = value;
    }

    private Rect _rect;
    private Vector2 _pos;
    private Vector2 _size;

    public Rectangle(Rect rect) => Rect = rect;
    public Rectangle(Vector2 pos, Vector2 size) => Rect = new Rect(pos.X, pos.Y, size.X, size.Y);
    public Rectangle(float x = 0, float y = 0, float w = 0, float h = 0) => Rect = new Rect(x, y, w, h);

    public void MaskDraw(Action draw) => Pos.MaskDraw(Size, draw);
    public bool IsV2In(Vector2 v2) => CheckCollisionPointRec(v2, Rect);
    public bool IsRectIn(Rect rect) => CheckCollisionRecs(Rect, rect);
    public bool IsCircleIn(Circle c) => c.IsRectIn(Rect);
    public Vector2 Center() => Pos + Size / 2f;
    public Rectangle Clone() => new(Rect);

    /// <summary>shrinks all 4 sides by amount</summary>
    public Rectangle ShrinkThis(float amount)
    {
        X += amount;
        Y += amount;
        W -= amount * 2;
        H -= amount * 2;
        return this;
    }

    /// <summary>Returns new rectangle with all 4 sides shrunk by amount</summary>
    public Rectangle Shrink(float amount) => new(X + amount, Y + amount, W - amount * 2, H - amount * 2);

    /// <summary>Grows all 4 sides by amount</summary>
    public Rectangle GrowThis(float amount)
    {
        X -= amount;
        Y -= amount;
        W += amount * 2;
        H += amount * 2;
        return this;
    }

    /// <summary>Returns new rectangle with all 4 sides grown by amount</summary>
    public Rectangle Grow(float amount) => new(X - amount, Y - amount, W + amount * 2, H + amount * 2);

    /// <summary>Changes the position while keeping the position of the size the same</summary>
    public void ExtendPos(float amount) => ExtendPos(amount, amount);

    /// <summary>Changes the position while keeping the position of the size the same</summary>
    public void ExtendPos(Vector2 amount) => ExtendPos(amount.X, amount.Y);

    /// <summary>Changes the position while keeping the position of the size the same</summary>
    public void ExtendPos(float xAmount, float yAmount)
    {
        X -= xAmount;
        Y -= yAmount;
        W += xAmount;
        H += yAmount;
    }

    public void Deconstruct(out float x, out float y, out float w, out float h)
    {
        x = X;
        y = Y;
        w = W;
        h = H;
    }

    public void Draw(Color color) => DrawRectangleRec(Rect, color);

    public void DrawPro(Color color, Vector2? origin = null, float rotation = 0f)
    {
        DrawRectanglePro(Rect, origin ?? Vector2.Zero, rotation, color);
    }

    public void DrawLines(Color color, float thickness = 3f)
    {
        DrawRectangleLinesEx(Rect, thickness, color);
    }

    public void DrawRounded(Color color, int segments = 10, float roundness = 1f)
    {
        DrawRectangleRounded(Rect, roundness, segments, color);
    }

    public void DrawRoundedLines(Color color, int segments = 10, float roundness = 1f, float thickness = 3f)
    {
        DrawRectangleRoundedLines(Rect, roundness, segments, thickness, color);
    }

    public void DrawGradient(Color c1, Color c2, Color c3, Color c4)
    {
        DrawRectangleGradientEx(Rect, c1, c2, c3, c4);
    }

    public void DrawGradient(Color c1, Color c2, bool isVertical = false)
    {
        if (isVertical) DrawGradientV(c1, c2);
        else DrawGradientH(c1, c2);
    }

    public void DrawGradientV(Color c1, Color c2)
    {
        DrawRectangleGradientV((int) X, (int) Y, (int) W, (int) H, c1, c2);
    }

    public void DrawGradientH(Color c1, Color c2)
    {
        DrawRectangleGradientH((int) X, (int) Y, (int) W, (int) H, c1, c2);
    }


    public static bool operator ==(Rectangle r1, Rect r2) =>
        r1.X == r2.x && r1.Y == r2.y && r1.W == r2.width && r1.H == r2.height;

    public static bool operator !=(Rectangle r1, Rect r2) => !(r1 == r2);

    public static explicit operator Rectangle(Rect rect) => new(rect);
    public static implicit operator Rect(Rectangle rect) => rect.Rect;

    public override string ToString() => $"[({X},{Y})({W}x{H})]";
}