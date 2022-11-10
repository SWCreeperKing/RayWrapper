/*******************************************************************************************
*
*   raylib-extras [ImGui] example - Simple Integration
*
*	This is a simple ImGui Integration
*	It is done using C++ but with C style code
*	It can be done in C as well if you use the C ImGui wrapper
*	https://github.com/cimgui/cimgui
*
*   Copyright (c) 2021 Jeffery Myers
*
********************************************************************************************/
/*******************************************************************************************
 *
 * Modified for Raylib-cslo
 * 
********************************************************************************************/

using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET;
using Raylib_CsLo;
using RayWrapper.Base.GameBox;
using static RayWrapper.Base.GameBox.AttributeManager;

namespace RayWrapper.Imgui;

public static class RlImgui
{
    public static readonly Vector2 V2MaxValue = new(float.MaxValue, float.MaxValue);
    public static readonly Vector2 V2MinValue = new(float.MinValue, float.MinValue);
    public static readonly Vector3 V3MaxValue = new(float.MaxValue, float.MaxValue, float.MaxValue);
    public static readonly Vector3 V3MinValue = new(float.MinValue, float.MinValue, float.MinValue);
    public static readonly Vector4 V4MaxValue = new(float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue);
    public static readonly Vector4 V4MinValue = new(float.MinValue, float.MinValue, float.MinValue, float.MinValue);

    public static nint imGuiContext = nint.Zero;
    private static ImGuiMouseCursor _currentMouseCursor = ImGuiMouseCursor.COUNT;
    private static Dictionary<ImGuiMouseCursor, MouseCursor> _mouseCursorMap;
    private static KeyboardKey[] _keyEnumMap;
    private static Texture _fontTexture;

    [GameBoxWedge(PlacerType.AfterInit)] public static void Setup() => Setup(true);

    public static void Setup(bool darkTheme)
    {
        _mouseCursorMap = new Dictionary<ImGuiMouseCursor, MouseCursor>();
        _keyEnumMap = Enum.GetValues(typeof(KeyboardKey)) as KeyboardKey[];

        _fontTexture.id = 0;

        BeginInitImGui();

        if (darkTheme) ImGui.StyleColorsDark();
        else ImGui.StyleColorsLight();

        EndInitImGui();
    }

    public static void BeginInitImGui() => imGuiContext = ImGui.CreateContext();

    private static void SetupMouseCursors()
    {
        _mouseCursorMap.Clear();
        _mouseCursorMap[ImGuiMouseCursor.Arrow] = MouseCursor.MOUSE_CURSOR_ARROW;
        _mouseCursorMap[ImGuiMouseCursor.TextInput] = MouseCursor.MOUSE_CURSOR_IBEAM;
        _mouseCursorMap[ImGuiMouseCursor.Hand] = MouseCursor.MOUSE_CURSOR_POINTING_HAND;
        _mouseCursorMap[ImGuiMouseCursor.ResizeAll] = MouseCursor.MOUSE_CURSOR_RESIZE_ALL;
        _mouseCursorMap[ImGuiMouseCursor.ResizeEW] = MouseCursor.MOUSE_CURSOR_RESIZE_EW;
        _mouseCursorMap[ImGuiMouseCursor.ResizeNESW] = MouseCursor.MOUSE_CURSOR_RESIZE_NESW;
        _mouseCursorMap[ImGuiMouseCursor.ResizeNS] = MouseCursor.MOUSE_CURSOR_RESIZE_NS;
        _mouseCursorMap[ImGuiMouseCursor.ResizeNWSE] = MouseCursor.MOUSE_CURSOR_RESIZE_NWSE;
        _mouseCursorMap[ImGuiMouseCursor.NotAllowed] = MouseCursor.MOUSE_CURSOR_NOT_ALLOWED;
    }

    public static unsafe void ReloadFonts()
    {
        ImGui.SetCurrentContext(imGuiContext);
        var io = ImGui.GetIO();

        io.Fonts.GetTexDataAsRGBA32(out byte* pixels, out var width, out var height, out var bytesPerPixel);

        var image = new Image
        {
            data = pixels,
            width = width,
            height = height,
            mipmaps = 1,
            format = (int) PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8,
        };

        _fontTexture = Raylib.LoadTextureFromImage(image);

        io.Fonts.SetTexID(new nint(_fontTexture.id));
    }

