using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Raylib_CsLo;
using RayWrapper.Base.GameBox;
using ZimonIsHimUtils.ExtensionMethods;
using static System.Numerics.Vector2;
using static Raylib_CsLo.Raylib;
using static RayWrapper.Base.GameBox.Logger.Level;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.Base.Extras;

public static class Core
{
    public static int maskingLayer;

    /// <summary>
    /// a <see cref="Color"/> with 0 rgba
    /// </summary>
    public static readonly Color Transparent = new(0, 0, 0, 0);

    /// <summary>
    /// the degree to radians constant
    /// </summary>
    public const double DegToRad = Math.PI * 2 / 360d;

    /// <summary>
    /// gets the size of an <see cref="Raylib_CsLo.Image"/> as a <see cref="Vector2"/>
    /// </summary>
    /// <param name="image">the <see cref="Raylib_CsLo.Image"/> to get the size of</param>
    /// <returns>the size of an <see cref="Raylib_CsLo.Image"/></returns>
    public static Vector2 Size(this Image image) => image.Texture().Size();

    /// <summary>
    /// gets the size of a <see cref="Texture"/> as a <see cref="Vector2"/>
    /// </summary>
    /// <param name="texture">the <see cref="Texture"/> to get the size of</param>
    /// <returns>the size of a <see cref="Texture"/></returns>
    public static Vector2 Size(this Texture texture) => new(texture.width, texture.height);

    /// <summary>
    /// gets text from the user's clipboard
    /// </summary>
    /// <returns>the text from the user's clipboard</returns>
    public static string FromClipboard() => GetClipboardText_();

    /// <summary>
    /// draw text to the screen
    /// </summary>
    /// <param name="font"><see cref="Font"/> to use</param>
    /// <param name="text">text to draw</param>
    /// <param name="pos">position to draw at</param>
    /// <param name="fontColor"><see cref="Color"/> to draw with</param>
    /// <param name="fontSize">size of text</param>
    /// <param name="spacing">spacing of the characters</param>
    /// <param name="origin">origin for rotation</param>
    /// <param name="rotation">rotation for text in degrees</param>
    public static void DrawText(this Font font, string text, Vector2 pos, Color fontColor, float fontSize = 24,
        float spacing = 1.5f, Vector2? origin = null, float rotation = 0)
    {
        DrawTextPro(font, text, pos, origin ?? Zero, rotation, fontSize, spacing, fontColor);
    }

    /// <summary>
    /// measures a given text with a given <see cref="Font"/>
    /// </summary>
    /// <param name="font"><see cref="Font"/> to measure with</param>
    /// <param name="text">text to measure</param>
    /// <param name="fontSize">size of the text</param>
    /// <param name="spacing">spacing of the characters</param>
    /// <returns>the size of the text with a given <see cref="Font"/></returns>
    public static Vector2 MeasureText(this Font font, string text, float fontSize = 24f, float spacing = 1.5f)
    {
        return MeasureTextEx(font, text, fontSize, spacing);
    }

    /// <summary>
    /// converts <see cref="KeyboardKey"/> into a more readable string
    /// </summary>
    /// <param name="key"><see cref="KeyboardKey"/> to stringify</param>
    /// <returns>the string version of <see cref="KeyboardKey"/></returns>
    public static string GetString(this KeyboardKey key)
    {
        return $"{key}".Replace("KEY_MENU", "KEY_R").Replace("KEY_", string.Empty).Replace("_", " ").ToLower();
    }

    /// <summary>
    /// adds rgba values to a given <see cref="Color"/>
    /// </summary>
    /// <param name="color"><see cref="Color"/> to edit</param>
    /// <param name="r">amount of red to add</param>
    /// <param name="g">amount of green to add</param>
    /// <param name="b">amount of blue to add</param>
    /// <param name="a">amount of alpha to add</param>
    /// <returns>the <see cref="Color"/> with the new values</returns>
    public static Color EditColor(this Color color, int r = 0, int g = 0, int b = 0, int a = 0)
    {
        return new Color(color.r + r, color.g + g, color.b + b, color.a + a);
    }

