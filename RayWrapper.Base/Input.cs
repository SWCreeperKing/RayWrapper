using System.Numerics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace RayWrapper.Base;

public static class Input
{
    public static Vector2 MousePosition { get; private set; }
    public static Vector2 MouseUpdateDelta { get; private set; }
    public static Vector2 MouseWheel { get; private set; }
    public static Vector2 GamepadCursorLeft { get; private set; }
    public static Vector2 GamepadCursorRight { get; private set; }
    public static Gesture LastGesture { get; private set; }
    public static Gesture CurrentGesture { get; private set; }

    public static Func<Vector2, Vector2>? processPositions;
    public static Vector2 maxWindowSize;
    public static int currentGamepadIndex = 0;

    private static Vector2 _rawGamepadCursorLeft;
    private static Vector2 _rawGamepadCursorRight;
    private static bool _hasInit = false;

    public static void Init(Vector2 WindowSize)
    {
        maxWindowSize = WindowSize;
        _hasInit = true;
    }

    public static void Update()
    {
        if (!_hasInit) throw new ArgumentException("Input.Init() has not been called");
        
        var lastMouse = MousePosition;
        MousePosition = ProcessPosition(GetMousePositionRaw());
        MouseUpdateDelta = MousePosition - lastMouse;
        MouseWheel = GetMouseWheelMoveV();

        LastGesture = CurrentGesture;
        CurrentGesture = GetGestureDetected_();

        if (!IsGamepadAvailable(currentGamepadIndex)) return;
        _rawGamepadCursorLeft +=
            new Vector2(GetGamepadAxisMovement(currentGamepadIndex, GamepadAxis.GAMEPAD_AXIS_LEFT_X),
                GetGamepadAxisMovement(currentGamepadIndex, GamepadAxis.GAMEPAD_AXIS_LEFT_Y));
        _rawGamepadCursorRight +=
            new Vector2(GetGamepadAxisMovement(currentGamepadIndex, GamepadAxis.GAMEPAD_AXIS_RIGHT_X),
                GetGamepadAxisMovement(currentGamepadIndex, GamepadAxis.GAMEPAD_AXIS_RIGHT_Y));
        GamepadCursorLeft = ProcessPosition(_rawGamepadCursorLeft);
        GamepadCursorRight = ProcessPosition(_rawGamepadCursorRight);
    }

    public static Vector2 ProcessPosition(Vector2 pos)
    {
        var rawProcess = processPositions?.Invoke(pos) ?? pos;
        return Vector2.Min(Vector2.Zero, Vector2.Max(maxWindowSize, rawProcess));
    }

    public static Vector2 GetMousePositionRaw() => GetMousePosition();

    public static Vector2[] GetTouchPositions(int maxPositions = 0)
    {
        var count = GetTouchPointCount();
        if (maxPositions > 0) count = Math.Min(maxPositions, count);
        return Enumerable.Range(0, count).Select(GetTouchPosition).Select(ProcessPosition).ToArray();
    }
}