    public static void EndInitImGui()
    {
        SetupMouseCursors();

        ImGui.SetCurrentContext(imGuiContext);
        var io = ImGui.GetIO();

        io.Fonts.AddFontDefault();
        io.KeyMap[(int) ImGuiKey.Tab] = (int) KeyboardKey.KEY_TAB;
        io.KeyMap[(int) ImGuiKey.LeftArrow] = (int) KeyboardKey.KEY_LEFT;
        io.KeyMap[(int) ImGuiKey.RightArrow] = (int) KeyboardKey.KEY_RIGHT;
        io.KeyMap[(int) ImGuiKey.UpArrow] = (int) KeyboardKey.KEY_UP;
        io.KeyMap[(int) ImGuiKey.DownArrow] = (int) KeyboardKey.KEY_DOWN;
        io.KeyMap[(int) ImGuiKey.PageUp] = (int) KeyboardKey.KEY_PAGE_UP;
        io.KeyMap[(int) ImGuiKey.PageDown] = (int) KeyboardKey.KEY_PAGE_DOWN;
        io.KeyMap[(int) ImGuiKey.Home] = (int) KeyboardKey.KEY_HOME;
        io.KeyMap[(int) ImGuiKey.End] = (int) KeyboardKey.KEY_END;
        io.KeyMap[(int) ImGuiKey.Delete] = (int) KeyboardKey.KEY_DELETE;
        io.KeyMap[(int) ImGuiKey.Backspace] = (int) KeyboardKey.KEY_BACKSPACE;
        io.KeyMap[(int) ImGuiKey.Enter] = (int) KeyboardKey.KEY_ENTER;
        io.KeyMap[(int) ImGuiKey.Escape] = (int) KeyboardKey.KEY_ESCAPE;
        io.KeyMap[(int) ImGuiKey.Space] = (int) KeyboardKey.KEY_SPACE;
        io.KeyMap[(int) ImGuiKey.A] = (int) KeyboardKey.KEY_A;
        io.KeyMap[(int) ImGuiKey.C] = (int) KeyboardKey.KEY_C;
        io.KeyMap[(int) ImGuiKey.V] = (int) KeyboardKey.KEY_V;
        io.KeyMap[(int) ImGuiKey.X] = (int) KeyboardKey.KEY_X;
        io.KeyMap[(int) ImGuiKey.Y] = (int) KeyboardKey.KEY_Y;
        io.KeyMap[(int) ImGuiKey.Z] = (int) KeyboardKey.KEY_Z;

        ReloadFonts();
    }