    /// <summary>
    /// sets the alpha of a given <see cref="Color"/>
    /// </summary>
    /// <param name="color"><see cref="Color"/> to modify</param>
    /// <param name="a">the new alpha to set</param>
    /// <returns>the <see cref="Color"/> with the new alpha</returns>
    public static Color SetAlpha(this Color color, int a) => new(color.r, color.g, color.b, a);

    /// <summary>
    /// draws a line from <paramref name="v1"/> to <paramref name="v2"/>
    /// </summary>
    /// <param name="v1">starting <see cref="Vector2"/></param>
    /// <param name="v2">ending <see cref="Vector2"/></param>
    /// <param name="color">the <see cref="Color"/> of the line</param>
    /// <param name="thickness">the thickness of the line</param>
    public static void DrawLine(this Vector2 v1, Vector2 v2, Color color, float thickness = 3)
    {
        DrawLineEx(v1, v2, thickness, color);
    }

    /// <summary>
    /// draws a Bezier line from <paramref name="v1"/> to <paramref name="v2"/>
    /// </summary>
    /// <param name="v1">starting <see cref="Vector2"/></param>
    /// <param name="v2">ending <see cref="Vector2"/></param>
    /// <param name="color">the <see cref="Color"/> of the line</param>
    /// <param name="thickness">the thickness of the line</param>
    public static void DrawBezLine(this Vector2 v1, Vector2 v2, Color color, float thickness = 3)
    {
        DrawLineBezier(v1, v2, thickness, color);
    }

    /// <inheritdoc cref="DrawLine(System.Numerics.Vector2,System.Numerics.Vector2,Raylib_CsLo.Color,float)"/>
    /// <remarks>v1 and v2 are stored as a tuple</remarks>
    public static void DrawLine(this (Vector2 v1, Vector2 v2) l, Color color, float thickness = 3)
    {
        var (v1, v2) = l;
        DrawLineEx(v1, v2, thickness, color);
    }

    /// <inheritdoc cref="DrawLine(System.Numerics.Vector2,System.Numerics.Vector2,Raylib_CsLo.Color,float)"/>
    /// <remarks>v1 and v2 are split into floats and stored as a tuple of 4 floats</remarks>
    public static void DrawLine(this (float x1, float y1, float x2, float y2) l, Color color, float thickness = 3)
    {
        (new Vector2(l.x1, l.y1), new Vector2(l.x2, l.y2)).DrawLine(color, thickness);
    }

    /// <summary>
    /// add float to <see cref="Vector2"/>
    /// </summary>
    /// <param name="v2"><see cref="Vector2"/> to add to</param>
    /// <param name="f">float to add</param>
    /// <returns>the sum of the <see cref="Vector2"/> and the float</returns>
    public static Vector2 Add(this Vector2 v2, float f) => v2 + new Vector2(f);

    /// <summary>
    /// returns a <see cref="Color"/> that is a % b/t 2 other <see cref="Color"/>s 
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <param name="percent"></param>
    /// <returns></returns>
    public static Color Percent(this Color c1, Color c2, float percent)
    {
        int DoCalc(int c1, int c2) => Math.Clamp((int) ((1.0 - percent) * c1 + percent * c2 + 0.5), 1, 254);
        return new Color(DoCalc(c1.r, c2.r), DoCalc(c1.g, c2.g), DoCalc(c1.b, c2.b), 255);
    }

    /// <summary>
    /// adds a next for floating point number for <see cref="Random"/>
    /// </summary>
    /// <param name="r">the <see cref="Random"/> to use</param>
    /// <param name="min">min number [inclusive]</param>
    /// <param name="max">max number [exclusive]</param>
    /// todo: make sure that this is max exclusive
    /// <returns>a random float b/t <paramref name="min"/> and <paramref name="max"/></returns>
    public static float Next(this Random r, float min, float max) => (float) (r.NextDouble() * (max - min) + min);

