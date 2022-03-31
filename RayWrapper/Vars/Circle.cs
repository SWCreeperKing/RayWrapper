using System.Numerics;
using Raylib_CsLo;

namespace RayWrapper.Vars;

public struct Circle
{
    public static readonly Circle Zero = new(Vector2.Zero, 0);

    public ColorModule color = Raylib.RED;
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

    public bool CheckCollision(Rectangle rect) => Raylib.CheckCollisionCircleRec(position, radius, rect);

    public void Draw()
    {
        Raylib.DrawCircle((int) position.X, (int) position.Y, radius, color);
    }
}