using System.Numerics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace RayWrapper.Base;

public class Triangle
{
    public readonly Vector2[] points = { Vector2.Zero, Vector2.Zero, Vector2.Zero };

    public Triangle(Vector2 point1, Vector2 point2, Vector2 point3)
    {
        points[0] = point1;
        points[1] = point2;
        points[2] = point3;
    }

    public Triangle(params Vector2[] points)
    {
        if (this.points.Length != 3)
        {
            throw new ArgumentException($"Triangle requires 3 points, not [{points.Length}] points");
        }

        for (var i = 0; i < 3; i++) this.points[i] = points[i];
    }

    public void AddToPoints(Vector2 v2)
    {
        for (var i = 0; i < points.Length; i++) points[i] += v2;
    }

    public Vector2 Center() => points.Center();
    public bool IsPointIn(Vector2 v2) => CheckCollisionPointTriangle(v2, points[0], points[1], points[2]);
    public void Draw(Color? color = null) => DrawTriangle(points[0], points[1], points[2], color ?? WHITE);
    public void DrawLines(Color? color = null) => DrawTriangleLines(points[0], points[1], points[2], color ?? WHITE);
}