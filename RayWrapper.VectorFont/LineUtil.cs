using System.Numerics;
using Raylib_CsLo;

namespace RayWrapper.VectorFont;

public static class LineUtil
{
        public static void DrawBezQuadLineTriangle(Vector2 startPos, Vector2 endPos, Vector2 controlPos, float thickness,
        Color color, int segments = 24)
    {
        Vector2 Radius(Vector2 pos1, Vector2 pos2)
        {
            Vector2 delta = new(pos2.X - pos1.X, pos2.Y - pos1.Y);
            var length = Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
            var scale = thickness / (2 * Math.Max(1, length));
            return new((float) (-scale * delta.Y), (float) (scale * delta.X));
        }

        var step = 1f / segments;

        var previous = startPos;
        Vector2 current;
        var n = 0;
        var points = new Vector2[(segments + 1) * 2];

        for (var i = 1; i <= segments; i++)
        {
            var t = step * i;
            var a = MathF.Pow(1 - t, 2);
            var b = 2 * (1 - t) * t;
            var c = MathF.Pow(t, 2);

            current = a * startPos + b * controlPos + c * endPos;
            var radius = Radius(previous, current);

            if (i == 1) points[n++] = previous - radius;
            points[n++] = previous + radius;
            points[n++] = current - radius;
            if (i == segments) points[n++] = current + radius;

            previous = current;
        }

        unsafe
        {
            fixed (Vector2* pointsPoint = points)
                Raylib.DrawTriangleStrip(pointsPoint, points.Length, color);
        }
    }


    public static void DrawBezCubicLineTriangle(Vector2 startPos, Vector2 endPos, Vector2 startControlPos,
        Vector2 endControlPos, float thickness, Color color, int segments = 24)
    {
        Vector2 Radius(Vector2 pos1, Vector2 pos2)
        {
            Vector2 delta = new(pos2.X - pos1.X, pos2.Y - pos1.Y);
            var length = Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
            var scale = thickness / (2 * Math.Max(1, length));
            return new((float) (-scale * delta.Y), (float) (scale * delta.X));
        }

        var step = 1f / segments;

        var previous = startPos;
        Vector2 current;
        var n = 0;
        var points = new Vector2[(segments + 1) * 2];

        for (var i = 1; i <= segments; i++)
        {
            var t = step * i;
            var a = (float) Math.Pow(1 - t, 3);
            var b = (float) (3*Math.Pow(1 - t, 2)*t);
            var c = (float) (3*(1-t)*Math.Pow(t, 2));
            var d = (float) Math.Pow(t, 3);

            current = a * startPos + b * startControlPos + c * endControlPos + d * endPos;
            var radius = Radius(previous, current);

            if (i == 1) points[n++] = previous - radius;
            points[n++] = previous + radius;
            points[n++] = current - radius;
            if (i == segments) points[n++] = current + radius;

            previous = current;
        }

        unsafe
        {
            fixed (Vector2* pointsPoint = points)
                Raylib.DrawTriangleStrip(pointsPoint, points.Length, color);
        }
    }
}