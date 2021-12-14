using System;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace RayWrapper
{
    public static class RectWrapper
    {
        public static int maskingLayer = 0;
        public static Rectangle Zero = new(0, 0, 0, 0);

        /// <summary>
        /// Makes a <see cref="Rectangle"/> from 2 <see cref="Vector2"/>s
        /// </summary>
        /// <param name="pos">Position of new rectangle</param>
        /// <param name="size">Size of new rectangle</param>
        /// <returns>The new <see cref="Rectangle"/> of <paramref name="pos"/> and <paramref name="size"/></returns>
        public static Rectangle AssembleRectFromVec(Vector2 pos, Vector2 size) => new(pos.X, pos.Y, size.X, size.Y);

        /// <summary>
        /// Checks if a <see cref="Vector2"/> is inside the bounds of a <see cref="Rectangle"/> 
        /// </summary>
        /// <param name="rect">Bounds to check with</param>
        /// <param name="v2"><see cref="Vector2"/> to check</param>
        /// <returns>If the <paramref name="pos"/> is in the <paramref name="rect"/></returns>
        public static bool IsV2In(this Rectangle rect, Vector2 v2) => CheckCollisionPointRec(v2, rect);

        /// <summary>
        /// Uses <see cref="IsV2In"/> to test if <see cref="Raylib.GetMousePosition"/> is in a <see cref="Rectangle"/>
        /// </summary>
        /// <param name="rect"><see cref="Rectangle"/> to check with</param>
        /// <returns>if the <see cref="Raylib.GetMousePosition"/> is in <paramref name="rect"/></returns>
        public static bool IsMouseIn(this Rectangle rect) => rect.IsV2In(GameBox.mousePos);

        /// <summary>
        /// Draws a <see cref="Rectangle"/> with a <see cref="Color"/> 
        /// </summary>
        /// <param name="rect"><see cref="Rectangle"/> to draw</param>
        /// <param name="color"><see cref="Color"/> to draw the <see cref="Rectangle"/></param>
        public static void Draw(this Rectangle rect, Color color) => DrawRectangleRec(rect, color);

        /// <summary>
        /// Grows a <see cref="Rectangle"/> from its center
        /// </summary>
        /// <param name="rect"><see cref="Rectangle"/> to grow</param>
        /// <param name="changeBy">Amount to grow the rectangle by</param>
        /// <returns>The <paramref name="rect"/> that grew from its center by <paramref name="changeBy"/></returns>
        public static Rectangle Grow(this Rectangle rect, int changeBy) => rect.Shrink(-changeBy);

        public static Rectangle AdjustWh(this Rectangle rect, Vector2 v2) => new(rect.x, rect.y, v2.X, v2.Y);
        public static Rectangle NewMoveTo(this Rectangle rect, Vector2 v2) => AssembleRectFromVec(v2, rect.Size());
        public static void MoveTo(this ref Rectangle rect, Vector2 v2) => (rect.x, rect.y) = (v2.X, v2.Y);
        public static void MoveTo(this ref Rectangle rect, float x = 0, float y = 0) => (rect.x, rect.y) = (x, y);
        public static Rectangle Clone(this Rectangle rect) => new(rect.x, rect.y, rect.width, rect.height);
        public static Rectangle SetSize(this Rectangle rect, Vector2 v2) => new(rect.x, rect.y, v2.X, v2.Y);
        public static bool IsColliding(this Rectangle rect1, Rectangle rect2) => CheckCollisionRecs(rect1, rect2);
        public static Vector2 Center(this Rectangle rect) => new(rect.x + rect.width / 2, rect.y + rect.height / 2);
        public static Vector2 Pos(this Rectangle rect) => new(rect.x, rect.y);
        public static Vector2 Size(this Rectangle rect) => new(rect.width, rect.height);
        public static string GetString(this Rectangle rect) => $"[({rect.x},{rect.y})({rect.width}x{rect.height})]";
        public static bool IsEqualTo(this Rectangle rect1, Rectangle rect2) => rect1.GetString() == rect2.GetString();

        public static void DrawCircle(this Rectangle rect, Color color, int segments = 10) =>
            rect.DrawRounded(color, 1, segments);

        public static void DrawGradiant(this Rectangle rect, Color c1, Color c2, bool isVertical = false)
        {
            if (isVertical) DrawRectangleGradientV((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height, c1, c2);
            else DrawRectangleGradientH((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height, c1, c2);
        }

        public static void DrawRounded(this Rectangle rect, Color color, float roundness = .5f, int segments = 10) =>
            DrawRectangleRounded(rect, roundness, segments, color);

        public static void DrawRoundedLines(this Rectangle rect, Color color, float roundness = .5f,
            int lineThickness = 3) =>
            DrawRectangleRoundedLines(rect, roundness, 5, lineThickness, color);

        public static void DrawHallowCircle(this Rectangle rect, Color color, int thickness = 3) =>
            DrawRectangleRoundedLines(rect, 1f, 5, thickness, color);

        public static Rectangle Shrink(this Rectangle rect, int changeBuy) =>
            new(rect.x + changeBuy, rect.y + changeBuy, rect.width - changeBuy * 2, rect.height - changeBuy * 2);

        public static Rectangle AdjustByWh(this Rectangle rect, Vector2 v2) =>
            new(rect.x, rect.y, rect.width + v2.X, rect.height + v2.Y);

        public static Rectangle NewMoveBy(this Rectangle rect, Vector2 v2) =>
            AssembleRectFromVec(rect.Pos() + v2, rect.Size());

        public static void MoveBy(this ref Rectangle rect, Vector2 v2) =>
            (rect.x, rect.y) = (rect.x + v2.X, rect.y + v2.Y);

        public static void MoveBy(this ref Rectangle rect, float x = 0, float y = 0) =>
            (rect.x, rect.y) = (rect.x + x, rect.y + y);

        public static Rectangle ExtendPos(this Rectangle rect, Vector2 v2) =>
            new(rect.x - v2.X, rect.y - v2.Y, rect.width + v2.X, rect.height + v2.Y);

        public static void DrawHallowRect(this Rectangle rect, Color color, int thickness = 3) =>
            DrawRectangleLinesEx(rect, thickness, color);

        public static void DrawTooltip(this Rectangle box, string text)
        {
            if (box.IsMouseIn()) GameBox.tooltip.Add(text);
        }

        public static void MaskDraw(this Rectangle rect, Action draw)
        {
            maskingLayer++;
            BeginScissorMode((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
            draw.Invoke();
            if (maskingLayer == 1) EndScissorMode();
            maskingLayer--;
        }

        public static Rectangle Multi(this Rectangle rect, Vector2 v2) =>
            AssembleRectFromVec(rect.Pos() * v2, rect.Size() * v2);
    }
}