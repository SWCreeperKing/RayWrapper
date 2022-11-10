using System.Numerics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace RayWrapper.Base.Primitives;

public struct Circle
{
    public static readonly Circle Zero = new(Vector2.Zero, 0);

    public Vector2 position;
    public float radius;

    public Circle(Vector2 position, float radius)
    {
        this.position = position;
        this.radius = radius;
    }

    public bool IsV2In(Vector2 v2) => CheckCollisionPointCircle(v2, position, radius);
    public bool IsRectIn(Raylib_CsLo.Rectangle rect) => CheckCollisionCircleRec(position, radius, rect);
    public bool IsCircleIn(Circle circle) => CheckCollisionCircles(position, radius, circle.position, circle.radius);
    public void Draw(Color color) => DrawCircle((int) position.X, (int) position.Y, radius, color);
    public void DrawHollow(Color color) => DrawCircleLines((int) position.X, (int) position.Y, radius, color);

    public void DrawWithSides(Color color, int sides = 5, float rotation = 0)
    {
        DrawPoly(position, sides, radius, rotation, color);
    }
}