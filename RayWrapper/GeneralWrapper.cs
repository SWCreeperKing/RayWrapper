using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using Raylib_cs;
using RayWrapper.Vars;
using static Raylib_cs.Color;
using static Raylib_cs.Raylib;
using static RayWrapper.RectWrapper;

namespace RayWrapper
{
    public static class GeneralWrapper
    {
        public static readonly Color Transparent = new(0, 0, 0, 0);
        public const double DegToRad = Math.PI * 2 / 360d;

        public static Vector2 Max(this Vector2 v2) => new(Math.Max(v2.X, v2.Y));
        public static Vector2 Max(this Vector2 v2, Vector2 v22) => new(Math.Max(v2.X, v22.X), Math.Max(v2.Y, v22.Y));
        public static unsafe string FromClipboard() => Utf8StringUtils.GetUTF8String(GetClipboardText());

        public static void DrawCenterText(this Font font, Vector2 pos, string text, Color color, float fontSize = 24,
            float spacing = 1.5f) =>
            DrawTextEx(font, text, pos - MeasureTextEx(font, text, fontSize, spacing) / 2, fontSize, spacing, color);

        public static void DrawCenterWrapText(this Font font, Rectangle rect, string text, Color color,
            float fontSize = 24,
            float spacing = 1.5f)
        {
            rect.MoveBy(-(MeasureTextEx(font, text, fontSize, spacing) / 2));
            // DrawTextRec(font, text, rect, fontSize, spacing, true, color);
        }

        public static void DrawTextRec(this Font font, string text, Rectangle rect, Color fontColor,
            int fontSize = 24,
            float spacing = 1.5f) =>
            DrawTextRec(font, text, rect, fontSize, spacing, true, fontColor);

        public static void DrawTextRec(this Font font, string text, Rectangle rec, float fontSize, float spacing,
            bool wordWrap, Color tint) =>
            DrawTextRec(font, text, rec, fontSize, spacing, wordWrap, tint, 0, 0, WHITE, WHITE);

