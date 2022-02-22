using System;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace RayWrapper
{
    public static class RectWrapper
    {
        /// <summary>
        /// a <see cref="Rectangle"/> with 0 x, y, width, and height
        /// </summary>
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

        /// <summary>
        /// Adjusts the width and height of a <see cref="Rectangle"/>
        /// </summary>
        /// <param name="rect"><see cref="Rectangle"/> to adjust</param>
        /// <param name="v2">the amount to grow each one to</param>
        /// <returns>the <see cref="Rectangle"/> with the new width and height</returns>
        public static Rectangle AdjustWh(this Rectangle rect, Vector2 v2) => new(rect.x, rect.y, v2.X, v2.Y);

        /// <summary>
        /// adds to the width and height of a <see cref="Rectangle"/>
        /// </summary>
        /// <param name="rect"><see cref="Rectangle"/> to add to</param>
        /// <param name="v2">the amount to grow each one to</param>
        /// <returns>the <see cref="Rectangle"/> with the added width and height</returns>
        public static Rectangle AdjustByWh(this Rectangle rect, Vector2 v2)
        {
            return new(rect.x, rect.y, rect.width + v2.X, rect.height + v2.Y);
        }

        /// <summary>
        /// Copies the <see cref="Rectangle"/> and moves it by the given <see cref="Vector2"/>
        /// </summary>
        /// <param name="rect"><see cref="Rectangle"/> to move</param>
        /// <param name="v2">by how much the <see cref="Rectangle"/> should move</param>
        /// <returns>the <see cref="Rectangle"/> with the new position</returns>
        public static Rectangle NewMoveBy(this Rectangle rect, Vector2 v2)
        {
            return AssembleRectFromVec(rect.Pos() + v2, rect.Size());
        }

        /// <summary>
        /// Adds to the position of a given <see cref="Rectangle"/>
        /// </summary>
        /// <param name="rect">the <see cref="Rectangle"/> to move</param>
        /// <param name="v2">position to move the <see cref="Rectangle"/> by</param>
        public static void MoveBy(this ref Rectangle rect, Vector2 v2)
        {
            (rect.x, rect.y) = (rect.x + v2.X, rect.y + v2.Y);
        }

        /// <summary>
        /// Adds to the position of a given <see cref="Rectangle"/>
        /// </summary>
        /// <param name="rect">the <see cref="Rectangle"/> to move</param>
        /// <param name="x">added x position</param>
        /// <param name="y">added y position</param>
        public static void MoveBy(this ref Rectangle rect, float x = 0, float y = 0)
        {
            (rect.x, rect.y) = (rect.x + x, rect.y + y);
        }

        /// <summary>
        /// Copies the <see cref="Rectangle"/> and moves it to the given <see cref="Vector2"/>
        /// </summary>
        /// <param name="rect"><see cref="Rectangle"/> to move</param>
        /// <param name="v2">where the <see cref="Rectangle"/> should go</param>
        /// <returns>the <see cref="Rectangle"/> with the new position</returns>
        public static Rectangle NewMoveTo(this Rectangle rect, Vector2 v2) => AssembleRectFromVec(v2, rect.Size());

        /// <summary>
        /// Sets the position of a given <see cref="Rectangle"/>
        /// </summary>
        /// <param name="rect">the <see cref="Rectangle"/> to move</param>
        /// <param name="v2">position to move the <see cref="Rectangle"/> to</param>
        public static void MoveTo(this ref Rectangle rect, Vector2 v2) => (rect.x, rect.y) = (v2.X, v2.Y);

        /// <summary>
        /// Sets the position of a given <see cref="Rectangle"/>
        /// </summary>
        /// <param name="rect">the <see cref="Rectangle"/> to move</param>
        /// <param name="x">new x position</param>
        /// <param name="y">new y position</param>
        public static void MoveTo(this ref Rectangle rect, float x = 0, float y = 0) => (rect.x, rect.y) = (x, y);

        /// <summary>
        /// Clones a given <see cref="Rectangle"/>
        /// </summary>
        /// <param name="rect">the <see cref="Rectangle"/> to copy</param>
        /// <returns>copied <see cref="Rectangle"/></returns>
        public static Rectangle Clone(this Rectangle rect) => new(rect.x, rect.y, rect.width, rect.height);

        /// <summary>
        /// Copies the <see cref="Rectangle"/> and sets its size to the given <see cref="Vector2"/>
        /// </summary>
        /// <param name="rect">the <see cref="Rectangle"/> to set the size of</param>
        /// <param name="v2">what the <see cref="Rectangle"/>'s size should be</param>
        /// <returns>the <see cref="Rectangle"/> with the new size</returns>
        public static Rectangle SetSize(this Rectangle rect, Vector2 v2) => new(rect.x, rect.y, v2.X, v2.Y);

        /// <summary>
        /// Checks if 2 <see cref="Rectangle"/>s are colliding
        /// </summary>
        /// <param name="rect1">1st <see cref="Rectangle"/></param>
        /// <param name="rect2">2nd <see cref="Rectangle"/></param>
        /// <returns>if the <see cref="Rectangle"/>s are colliding</returns>
        public static bool IsColliding(this Rectangle rect1, Rectangle rect2) => CheckCollisionRecs(rect1, rect2);

        /// <summary>
        /// Returns the center of a <see cref="Rectangle"/> as a <see cref="Vector2"/>
        /// </summary>
        /// <param name="rect">the <see cref="Rectangle"/> to get the center of</param>
        /// <returns>the center of the given <see cref="Rectangle"/></returns>
        public static Vector2 Center(this Rectangle rect) => new(rect.x + rect.width / 2, rect.y + rect.height / 2);

        /// <summary>
        /// Returns the position of a <see cref="Rectangle"/> as a <see cref="Vector2"/>
        /// </summary>
        /// <param name="rect">the <see cref="Rectangle"/> to get the position of</param>
        /// <returns>the position of the given <see cref="Rectangle"/></returns>
        public static Vector2 Pos(this Rectangle rect) => new(rect.x, rect.y);

        /// <summary>
        /// Returns the size of a <see cref="Rectangle"/> as a <see cref="Vector2"/>
        /// </summary>
        /// <param name="rect">the <see cref="Rectangle"/> to get the size of</param>
        /// <returns>the size of the given <see cref="Rectangle"/></returns>
        public static Vector2 Size(this Rectangle rect) => new(rect.width, rect.height);

        /// <summary>
        /// Converts a <see cref="Rectangle"/> into a proper string
        /// as [(x, y)(w, h)]
        /// </summary>
        /// <param name="rect">the <see cref="Rectangle"/> to turn into a string</param>
        /// <returns>the <see cref="Rectangle"/> as "[(x, y)(w, h)]"</returns>
        public static string GetString(this Rectangle rect) => $"[({rect.x},{rect.y})({rect.width}x{rect.height})]";

        /// <summary>
        /// Checks if 2 <see cref="Rectangle"/>s are the same
        /// </summary>
        /// <param name="rect1">1st <see cref="Rectangle"/></param>
        /// <param name="rect2">2nd <see cref="Rectangle"/></param>
        /// <returns>if the 2 <see cref="Rectangle"/>s given are the same</returns>
        public static bool IsEqualTo(this Rectangle rect1, Rectangle rect2) => rect1.GetString() == rect2.GetString();

        /// <summary>
        /// Draws a circle using a <see cref="Rectangle"/>
        /// </summary>
        /// <param name="rect">the <see cref="Rectangle"/> to draw as a circle</param>
        /// <param name="color">the <see cref="Color"/> of the circle</param>
        /// <param name="segments">the amount of segments the circle should have</param>
        public static void DrawCircle(this Rectangle rect, Color color, int segments = 10)
        {
            rect.DrawRounded(color, 1, segments);
        }

        /// <summary>
        /// Draws a <see cref="Rectangle"/> with a gradient
        /// </summary>
        /// <param name="rect">the <see cref="Rectangle"/> to draw as a gradient</param>
        /// <param name="c1">start <see cref="Color"/></param>
        /// <param name="c2">end <see cref="Color"/></param>
        /// <param name="isVertical">if the gradient should be vertical vs horizontal</param>
        public static void DrawGradient(this Rectangle rect, Color c1, Color c2, bool isVertical = false)
        {
            if (isVertical)
            {
                DrawRectangleGradientV((int) rect.x, (int) rect.y, (int) rect.width, (int) rect.height, c1, c2);
            }
            else DrawRectangleGradientH((int) rect.x, (int) rect.y, (int) rect.width, (int) rect.height, c1, c2);
        }

        /// <summary>
        /// Draws a given <see cref="Rectangle"/> with rounded corners
        /// </summary>
        /// <param name="rect">the <see cref="Rectangle"/> to draw</param>
        /// <param name="color">the <see cref="Color"/> to draw it as</param>
        /// <param name="roundness">how round the corners should be</param>
        /// <param name="segments">how many segments there should be</param>
        public static void DrawRounded(this Rectangle rect, Color color, float roundness = .5f, int segments = 10)
        {
            DrawRectangleRounded(rect, roundness, segments, color);
        }

        /// <summary>
        /// Draw a given <see cref="Rectangle"/> with rounded corners and as an outline
        /// </summary>
        /// <param name="rect">the <see cref="Rectangle"/> to draw</param>
        /// <param name="color">the <see cref="Color"/> to draw it as</param>
        /// <param name="roundness">how round the corners should be</param>
        /// <param name="thickness">how thick the outline should be</param>
        public static void DrawRoundedLines(this Rectangle rect, Color color, float roundness = .5f,
            int thickness = 3)
        {
            // todo: undo when update fixes
            // DrawRectangleRoundedLines(rect, roundness, 5, thickness, color);
        }

        /// <summary>
        /// Draw a given <see cref="Rectangle"/> as an outlined circle
        /// </summary>
        /// <param name="rect">the <see cref="Rectangle"/> to draw</param>
        /// <param name="color">the <see cref="Color"/> to draw it as</param>
        /// <param name="thickness">how thick the outline should be</param>
        public static void DrawHallowCircle(this Rectangle rect, Color color, int thickness = 3)
        {
            // todo: undo when update fixes
            // DrawRectangleRoundedLines(rect, 1f, 5, thickness, color);
        }

        /// <summary>
        /// Draw a given <see cref="Rectangle"/> as an outline
        /// </summary>
        /// <param name="rect">the <see cref="Rectangle"/> to draw</param>
        /// <param name="color">the <see cref="Color"/> to draw it as</param>
        /// <param name="thickness">how thick the outline should be</param>
        public static void DrawHallowRect(this Rectangle rect, Color color, int thickness = 3)
        {
            // todo: undo when update fixes
            // DrawRectangleLinesEx(rect, thickness, color);
        }

        /// <summary>
        /// shrinks a given <see cref="Rectangle"/>'s position and size
        /// </summary>
        /// <param name="rect">the <see cref="Rectangle"/> to shrink</param>
        /// <param name="changeBuy">the amount to shrink by</param>
        /// <returns>the shrunk <see cref="Rectangle"/></returns>
        public static Rectangle Shrink(this Rectangle rect, int changeBuy)
        {
            return new Rectangle(rect.x + changeBuy, rect.y + changeBuy, rect.width - changeBuy * 2,
                rect.height - changeBuy * 2);
        }

        /// <summary>
        /// extends the starting position of the <see cref="Rectangle"/> while keeping the position of where the w/h end up
        /// </summary>
        /// <param name="rect">the <see cref="Rectangle"/> to copy and extend</param>
        /// <param name="v2">how much to extend the <see cref="Rectangle"/> by</param>
        /// <returns>the <see cref="Rectangle"/> with the new dimensions</returns>
        public static Rectangle ExtendPos(this Rectangle rect, Vector2 v2)
        {
            return new Rectangle(rect.x - v2.X, rect.y - v2.Y, rect.width + v2.X, rect.height + v2.Y);
        }

        /// <summary>
        /// draws a tooltip if a cursor is inside the bounds of the given <see cref="Rectangle"/>
        /// </summary>
        /// <param name="box">bounds to check</param>
        /// <param name="text">the tooltip text</param>
        public static void DrawTooltip(this Rectangle box, string text)
        {
            if (box.IsMouseIn()) GameBox.tooltip.Add(text);
        }

        /// <summary>
        /// executes <paramref name="draw"/> inside of a masked <see cref="Rectangle"/>
        /// </summary>
        /// <param name="rect">the mask</param>
        /// <param name="draw">what to draw in the mask</param>
        public static void MaskDraw(this Rectangle rect, Action draw)
        {
            rect.Pos().MaskDraw(rect.Size(), draw);
        }
    }
}