    /// <inheritdoc cref="Next(System.Random,float,float)"/>
    public static double Next(this Random r, double min, double max) => r.NextDouble() * (max - min) + min;

    /// <summary>
    /// converts an array of floats into an array of points based on a given <see cref="Raylib_CsLo.Rectangle"/>
    /// </summary>
    /// <param name="array">floats to chart</param>
    /// <param name="rect">the <see cref="Raylib_CsLo.Rectangle"/>to base the points on</param>
    /// <returns></returns>
    public static Vector2[] CalcVectsFromFloats(this float[] array, Raylib_CsLo.Rectangle rect)
    {
        var step = rect.width / array.Length;
        var vectors = new Vector2[array.Length];

        for (var i = 0; i < array.Length; i++)
        {
            vectors[i] = new Vector2(rect.x + rect.height + i * step, array[i]);
        }

        return vectors;
    }

    /// <summary>
    /// draws an array of <see cref="Vector2"/>s as a line
    /// </summary>
    /// <param name="array">points</param>
    /// <param name="color"><see cref="Color"/> of the line</param>
    /// <param name="thickness">thickness of the line</param>
    public static void DrawArrAsLine(this Vector2[] array, Color color, int thickness = 3)
    {
        for (var i = 1; i < array.Length; i++) array[i - 1].DrawLine(array[i], color, thickness);
    }

    /// <summary>
    /// draws an array of <see cref="Vector2"/>s as a Bezier line
    /// </summary>
    /// <param name="array">points</param>
    /// <param name="color"><see cref="Color"/> of the line</param>
    /// <param name="thickness">thickness of the line</param>
    public static void DrawArrAsBezLine(this Vector2[] array, Color color, int thickness = 3)
    {
        for (var i = 1; i < array.Length; i++) array[i - 1].DrawBezLine(array[i], color, thickness);
    }

    /// <summary>
    /// loads a <see cref="Texture"/> from an <see cref="Raylib_CsLo.Image"/>
    /// </summary>
    /// <param name="i">the <see cref="Raylib_CsLo.Image"/> to get the <see cref="Texture"/> from</param>
    /// <returns>the <see cref="Texture"/> from <paramref name="i"/></returns>
    public static Texture Texture(this Image i) => LoadTextureFromImage(i);

    /// <summary>
    /// draws a given <see cref="Texture"/>
    /// </summary>
    /// <param name="t"><see cref="Texture"/> to draw</param>
    /// <param name="pos">where to draw <see cref="Texture"/></param>
    /// <param name="tint"><see cref="Color"/> tint of <see cref="Texture"/> (white should be default)</param>
    /// <param name="rot">rotation of <see cref="Texture"/></param>
    /// <param name="scale">scale of <see cref="Texture"/></param>
    public static void Draw(this Texture t, Vector2 pos, Color tint, float rot = 0, float scale = 1)
    {
        DrawTextureEx(t, pos, rot, scale, tint);
    }

    /// <summary>
    /// draws a given <see cref="Texture"/>
    /// </summary>
    /// <param name="t"><see cref="Texture"/> to draw</param>
    /// <param name="pos">where to draw <see cref="Texture"/></param>
    /// <param name="rotation">rotation of <see cref="Texture"/></param>
    public static void DrawPro(this Texture t, Vector2 pos, int rotation = 0)
    {
        DrawTexturePro(t, new Raylib_CsLo.Rectangle(0, 0, t.width, t.height),
            new Raylib_CsLo.Rectangle(pos.X, pos.Y, t.width, t.height),
            new Vector2(t.width / 2f, t.height / 2f), rotation, WHITE);
    }

