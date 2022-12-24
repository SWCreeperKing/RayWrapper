using System.Numerics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace RayWrapper.Base.Primitives;

public struct Circle
{
    public static readonly Circle Zero = new(Vector2.Zero, 0);

    public Vector2 Position { get; set; }
    public float Radius { get; set; }

    public Circle(Vector2 position, float radius)
    {
        this.Position = position;
        this.Radius = radius;
    }

    public bool IsV2In(Vector2 v2) => CheckCollisionPointCircle(v2, Position, Radius);
    public bool IsRectIn(Raylib_CsLo.Rectangle rect) => CheckCollisionCircleRec(Position, Radius, rect);
    public bool IsCircleIn(Circle circle) => CheckCollisionCircles(Position, Radius, circle.Position, circle.Radius);
    public void Draw(Color color) => DrawCircle((int) Position.X, (int) Position.Y, Radius, color);
    public void DrawHollow(Color color) => DrawCircleLines((int) Position.X, (int) Position.Y, Radius, color);

    public void DrawWithSides(Color color, int sides = 5, float rotation = 0)
    {
        DrawPoly(Position, sides, Radius, rotation, color);
    }
}