        public static unsafe void DrawTextRec(this Font font, string text, Rectangle rec, float fontSize, float
            spacing, bool wordWrap, Color tint, int selectStart, int
            selectLength, Color selectTint, Color selectBackTint)
        {
            var bytes = Encoding.ASCII.GetBytes(text);
            sbyte* sb;
            fixed (byte* b = bytes) sb = (sbyte*)b;
            var length = TextLength(sb); // Total length in bytes of the text, scanned by codepoints in loop

            var textOffsetY = 0f; // Offset between lines (on line break '\n')
            var textOffsetX = 0f; // Offset X to next character to draw

            var scaleFactor = fontSize / font.baseSize;
            var state = !wordWrap;

            var startLine = -1; // Index where to begin drawing (where a line begins)
            var endLine = -1; // Index where to stop drawing (where a line ends)
            var lastk = -1; // Holds last value of the character position

            for (int i = 0, k = 0; i < length; i++, k++)
            {
                // Get next codepoint from byte string and glyph index in font
                var codepointByteCount = 0;
                var codepoint = GetCodepoint(&sb[i], &codepointByteCount);
                var index = GetGlyphIndex(font, codepoint);

                // NOTE: Normally we exit the decoding sequence as soon as a bad byte is found (and return 0x3f)
                // but we need to draw all of the bad bytes using the '?' symbol moving one byte
                if (codepoint == 0x3f) codepointByteCount = 1;
                i += codepointByteCount - 1;

                float glyphWidth = 0;
                if (codepoint != '\n')
                {
                    glyphWidth = (font.glyphs[index].advanceX == 0)
                        ? font.recs[index].width * scaleFactor
                        : font.glyphs[index].advanceX * scaleFactor;

                    if (i + 1 < length) glyphWidth = glyphWidth + spacing;
                }

                if (!state)
                {
                    if (codepoint is ' ' or '\t' or '\n') endLine = i;

                    if (textOffsetX + glyphWidth > rec.width)
                    {
                        endLine = endLine < 1 ? i : endLine;
                        if (i == endLine) endLine -= codepointByteCount;
                        if (startLine + codepointByteCount == endLine) endLine = i - codepointByteCount;

                        state = !state;
                    }
                    else if (i + 1 == length)
                    {
                        endLine = i;
                        state = !state;
                    }
                    else if (codepoint == '\n') state = !state;

                    if (state)
                    {
                        textOffsetX = 0;
                        i = startLine;
                        glyphWidth = 0;

                        // Save character position when we switch states
                        var tmp = lastk;
                        lastk = k - 1;
                        k = tmp;
                    }
                }
                else
                {
                    if (codepoint == '\n')
                    {
                        if (!wordWrap)
                        {
                            textOffsetY += (font.baseSize + font.baseSize / 2) * scaleFactor;
                            textOffsetX = 0;
                        }
                    }
                    else
                    {
                        if (!wordWrap && textOffsetX + glyphWidth > rec.width)
                        {
                            textOffsetY += (font.baseSize + font.baseSize / 2) * scaleFactor;
                            textOffsetX = 0;
                        }

                        // When text overflows rectangle height limit, just stop drawing
                        if (textOffsetY + font.baseSize * scaleFactor > rec.height) break;

                        // Draw selection background
                        var isGlyphSelected = false;
                        if (selectStart >= 0 && k >= selectStart && k < selectStart + selectLength)
                        {
                            DrawRectangleRec(
                                new Rectangle(rec.x + textOffsetX - 1, rec.y + textOffsetY, glyphWidth,
                                    font.baseSize * scaleFactor), selectBackTint);
                            isGlyphSelected = true;
                        }

                        // Draw current character glyph
                        if (codepoint != ' ' && codepoint != '\t')
                            DrawTextCodepoint(font, codepoint, new Vector2(rec.x + textOffsetX, rec.y + textOffsetY),
                                fontSize, isGlyphSelected ? selectTint : tint);
                    }

                    if (wordWrap && i == endLine)
                    {
                        textOffsetY += (font.baseSize + font.baseSize / 2) * scaleFactor;
                        textOffsetX = 0;
                        startLine = endLine;
                        endLine = -1;
                        glyphWidth = 0;
                        selectStart += lastk - k;
                        k = lastk;
                        state = !state;
                    }
                }

                textOffsetX += glyphWidth;
            }
        }

        public static void DrawText(this Font font, string text, Vector2 pos, Color fontColor, int fontSize = 24,
            float spacing = 1.5f) =>
            DrawTextEx(font, text, pos, fontSize, spacing, fontColor);

        public static Vector2 MeasureText(this Font font, string text, float fontSize = 24f, float spacing = 1.5f) =>
            MeasureTextEx(font, text, fontSize, spacing);

        public static string GetString(this KeyboardKey key) =>
            $"{key}".Replace("KEY_MENU", "KEY_R").Replace("KEY_", "").Replace("_", " ").ToLower();

        public static Color MakeLighter(this Color color) =>
            new((int)Math.Min(color.r * 1.5, 255), (int)Math.Min(color.g * 1.5, 255), (int)Math.Min(color.b * 1.5, 255),
                color.a);

        public static Color MakeDarker(this Color color) =>
            new((int)(color.r / 1.7), (int)(color.g / 1.7), (int)(color.b / 1.7), color.a);

        public static Color EditColor(this Color color, int r = 0, int g = 0, int b = 0, int a = 0) =>
            new(color.r + r, color.g + g, color.b + b, color.a + a);

        public static Color SetAlpha(this Color color, int a) => new(color.r, color.g, color.b, a);

        public static void DrawLine(this Vector2 v1, Vector2 v2, Color color, float thickness = 3) =>
            DrawLineEx(v1, v2, thickness, color);

        public static void DrawBezLine(this Vector2 v1, Vector2 v2, Color color, float thickness = 3) =>
            DrawLineBezier(v1, v2, thickness, color);

        public static void DrawLine(this (Vector2 v1, Vector2 v2) l, Color color, float thickness = 3) =>
            DrawLineEx(l.v1, l.v2, thickness, color);

        public static void
            DrawLine(this (float x1, float y1, float x2, float y2) l, Color color, float thickness = 3) =>
            (new Vector2(l.x1, l.y1), new Vector2(l.x2, l.y2)).DrawLine(color, thickness);

