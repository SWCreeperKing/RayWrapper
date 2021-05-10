using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Objs;
using static Raylib_cs.Raylib;

namespace RayWrapper
{
    public static class RayWrapper
    {
        public static readonly Color Transparent = new(0, 0, 0, 0);
        public static bool MouseOccupied { get; set; }
        public static Rectangle AssembleRectFromVec(Vector2 pos, Vector2 size) => new(pos.X, pos.Y, size.X, size.Y);
        public static bool IsMouseIn(this Rectangle rect) => CheckCollisionPointRec(GetMousePosition(), rect);
        public static void Draw(this Rectangle rect, Color color) => DrawRectangleRec(rect, color);

        public static void MaskDraw(this Rectangle rect, Action draw)
        {
            BeginScissorMode((int) rect.x, (int) rect.y, (int) rect.width, (int) rect.height);
            draw.Invoke();
            EndScissorMode();
        }

        public static Rectangle Shrink(this Rectangle rect, int changeBuy) =>
            new(rect.x + changeBuy, rect.y + changeBuy, rect.width - changeBuy * 2, rect.height - changeBuy * 2);

        public static Rectangle Grow(this Rectangle rect, int changeBuy) => rect.Shrink(-changeBuy);
        public static Rectangle AdjustWH(this Rectangle rect, Vector2 v2) => new(rect.x, rect.y, v2.X, v2.Y);

        public static Rectangle ExtendPos(this Rectangle rect, Vector2 v2) =>
            new(rect.x - v2.X, rect.y - v2.Y, rect.width + v2.X, rect.height + v2.Y);

        public static Rectangle Clone(this Rectangle rect) => new(rect.x, rect.y, rect.width, rect.height);
        public static bool IsColliding(this Rectangle rect1, Rectangle rect2) => CheckCollisionRecs(rect1, rect2);
        public static Vector2 Center(this Rectangle rect) => new(rect.x + rect.width / 2, rect.y + rect.height / 2);
        public static Vector2 Pos(this Rectangle rect) => new(rect.x, rect.y);
        public static Vector2 Size(this Rectangle rect) => new(rect.width, rect.height);
        public static string GetString(this Rectangle rect) => $"[({rect.x},{rect.y})({rect.width}x{rect.height})]";

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

        public static void DrawTooltip(this Rectangle box, string text, int fontSize = 24, float spacing = 1.5f) =>
            box.DrawTooltip(text, new(170, 170, 255, 240), fontSize, spacing);
        
        public static void DrawTooltip(this Rectangle box, string text, Color color, int fontSize = 24,
            float spacing = 1.5f)
        {
            if (!box.IsMouseIn()) return;
            var curPos = GetMousePosition();
            var textSize = GameBox.font.MeasureText(text, fontSize, spacing);
            var quad = GetCursorQuadrant();
            Vector2 pos = new(curPos.X - (quad % 2 != 0 ? textSize.X : 0), curPos.Y - (quad < 3 ? textSize.Y : 0));
            AssembleRectFromVec(pos, textSize).Grow(4).Draw(new(0, 0, 0, 240));
            DrawTextEx(GameBox.font, text, pos, fontSize, spacing, color);
        }
    }
}