    private static void NewFrame()
    {
        var io = ImGui.GetIO();

        if (Raylib.IsWindowFullscreen())
        {
            var monitor = Raylib.GetCurrentMonitor();
            io.DisplaySize = new Vector2(Raylib.GetMonitorWidth(monitor), Raylib.GetMonitorHeight(monitor));
        }
        else io.DisplaySize = GameBox.WindowSize;

        io.DisplayFramebufferScale = new Vector2(1, 1);
        io.DeltaTime = Raylib.GetFrameTime();

        io.KeyCtrl = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL) ||
                     Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL);
        io.KeyShift = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT);
        io.KeyAlt = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_ALT) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_ALT);
        io.KeySuper = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SUPER) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SUPER);

        if (io.WantSetMousePos) Raylib.SetMousePosition((int) io.MousePos.X, (int) io.MousePos.Y);
        else io.MousePos = Raylib.GetMousePosition();

        io.MouseDown[0] = Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT);
        io.MouseDown[1] = Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT);
        io.MouseDown[2] = Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_MIDDLE);

        if (Raylib.GetMouseWheelMove() > 0) io.MouseWheel += 1;
        else if (Raylib.GetMouseWheelMove() < 0) io.MouseWheel -= 1;

        if ((io.ConfigFlags & ImGuiConfigFlags.NoMouseCursorChange) != 0) return;

        var imguiCursor = ImGui.GetMouseCursor();
        if (imguiCursor == _currentMouseCursor && !io.MouseDrawCursor) return;

        _currentMouseCursor = imguiCursor;
        if (io.MouseDrawCursor || imguiCursor == ImGuiMouseCursor.None) Raylib.HideCursor();
        else
        {
            Raylib.ShowCursor();

            if ((io.ConfigFlags & ImGuiConfigFlags.NoMouseCursorChange) != 0) return;

            Raylib.SetMouseCursor(!_mouseCursorMap.ContainsKey(imguiCursor)
                ? MouseCursor.MOUSE_CURSOR_DEFAULT
                : _mouseCursorMap[imguiCursor]);
        }
    }


    private static void FrameEvents()
    {
        var io = ImGui.GetIO();

        foreach (var key in _keyEnumMap) io.KeysDown[(int) key] = Raylib.IsKeyDown(key);

        var pressed = (uint) Raylib.GetCharPressed();
        while (pressed != 0)
        {
            io.AddInputCharacter(pressed);
            pressed = (uint) Raylib.GetCharPressed();
        }
    }

    [GameBoxWedge(PlacerType.BeforeRender)]
    public static void Begin()
    {
        ImGui.SetCurrentContext(imGuiContext);

        NewFrame();
        FrameEvents();
        ImGui.NewFrame();
    }

    private static void EnableScissor(float x, float y, float width, float height)
    {
        RlGl.rlEnableScissorTest();
        RlGl.rlScissor((int) x, Raylib.GetScreenHeight() - (int) (y + height), (int) width, (int) height);
    }

    private static void TriangleVert(ImDrawVertPtr idx_vert)
    {
        var c = ImGui.ColorConvertU32ToFloat4(idx_vert.col);

        RlGl.rlColor4f(c.X, c.Y, c.Z, c.W);
        RlGl.rlTexCoord2f(idx_vert.uv.X, idx_vert.uv.Y);
        RlGl.rlVertex2f(idx_vert.pos.X, idx_vert.pos.Y);
    }

    private static void RenderTriangles(uint count, uint indexStart, ImVector<ushort> indexBuffer,
        ImPtrVector<ImDrawVertPtr> vertBuffer, nint texturePtr)
    {
        if (count < 3) return;

        uint textureId = 0;
        if (texturePtr != nint.Zero) textureId = (uint) texturePtr.ToInt32();

        RlGl.rlBegin(RlGl.RL_TRIANGLES);
        RlGl.rlSetTexture(textureId);

        for (var i = 0; i <= count - 3; i += 3)
        {
            if (RlGl.rlCheckRenderBatchLimit(3))
            {
                RlGl.rlBegin(RlGl.RL_TRIANGLES);
                RlGl.rlSetTexture(textureId);
            }

            TriangleVert(vertBuffer[indexBuffer[(int) indexStart + i]]);
            TriangleVert(vertBuffer[indexBuffer[(int) indexStart + i + 1]]);
            TriangleVert(vertBuffer[indexBuffer[(int) indexStart + i + 2]]);
        }

        RlGl.rlEnd();
    }

    private delegate void Callback(ImDrawListPtr list, ImDrawCmdPtr cmd);

    private static void RenderData()
    {
        RlGl.rlDrawRenderBatchActive();
        RlGl.rlDisableBackfaceCulling();

        var data = ImGui.GetDrawData();

        for (var l = 0; l < data.CmdListsCount; l++)
        {
            var commandList = data.CmdListsRange[l];

            for (var cmdIndex = 0; cmdIndex < commandList.CmdBuffer.Size; cmdIndex++)
            {
                var cmd = commandList.CmdBuffer[cmdIndex];

                EnableScissor(cmd.ClipRect.X - data.DisplayPos.X, cmd.ClipRect.Y - data.DisplayPos.Y,
                    cmd.ClipRect.Z - (cmd.ClipRect.X - data.DisplayPos.X),
                    cmd.ClipRect.W - (cmd.ClipRect.Y - data.DisplayPos.Y));
                if (cmd.UserCallback != nint.Zero)
                {
                    var cb = Marshal.GetDelegateForFunctionPointer<Callback>(cmd.UserCallback);
                    cb(commandList, cmd);
                    continue;
                }

                RenderTriangles(cmd.ElemCount, cmd.IdxOffset, commandList.IdxBuffer, commandList.VtxBuffer,
                    cmd.TextureId);

                RlGl.rlDrawRenderBatchActive();
            }
        }

        RlGl.rlSetTexture(0);
        RlGl.rlDisableScissorTest();
        RlGl.rlEnableBackfaceCulling();
    }

    [GameBoxWedge(PlacerType.AfterRender)]
    public static void End()
    {
        ImGui.SetCurrentContext(imGuiContext);
        ImGui.Render();
        RenderData();
    }

    public static void Shutdown() => Raylib.UnloadTexture(_fontTexture);

    public static void Image(Texture image)
    {
        ImGui.Image(new nint(image.id), new Vector2(image.width, image.height));
    }

    public static void ImageSize(Texture image, int width, int height)
    {
        ImGui.Image(new nint(image.id), new Vector2(width, height));
    }

    public static void ImageSize(Texture image, Vector2 size) => ImGui.Image(new nint(image.id), size);

    public static void ImageRect(Texture image, int destWidth, int destHeight, Rectangle sourceRect)
    {
        var uv0 = new Vector2();
        var uv1 = new Vector2();

        if (sourceRect.width < 0)
        {
            uv0.X = -(sourceRect.x / image.width);
            uv1.X = (uv0.X - Math.Abs(sourceRect.width) / image.width);
        }
        else
        {
            uv0.X = sourceRect.x / image.width;
            uv1.X = uv0.X + sourceRect.width / image.width;
        }

        if (sourceRect.height < 0)
        {
            uv0.Y = -(sourceRect.y / image.height);
            uv1.Y = (uv0.Y - Math.Abs(sourceRect.height) / image.height);
        }
        else
        {
            uv0.Y = sourceRect.y / image.height;
            uv1.Y = uv0.Y + sourceRect.height / image.height;
        }

        ImGui.Image(new nint(image.id), new Vector2(destWidth, destHeight), uv0, uv1);
    }

    public static Vector4 ToV4(this Color color) => new Vector4(color.r, color.g, color.b, color.a) / 255f;
    public static Vector3 ToV3(this Color color) => new Vector3(color.r, color.g, color.b) / 255f;
    public static uint ToUint(this Vector4 color) => ImGui.ColorConvertFloat4ToU32(color);
    public static uint ToUint(this Color color) => color.ToV4().ToUint();
    public static Color ToColor(this Vector3 color) => new((short) color.X, (short) color.Y, (short) color.Z, 255);

    public static Color ToColor(this Vector4 color)
    {
        return new((short) color.X, (short) color.Y, (short) color.Z, (short) color.W);
    }

    public static Vector2 MeasureText(this string text) => ImGui.CalcTextSize(text);

    public static Vector2 MeasureText(this string text, bool hideAfterDoubleHash)
    {
        return ImGui.CalcTextSize(text, hideAfterDoubleHash);
    }

    public static Vector2 MeasureText(this string text, bool hideAfterDoubleHash, float wrapWidth)
    {
        return ImGui.CalcTextSize(text, hideAfterDoubleHash, wrapWidth);
    }

    public static Vector2 MeasureText(this string text, float wrapWidth) => ImGui.CalcTextSize(text, wrapWidth);
    public static Vector2 MeasureText(this string text, int start) => ImGui.CalcTextSize(text, start);

    public static Vector2 MeasureText(this string text, int start, bool hideAfterDoubleHash)
    {
        return ImGui.CalcTextSize(text, start, hideAfterDoubleHash);
    }

    public static Vector2 MeasureText(this string text, int start, float wrapWidth)
    {
        return ImGui.CalcTextSize(text, start, wrapWidth);
    }

    public static Vector2 MeasureText(this string text, int start, int length)
    {
        return ImGui.CalcTextSize(text, start, length);
    }

    public static Vector2 MeasureText(this string text, int start, int length, bool hideAfterDoubleHash)
    {
        return ImGui.CalcTextSize(text, start, length, hideAfterDoubleHash);
    }

    public static Vector2 MeasureText(this string text, int start, int length, bool hideAfterDoubleHash,
        float wrapWidth)
    {
        return ImGui.CalcTextSize(text, start, length, hideAfterDoubleHash, wrapWidth);
    }

    public static Vector2 MeasureText(this string text, int start, int length, float wrapWidth)
    {
        return ImGui.CalcTextSize(text, start, length, wrapWidth);
    }

    public static void SetScale(float scale = 1) => ImGui.GetIO().FontGlobalScale = scale;

    public static void DrawBezierCurve(this ImDrawListPtr ptr, Vector2 pos1, Vector2 pos2, uint color, float thickness)
    {
        Vector2 p1 = new(pos1.X, pos2.Y);
        Vector2 p2 = new(pos2.X, pos1.Y);
        ptr.AddBezierCubic(pos1, p2, p1, pos2, color, thickness);
    }
}