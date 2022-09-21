using System.Numerics;
using Raylib_CsLo;

namespace RayWrapper.Base;

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
    
    public bool CheckCollision(Circle circle)
    {
        return Raylib.CheckCollisionCircles(position, radius, circle.position, circle.radius);
    } 

    public bool CheckCollision(Raylib_CsLo.Rectangle rect) => Raylib.CheckCollisionCircleRec(position, radius, rect);

    public void Draw(Color color)
    {
        Raylib.DrawCircle((int) position.X, (int) position.Y, radius, color);
    }

    public void DrawHollow(Color color)
    {
        Raylib.DrawCircleLines((int) position.X, (int) position.Y, radius, color);
    }

    public void DrawWithSides(Color color, int sides = 5, float rotation = 0)
    {
        Raylib.DrawPoly(position, sides, radius, rotation, color);
    }
}