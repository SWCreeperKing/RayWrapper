using System.Numerics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using static RayWrapper.VectorFont.LineUtil;

namespace RayWrapper.VectorFont;

public class VectorFontRegister
{
    public static float minThickness = 2.15f;

    public abstract record Drawable
    {
        public abstract void Draw(Vector2 position, float scale, Color color);
    }

    public record Line(Vector2 Pos1, Vector2 Pos2, float Thickness) : Drawable
    {
        public override void Draw(Vector2 position, float scale, Color color)
        {
            Vector2 Scale(Vector2 pos) => position + pos * scale;
            DrawLineEx(Scale(Pos1), Scale(Pos2), Math.Max(minThickness, scale * Thickness), color);
        }
    }

    public record QuadBezierLine(Vector2 Pos1, Vector2 Pos2, Vector2 ControlPos, float Thickness) : Drawable
    {
        public override void Draw(Vector2 position, float scale, Color color)
        {
            Vector2 Scale(Vector2 pos) => position + pos * scale;
            DrawBezQuadLineTriangle(Scale(Pos1), Scale(Pos2), Scale(ControlPos),
                Math.Max(minThickness, scale * Thickness), color);
        }
    }

    public record CubeBezierLine(Vector2 Pos1, Vector2 Pos2, Vector2 ControlPos1, Vector2 ControlPos2,
        float Thickness) : Drawable
    {
        public override void Draw(Vector2 position, float scale, Color color)
        {
            Vector2 Scale(Vector2 pos) => position + pos * scale;
            DrawBezCubicLineTriangle(Scale(Pos1), Scale(Pos2), Scale(ControlPos1), Scale(ControlPos2),
                Math.Max(minThickness, scale * Thickness), color);
        }
    }

    public record FilledRectangle(Vector2 Pos, Vector2 Size) : Drawable
    {
        public override void Draw(Vector2 position, float scale, Color color)
        {
            DrawRectangleV(position + Pos * scale, Size * scale, color);
        }
    }

    public record EmptyRectangle(Vector2 Pos, Vector2 Size, float Thickness) : Drawable
    {
        public override void Draw(Vector2 position, float scale, Color color)
        {
            DrawRectangleLinesEx(RectWrapper.AssembleRectFromVec(position + Pos * scale, Size * scale),
                Math.Max(minThickness, Thickness * scale), color);
        }
    }

    public record Circle(Vector2 Pos, float Radius) : Drawable
    {
        public override void Draw(Vector2 position, float scale, Color color)
        {
            DrawCircleV(position + Pos * scale, Radius * scale, color);
        }
    }

    public record Ring(Vector2 Pos, float Radius, float Thickness, float StartAngle, float EndAngle) : Drawable
    {
        public override void Draw(Vector2 position, float scale, Color color)
        {
            var thick = Math.Max(minThickness, Thickness * scale);
            var r = Radius * scale;
            DrawRing(position + Pos * scale, r - thick, r, StartAngle, EndAngle, 0, color);
        }
    }
}