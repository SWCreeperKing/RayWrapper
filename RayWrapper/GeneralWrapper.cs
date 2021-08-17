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

        public static void DrawCenterWrapText(this Font font, Rectangle rect, string text, Color color,
            float fontSize = 24,
            float spacing = 1.5f) =>
            DrawTextRec(font, text, rect.MoveBy(-(MeasureTextEx(font, text, fontSize, spacing) / 2)), fontSize, spacing,
                true, color);

        public static void DrawTextWrap(this Font font, string text, Rectangle rect, Color fontColor, int fontSize = 24,
            float spacing = 1.5f) =>
            DrawTextRec(font, text, rect, fontSize, spacing, true, fontColor);

        public static void DrawText(this Font font, string text, Vector2 pos, Color fontColor, int fontSize = 24,
            float spacing = 1.5f) =>
            DrawTextEx(font, text, pos, fontSize, spacing, fontColor);

        public static Vector2 MeasureText(this Font font, string text, float fontSize = 24f, float spacing = 1.5f) =>
            MeasureTextEx(font, text, fontSize, spacing);

        public static string GetString(this KeyboardKey key) =>
            $"{key}".Replace("KEY_MENU", "KEY_R").Replace("KEY_", "").Replace("_", " ").ToLower();

        /// <summary>
        /// Makes a lighter and darker version of a given color
        /// </summary>
        /// <param name="color">Color to make alt version for</param>
        /// <returns>(lighter, darker)</returns>
        public static (Color light, Color dark) MakeLightDark(this Color color) =>
            (new Color((int)Math.Min(color.r * 1.5, 255), (int)Math.Min(color.g * 1.5, 255),
                    (int)Math.Min(color.b * 1.5, 255), color.a),
                new Color((int)(color.r / 1.7), (int)(color.g / 1.7), (int)(color.b / 1.7), color.a));

        /// <summary>
        /// TopRight: 1
        /// TopLeft: 2
        /// BottomRight: 3
        /// BottomLeft: 4
        /// </summary>
        public static int GetCursorQuadrant()
        {
            var lines = GameBox.WindowSize / 2;
            var cursor = GameBox.MousePos;
            var quad = cursor.X > lines.X ? 1 : 2;
            if (cursor.Y > lines.Y) quad += 2;
            return quad;
        }

        public static void DrawToolTipAtPoint(this Vector2 rawPos, string text, Color color, float fontSize = 24,
            float spacing = 1.5f)
        {
            var textSize = GameBox.Font.MeasureText(text, fontSize, spacing);
            var quad = GetCursorQuadrant();
            Vector2 pos = new(rawPos.X - (quad % 2 != 0 ? textSize.X : 0), rawPos.Y - (quad > 2 ? textSize.Y : -33));
            RectWrapper.AssembleRectFromVec(pos, textSize).Grow(4).Draw(new Color(0, 0, 0, 200));
            DrawTextEx(GameBox.Font, text, pos, fontSize, spacing, color);
        }

        [Obsolete("Use an actual clamp i.e. Math.Clamp(), but I won't judge you if you REALLY WANT to use this")]
        public static double Clamp<T>(this T n, T min, T max) =>
            n switch
            {
                int dn when min is int dMin && max is int dMax => Math.Max(Math.Min(dn, dMin), dMax),
                long dn when min is long dMin && max is long dMax => Math.Max(Math.Min(dn, dMin), dMax),
                float dn when min is float dMin && max is float dMax => Math.Max(Math.Min(dn, dMin), dMax),
                double dn when min is double dMin && max is double dMax => Math.Max(Math.Min(dn, dMin), dMax),
                _ => 0
            };

        public static Color EditColor(this Color color, int r = 0, int g = 0, int b = 0, int a = 0) =>
            new(color.r + r, color.g + g, color.b + b, color.a + a);

        public static void DrawLine(this (Vector2 v1, Vector2 v2) l, Color color, float thickness = 3) =>
            DrawLineEx(l.v1, l.v2, thickness, color);

        public static void
            DrawLine(this (float x1, float y1, float x2, float y2) l, Color color, float thickness = 3) =>
            (new Vector2(l.x1, l.y1), new Vector2(l.x2, l.y2)).DrawLine(color, thickness);

        public static Vector2 Add(this Vector2 v2, float f) => v2 + new Vector2(f);

        public static Color Percent(this Color c1, Color c2, float percent)
        {
            int DoCalc(int c1, int c2) => (int)((1.0 - percent) * c1 + percent * c2 + 0.5);
            return new Color(DoCalc(c1.r, c2.r), DoCalc(c1.g, c2.g), DoCalc(c1.b, c2.b), 255);
        }
    }
}