    /// <summary>
    /// this is basically = but it doesn't not reset the original reference
    /// </summary>
    /// <param name="t">original</param>
    /// <param name="overrider">new</param>
    /// <typeparam name="T">any type</typeparam>
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
                Logger.Log(Warning, $"FIELD: {field.Name} CORRUPT? {e.Message}");
            }
        }
    }

    /// <summary>
    /// check if a <see cref="Vector2"/> is inside a rectangle formed from 2 other <see cref="Vector2"/>
    /// </summary>
    /// <param name="vect">the <see cref="Vector2"/> to check</param>
    /// <param name="pos">top left corner of rectangle</param>
    /// <param name="size">size of rectangle</param>
    /// <param name="scale">scale of rectangle</param>
    /// <returns>if a <see cref="Vector2"/> is inside a rectangle formed from 2 other <see cref="Vector2"/></returns>
    public static bool IsVectInVects(this Vector2 vect, Vector2 pos, Vector2 size, float scale = 1)
    {
        return pos.X * scale < vect.X && pos.Y * scale < vect.Y && vect.X < (pos.X + size.X) * scale &&
               vect.Y < (pos.Y + size.Y) * scale;
    }

    /// <summary>
    /// draw circle using a position and radius
    /// </summary>
    /// <param name="v2">position of circle</param>
    /// <param name="r">radius of circle</param>
    /// <param name="color">color of circle</param>
    public static void DrawCircle(this Vector2 v2, float r, Color? color = null) => DrawCircleV(v2, r, color ?? WHITE);

    /// <summary>
    /// use <see cref="Fix(float)"/> on x and y of vector
    /// </summary>
    /// <param name="v2">the <see cref="Vector2"/> to fix</param>
    /// <returns></returns>
    public static Vector2 FixVector(this Vector2 v2) => new(v2.X.Fix(), v2.Y.Fix());

    /// <summary>
    /// if a float should be fixed using <see cref="Fix(float)"/>
    /// </summary>
    /// <param name="f">float to check</param>
    /// <returns>if a float should be fixed</returns>
    public static bool IsFixable(this float f) => float.IsNaN(f) || float.IsInfinity(f);

    /// <summary>
    /// fixes a float from being NaN/Infinity
    /// </summary>
    /// <param name="f">float to fix</param>
    /// <returns>a float that is not NaN nor Infinity</returns>
    public static float Fix(this float f)
    {
        return f.IsFixable()
            ? float.IsNegative(f)
                ? float.MinValue
                : float.MaxValue
            : f;
    }

    /// <summary>
    /// rotates a <see cref="Vector2"/> by a given <paramref name="degrees"/>  
    /// </summary>
    /// <param name="v2">the <see cref="Vector2"/> to rotate</param>
    /// <param name="degrees">amount to rotate</param>
    /// <returns>the <see cref="Vector2"/> that has been rotated</returns>
    public static Vector2 Rotate(this Vector2 v2, float degrees)
    {
        var sin = (float) Math.Sin(degrees * DegToRad);
        var cos = (float) Math.Cos(degrees * DegToRad);
        var tx = v2.X;
        var ty = v2.Y;

        v2.X = cos * tx - sin * ty;
        v2.Y = sin * tx + cos * ty;
        return v2;
    }

    /// <summary>
    /// Deconstructs a <see cref="Vector2"/> into it's x and y as a tuple
    /// </summary>
    /// <param name="v2">the <see cref="Vector2"/> to deconstruct</param>
    /// <returns>the X and Y of <paramref name="v2"/> as a tuple</returns>
    public static (float x, float y) Deconstruct(this Vector2 v2) => (v2.X, v2.Y);

    public static Vector2 Next(this Random r, Vector2 v21, Vector2 v22)
    {
        var (x1, x2) = v21.X > v22.X ? (v22.X, v21.X) : (v21.X, v22.X);
        var (y1, y2) = v21.Y > v22.Y ? (v22.Y, v21.Y) : (v21.Y, v22.Y);
        return new Vector2(r.Next(x1, x2), r.Next(y1, y2));
    }

    public static string GetString(this Color c) => $"({c.r}, {c.g}, {c.b}, {c.a})";
    public static int Next(this Random r, Range range) => r.Next(range.Start.Value, range.End.Value);

    /// <summary>
    /// draws text at the center of the given position
    /// </summary>
    /// <param name="font">font to draw with</param>
    /// <param name="pos">the center to draw on</param>
    /// <param name="text">the text to draw</param>
    /// <param name="color">the <see cref="Color"/> to draw the text as</param>
    /// <param name="fontSize">the size of the font</param>
    /// <param name="spacing">the spacing of the characters</param>
    /// <param name="origin">origin for rotation</param>
    /// <param name="rotation">rotation for text in degrees</param>
    public static void DrawCenterText(this Font font, string text, Vector2 pos, Color color, float fontSize = 24,
        float spacing = 1.5f, Vector2? origin = null, float rotation = 0)
    {
        var center = pos - MeasureTextEx(font, text, fontSize, spacing) / 2;
        DrawTextPro(font, text, center, origin ?? Zero, rotation, fontSize, spacing, color);
    }

    /// <inheritdoc cref="DrawCenterText"/>
    /// <remarks>the text is wrapped vs <see cref="DrawCenterText"/>, where it isn't</remarks>
    public static void DrawCenterWrapText(this Font font, Rectangle rect, string text, Color color,
        float fontSize = 24, float spacing = 1.5f)
    {
        rect.Pos += -(MeasureTextEx(font, text, fontSize, spacing) / 2);
        DrawTextRec(font, text, rect, color, fontSize, spacing);
    }

    /// <summary>
    /// this draws text in a <see cref="Rectangle"/> and wraps it according to said <see cref="Rectangle"/>
    /// </summary>
    /// <param name="font"><see cref="Font"/> to draw <paramref name="text"/> with</param>
    /// <param name="text">text to draw</param>
    /// <param name="rect">the bounds to draw text around</param>
    /// <param name="fontColor"><see cref="Color"/> of the text</param>
    /// <param name="fontSize">size of the text</param>
    /// <param name="spacing">spacing of the characters</param>
    /// <param name="wordWrap">to wrap words instead of just characters</param>
    public static void DrawTextRec(this Font font, string text, Rectangle rect, Color fontColor,
        float fontSize = 24, float spacing = 1.5f, bool wordWrap = true)
    {
        DrawTextRec(font, text, rect, fontColor, fontSize, spacing, wordWrap, 0, 0, WHITE, WHITE);
    }

    /// <inheritdoc cref="DrawTextRec(Font,string,Rectangle,Color,float,float,bool,int,int,Color,Color)"/>
    /// <param name="selectStart">unknown</param>
    /// <param name="selectLength">unknown</param>
    /// <param name="selectTint">unknown</param>
    /// <param name="selectBackTint">unknown</param>
    /// <remarks>this method was removed from Raylib 4.0, but the source code was still in an <a href="https://www.raylib.com/examples/text/loader.html?name=text_rectangle_bounds">Example</a> so the code was copied and fixed to work in C#</remarks>
    public static unsafe void DrawTextRec(Font font, string text, Rectangle rect, Color tint, float fontSize,
        float spacing, bool wordWrap, int selectStart, int selectLength, Color selectTint, Color selectBackTint)
    {
        var bytes = Encoding.ASCII.GetBytes(text);
        sbyte* sb;
        fixed (byte* b = bytes) sb = (sbyte*) b;
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
                glyphWidth = font.glyphs[index].advanceX == 0
                    ? font.recs[index].width * scaleFactor
                    : font.glyphs[index].advanceX * scaleFactor;

                if (i + 1 < length) glyphWidth += spacing;
            }

            if (!state)
            {
                if (codepoint is ' ' or '\t' or '\n') endLine = i;

                if (textOffsetX + glyphWidth > rect.W)
                {
                    endLine = endLine < 1 ? i : endLine;
                    if (i == endLine) endLine -= codepointByteCount;
                    if (startLine + codepointByteCount == endLine) endLine = i - codepointByteCount;

                    state.Flip();
                }
                else if (i + 1 == length)
                {
                    endLine = i;
                    state.Flip();
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
                    if (!wordWrap && textOffsetX + glyphWidth > rect.W)
                    {
                        textOffsetY += (font.baseSize + font.baseSize / 2) * scaleFactor;
                        textOffsetX = 0;
                    }

                    // When text overflows rectangle height limit, just stop drawing
                    if (textOffsetY + font.baseSize * scaleFactor > rect.H) break;

                    // Draw selection background
                    var isGlyphSelected = false;
                    if (selectStart >= 0 && k >= selectStart && k < selectStart + selectLength)
                    {
                        DrawRectangleRec(
                            new Rectangle(rect.X + textOffsetX - 1, rect.Y + textOffsetY, glyphWidth,
                                font.baseSize * scaleFactor), selectBackTint);
                        isGlyphSelected = true;
                    }

                    // Draw current character glyph
                    if (codepoint != ' ' && codepoint != '\t')
                    {
                        DrawTextCodepoint(font, codepoint, new Vector2(rect.X + textOffsetX, rect.Y + textOffsetY),
                            fontSize, isGlyphSelected ? selectTint : tint);
                    }
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
                    state.Flip();
                }
            }

            textOffsetX += glyphWidth;
        }
    }

    /// <summary>
    /// open a web link with a given url
    /// </summary>
    /// <param name="url">url to open</param>
    /// <remarks>might not work on linux :p</remarks>
    public static void OpenLink(this string url) => Process.Start("explorer.exe", url);

    /// <summary>
    /// yoink some json from the web
    /// </summary>
    /// <param name="site">website url</param>
    /// <returns>the yoinked data</returns>
    public static async Task<dynamic> LoadJsonFromWeb(this string site)
    {
        try
        {
            var client = new HttpClient();
            using var response = await client.GetAsync(site);
            using var content = response.Content;
            return JsonConvert.DeserializeObject(await content.ReadAsStringAsync());
        }
        catch (Exception e)
        {
            Logger.Log(Warning, $"Cannot load json from web:\n{e.Message}\n{e.Source}");
        }

        return null;
    }

    public static float[] MatrixToBuffer(this Matrix4x4 matrix)
    {
        return new[]
        {
            matrix.M11, matrix.M21, matrix.M31, matrix.M41,
            matrix.M12, matrix.M22, matrix.M32, matrix.M42,
            matrix.M13, matrix.M23, matrix.M33, matrix.M43,
            matrix.M14, matrix.M24, matrix.M34, matrix.M44
        };
    }

    /// <summary>
    /// mask a draw action within the bounds of 2 <see cref="Vector2"/>s
    /// </summary>
    /// <param name="pos">top left of mask</param>
    /// <param name="size">size of mask</param>
    /// <param name="draw">draw action to mask</param>
    public static void MaskDraw(this Vector2 pos, Vector2 size, Action draw)
    {
        maskingLayer++;
        BeginScissorMode((int) pos.X, (int) pos.Y, (int) size.X, (int) size.Y);
        draw();
        if (maskingLayer == 1) EndScissorMode();
        maskingLayer--;
    }

    public static bool IsMouseIn(this Raylib_CsLo.Rectangle rect) => rect.IsV2In(Input.MousePosition.currentPosition);
    public static bool IsV2In(this Raylib_CsLo.Rectangle rect, Vector2 pos) => CheckCollisionPointRec(pos, rect);
    public static Vector2 Center(this Vector2 p1, Vector2 p2) => (p1 + p2) / 2;
    public static Vector2 Center(this Vector2[] poses) => poses.Aggregate((v1, v2) => v1 + v2) / poses.Length;
    public static double Distance(this Vector2 p1, Vector2 p2) => Vector2.Distance(p1, p2);
    public static double SquaredDistance(this Vector2 p1, Vector2 p2) => DistanceSquared(p1, p2);
}