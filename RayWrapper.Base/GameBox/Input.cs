using System.Numerics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace RayWrapper.Base.GameBox;

public static class Input
{
    public enum PositionTypes
    {
        Mouse, GamepadLeft, GamepadRight
    }

    public static bool IsMouseOccupied => MouseOccupier is not null;
    public static object MouseOccupier = null;
    public static Positional MousePosition { get; private set; }
    public static Vector2 MouseWheel { get; private set; }
    public static Positional GamepadCursorLeft { get; private set; }
    public static Positional GamepadCursorRight { get; private set; }
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
        
        MousePosition = new Positional(ProcessPosition(GetMousePositionRaw()), MousePosition.currentPosition);
        MouseWheel = GetMouseWheelMoveV();

        LastGesture = CurrentGesture;
        CurrentGesture = GetGestureDetected_();

        if (!IsGamepadAvailable(currentGamepadIndex)) return;
        var beforeLeft = _rawGamepadCursorLeft;
        var beforeRight = GamepadCursorRight.currentPosition;
        _rawGamepadCursorLeft +=
            new Vector2(GetGamepadAxisMovement(currentGamepadIndex, GamepadAxis.GAMEPAD_AXIS_LEFT_X),
                GetGamepadAxisMovement(currentGamepadIndex, GamepadAxis.GAMEPAD_AXIS_LEFT_Y));
        _rawGamepadCursorRight +=
            new Vector2(GetGamepadAxisMovement(currentGamepadIndex, GamepadAxis.GAMEPAD_AXIS_RIGHT_X),
                GetGamepadAxisMovement(currentGamepadIndex, GamepadAxis.GAMEPAD_AXIS_RIGHT_Y));
        GamepadCursorLeft = new Positional(ProcessPosition(_rawGamepadCursorLeft), beforeLeft);
        GamepadCursorRight = new Positional(ProcessPosition(_rawGamepadCursorLeft), beforeRight);
    }

    public static bool IsKeyCombo(params KeyboardKey[] keys) => keys.All(IsKeyDown);
    
    public static Vector2 ProcessPosition(Vector2 pos)
    {
        var rawProcess = processPositions?.Invoke(pos) ?? pos;
        return Vector2.Max(Vector2.Zero, Vector2.Min(maxWindowSize, rawProcess));
    }

    public static Vector2 GetMousePositionRaw() => GetMousePosition();

    public static Vector2[] GetTouchPositions(int maxPositions = 0)
    {
        var count = GetTouchPointCount();
        if (maxPositions > 0) count = Math.Min(maxPositions, count);
        return Enumerable.Range(0, count).Select(GetTouchPosition).Select(ProcessPosition).ToArray();
    }
}

public readonly struct Positional
{
    public readonly Vector2 currentPosition;
    public readonly Vector2 lastPosition;
    public readonly Vector2 delta;

    public Positional(Vector2 currentPosition, Vector2 lastPosition)
    {
        this.currentPosition = currentPosition;
        this.lastPosition = lastPosition;
        delta = currentPosition - lastPosition;
    }

    public override string ToString() => $"[{currentPosition}-{lastPosition}={delta}]";
}