        public static Vector2 Add(this Vector2 v2, float f) => v2 + new Vector2(f);

        public static Color Percent(this Color c1, Color c2, float percent)
        {
            int DoCalc(int c1, int c2) => Math.Clamp((int)((1.0 - percent) * c1 + percent * c2 + 0.5), 1, 254);
            return new Color(DoCalc(c1.r, c2.r), DoCalc(c1.g, c2.g), DoCalc(c1.b, c2.b), 255);
        }

        public static float Next(this Random r, float min, float max) => (float)(r.NextDouble() * (max - min) + min);

        public static Vector2[] CalcVectsFromFloats(this float[] array, Rectangle rect)
        {
            var step = rect.width / array.Length;
            var vects = new Vector2[array.Length];
            for (var i = 0; i < array.Length; i++)
                vects[i] = new Vector2(rect.x + rect.height + i * step, array[i]);
            return vects;
        }

        public static void DrawArrAsLine(this Vector2[] array, Color color, int thickness = 3)
        {
            for (var i = 1; i < array.Length; i++) array[i - 1].DrawLine(array[i], color, thickness);
        }

        public static void DrawArrAsBezLine(this Vector2[] array, Color color, int thickness = 3)
        {
            for (var i = 1; i < array.Length; i++) array[i - 1].DrawBezLine(array[i], color, thickness);
        }

        public static Texture2D Texture(this Image i) => LoadTextureFromImage(i);

        public static void Draw(this Texture2D t, Vector2 pos, Color tint, float rot = 0, float scale = 1) =>
            DrawTextureEx(t, pos, rot, scale, tint);

        public static void DrawPro(this Texture2D t, Vector2 pos, int rotation = 0) =>
            DrawTexturePro(t, new Rectangle(0, 0, t.width, t.height), new Rectangle(pos.X, pos.Y, t.width, t.height),
                new Vector2(t.width / 2f, t.height / 2f), rotation, WHITE);

        public static void Set<T>(this T t, T overrider)
        {
            foreach (var field in typeof(T).GetRuntimeFields().Where(f => !f.IsStatic))
            {
                try
                {
                    field.SetValue(t, field.GetValue(overrider));
                }
                catch (TargetException e)
                {
                    Logger.Log(Logger.Level.Warning, $"FIELD: {field.Name} CORRUPT? {e.Message}");
                }
            }
        }

        public static bool IsVectInVects(this Vector2 vect, Vector2 pos, Vector2 size, float scale = 1) =>
            pos.X * scale < vect.X && pos.Y * scale < vect.Y && vect.X < (pos.X + size.X) * scale &&
            vect.Y < (pos.Y + size.Y) * scale;

        public static Vector2 Size(this Image img) => img.Texture().Size();
        public static Vector2 Size(this Texture2D t2d) => new(t2d.width, t2d.height);

        public static void MaskDraw(this Vector2 pos, Vector2 size, Action draw)
        {
            maskingLayer++;
            BeginScissorMode((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
            draw.Invoke();
            if (maskingLayer == 1) EndScissorMode();
            maskingLayer--;
        }

        public static void DrawCircle(this Vector2 v2, float r, Color? color = null) =>
            DrawCircleV(v2, r, color ?? WHITE);

        public static Vector2 FixVector(this Vector2 v2) => new Vector2(v2.X.Fix(), v2.Y.Fix());

        public static bool IsFixable(this float f) => float.IsNaN(f) || float.IsInfinity(f);

        public static float Fix(this float f) =>
            f.IsFixable()
                ? float.IsNegative(f)
                    ? float.MinValue
                    : float.MaxValue
                : f;

        public static Vector2 Rotate(this Vector2 v2, float degrees)
        {
            var sin = (float)Math.Sin(degrees * DegToRad);
            var cos = (float)Math.Cos(degrees * DegToRad);
            var tx = v2.X;
            var ty = v2.Y;

            v2.X = cos * tx - sin * ty;
            v2.Y = sin * tx + cos * ty;
            return v2;
        }
    }
}