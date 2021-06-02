using System;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace RayWrapper
{
    public static class GeneralWrapper
    {
        public static readonly Color Transparent = new(0, 0, 0, 0);
        public static bool MouseOccupied { get; set; }

        public static Vector2 Max(this Vector2 v2) => new(Math.Max(v2.X, v2.Y));
        public static Vector2 Max(this Vector2 v2, Vector2 v22) => new(Math.Max(v2.X, v22.X), Math.Max(v2.Y, v22.Y));

        public static void DrawCenterText(this Font font, Vector2 pos, string text, Color color, float fontSize = 24,
            float spacing = 1.5f) =>
            DrawTextEx(font, text, pos - MeasureTextEx(font, text, fontSize, spacing) / 2, fontSize, spacing, color);

        public static void DrawTextWrap(this Font font, string text, Rectangle rect, Color fontColor, int fontSize = 24,
            float spacing = 1.5f) =>
            DrawTextRec(font, text, rect, fontSize, spacing, true, fontColor);

        public static Vector2 MeasureText(this Font font, string text, float fontSize = 24f, float spacing = 1.5f) =>
            MeasureTextEx(font, text, fontSize, spacing);

        /// <summary>
        /// TopRight: 1
        /// TopLeft: 2
        /// BottomRight: 3
        /// BottomLeft: 4
        /// </summary>
        public static int GetCursorQuadrant()
        {
            var lines = GameBox.WindowSize / 2;
            var cursor = GetMousePosition();
            var quad = cursor.X > lines.X ? 1 : 2;
            if (cursor.Y > lines.Y) quad += 2;
            return quad;
        }

        public static double MinMax(this double num, double max, double min) => Math.Max(Math.Min(num, max), min);
        public static float MinMax(this float num, float max, float min) => Math.Max(Math.Min(num, max), min);
    }
}