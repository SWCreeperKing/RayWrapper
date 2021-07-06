using System;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace RayWrapper
{
    public static class RectWrapper
    {
        public static Rectangle AssembleRectFromVec(Vector2 pos, Vector2 size) => new(pos.X, pos.Y, size.X, size.Y);

        public static bool isV2In(this Rectangle rect, Vector2 v2) => CheckCollisionPointRec(v2, rect);
        public static bool IsMouseIn(this Rectangle rect) => rect.isV2In(GetMousePosition());
        public static void Draw(this Rectangle rect, Color color) => DrawRectangleRec(rect, color);
        public static Rectangle Grow(this Rectangle rect, int changeBuy) => rect.Shrink(-changeBuy);
        public static Rectangle AdjustWh(this Rectangle rect, Vector2 v2) => new(rect.x, rect.y, v2.X, v2.Y);
        public static Rectangle MoveTo(this Rectangle rect, Vector2 v2) => AssembleRectFromVec(v2, rect.Size());
        public static Rectangle Clone(this Rectangle rect) => new(rect.x, rect.y, rect.width, rect.height);
        public static bool IsColliding(this Rectangle rect1, Rectangle rect2) => CheckCollisionRecs(rect1, rect2);
        public static Vector2 Center(this Rectangle rect) => new(rect.x + rect.width / 2, rect.y + rect.height / 2);
        public static Vector2 Pos(this Rectangle rect) => new(rect.x, rect.y);
        public static Vector2 Size(this Rectangle rect) => new(rect.width, rect.height);
        public static string GetString(this Rectangle rect) => $"[({rect.x},{rect.y})({rect.width}x{rect.height})]";
        public static bool IsEqualTo(this Rectangle rect1, Rectangle rect2) => rect1.GetString() == rect2.GetString();
        public static void DrawCircle(this Rectangle rect, Color color) => DrawRectangleRounded(rect, 1f, 5, color);

        public static void DrawHallowCircle(this Rectangle rect, Color color, int thickness = 3) =>
            DrawRectangleRoundedLines(rect, 1f, 5, thickness, color);
        
        public static Rectangle Shrink(this Rectangle rect, int changeBuy) =>
            new(rect.x + changeBuy, rect.y + changeBuy, rect.width - changeBuy * 2, rect.height - changeBuy * 2);

        public static Rectangle AdjustByWh(this Rectangle rect, Vector2 v2) =>
            new(rect.x, rect.y, rect.width + v2.X, rect.height + v2.Y);

        public static Rectangle MoveBy(this Rectangle rect, Vector2 v2) =>
            AssembleRectFromVec(rect.Pos() + v2, rect.Size());

        public static Rectangle ExtendPos(this Rectangle rect, Vector2 v2) =>
            new(rect.x - v2.X, rect.y - v2.Y, rect.width + v2.X, rect.height + v2.Y);

        public static void DrawHallowRect(this Rectangle rect, Color color, int thickness = 3) =>
            DrawRectangleLinesEx(rect, thickness, color);

        public static void DrawTooltip(this Rectangle box, string text, int fontSize = 24, float spacing = 1.5f) =>
            box.DrawTooltip(text, new Color(170, 170, 255, 220), fontSize, spacing);
        
        public static void MaskDraw(this Rectangle rect, Action draw)
        {
            BeginScissorMode((int) rect.x, (int) rect.y, (int) rect.width, (int) rect.height);
            draw.Invoke();
            EndScissorMode();
        }

        public static void DrawTooltip(this Rectangle box, string text, Color color, int fontSize = 24,
            float spacing = 1.5f)
        {
            if (box.IsMouseIn()) GetMousePosition().DrawToolTipAtPoint(text, color, fontSize, spacing);
        }
    }
}