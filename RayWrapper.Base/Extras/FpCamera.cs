using System.Numerics;
using Raylib_CsLo;
using static Raylib_CsLo.KeyboardKey;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.RlGl;
using static RayWrapper.Base.Extras.FpCamera.CameraControls;

namespace RayWrapper.Base.Extras;

//https://github.com/NotNotTech/Raylib-Extras-CsLo/blob/main/raylibExtras-cs/cameras/rlFPCamera/rlFPCamera.cs
public class FpCamera
{
    private const float Pi180 = MathF.PI / 180f;
    private const float ViewBobbleDampen = 8f;

    public enum CameraControls
    {
        MoveFront = 0,
        MoveBack,
        MoveRight,
        MoveLeft,
        MoveUp,
        MoveDown,
        TurnLeft,
        TurnRight,
        TurnUp,
        TurnDown,
        Sprint,
    }

    public Camera3D Camera => _camera;

    public Dictionary<CameraControls, KeyboardKey> keybindings = new()
    {
        { MoveFront, KEY_W }, { MoveBack, KEY_S }, { MoveLeft, KEY_A }, { MoveRight, KEY_D }, { MoveUp, KEY_SPACE },
        { MoveDown, KEY_LEFT_SHIFT }, { TurnDown, KEY_DOWN }, { TurnLeft, KEY_LEFT }, { TurnRight, KEY_RIGHT },
        { TurnUp, KEY_UP }, { Sprint, KEY_LEFT_CONTROL }
    };

    public Vector3 moveSpeed = Vector3.One;
    public Vector2 turnSpeed = new(90);
    public float mouseSensitivity = 600;

    public float minViewY = -65f;
    public float maxViewY = 89f;
    public float viewBobbleFreq = 0f;
    public float viewBobbleMag = .02f;

    public float viewBobbleWaveMag = .002f;
    public bool useMouseX = true;
    public bool useMouseY = true;
    public bool useKeyboard = true;
    public bool useController = true;
    public bool controllerId = false;
    public bool hideCursor = true;

    /// <summary>clipping plane: must use BeginMode3D and EndMode3D on the camera object for clipping planes to work</summary>
    public double nearPlane = .01f;

    /// <summary>clipping plane: must use BeginMode3D and EndMode3D on the camera object for clipping planes to work</summary>
    public double farPlane = 1e3;

    protected Vector3 position = Vector3.Zero;
    protected Vector2 fov = Vector2.Zero;
    protected Vector2 prevMousePos = Vector2.Zero;
    protected Vector2 angle = Vector2.Zero; // Camera angle in plane XZ
    protected float targetDist; // Camera distance from position to target
    protected float playerEyesPos = 0.5f; // Player eyes position from ground (in meters)
    protected float currentBobble;
    protected bool focused = true;

    private Camera3D _camera;

    public PositionCallback? validateCamPos = null;

    public delegate bool PositionCallback(FpCamera camera, Vector3 newPosition, Vector3 oldPosition);

    public FpCamera() => prevMousePos = GetMousePosition();

    public void Setup(float fovY, Vector3 pos)
    {
        _camera.position = position = new Vector3(pos.X, pos.Y, pos.Z);
        _camera.position.Y += playerEyesPos;
        _camera.target = _camera.position + new Vector3(0, 0, 1);
        _camera.up = new Vector3(0, 1, 0);
        _camera.fovy = fovY;
        _camera.projection_ = CameraProjection.CAMERA_PERSPECTIVE;
        focused = IsWindowFocused();

        if (hideCursor && focused && (useMouseX || useMouseY)) DisableCursor();
        targetDist = 1;
        ViewResized();
    }

    public void ViewResized()
    {
        var height = (float) GetScreenHeight();
        fov.Y = _camera.fovy;
        if (height != 0) fov.X = fov.Y * (GetScreenWidth() / height);
    }

    protected float GetSpeedForAxis(CameraControls axis, float speed)
    {
        if (!useKeyboard || !keybindings.ContainsKey(axis)) return 0;

        var key = keybindings[axis];
        if (key == KEY_NULL) return 0;

        var factor = 1f;
        if (IsKeyDown(keybindings[Sprint])) factor = 2;
        if (IsKeyDown(key)) return speed * GetFrameTime() * factor;

        return 0f;
    }

    public void Update()
    {
        if (hideCursor && IsWindowFocused() != focused && (useMouseX || useMouseY))
        {
            focused = IsWindowFocused();
            if (focused) DisableCursor();
            else EnableCursor();
        }

        var curMousePos = GetMousePosition();
        var mousePositionDelta = curMousePos - prevMousePos;
        prevMousePos = curMousePos;

        // Mouse movement detection
        var mouseWheelMove = GetMouseWheelMove();

        // Keys input detection
        var directions = new Dictionary<CameraControls, float>
        {
            { MoveFront, GetSpeedForAxis(MoveFront, moveSpeed.Z) },
            { MoveBack, GetSpeedForAxis(MoveBack, moveSpeed.Z) },
            { MoveRight, GetSpeedForAxis(MoveRight, moveSpeed.X) },
            { MoveLeft, GetSpeedForAxis(MoveLeft, moveSpeed.X) }, { MoveUp, GetSpeedForAxis(MoveUp, moveSpeed.Y) },
            { MoveDown, GetSpeedForAxis(MoveDown, moveSpeed.Y) },
        };

        var forward = _camera.target - _camera.position;
        forward.Y = 0;
        forward = Vector3.Normalize(forward);

        var right = new Vector3(forward.Z * -1f, 0, forward.X);
        var oldPosition = position;

        position += (forward * (directions[MoveFront] - directions[MoveBack]));
        position += (right * (directions[MoveRight] - directions[MoveLeft]));
        position.Y += directions[MoveUp] - directions[MoveDown];

        // let someone modify the projected position
        validateCamPos?.Invoke(this, position, oldPosition);

        // Camera orientation calculation
        var turnRotation = GetSpeedForAxis(TurnRight, turnSpeed.X) - GetSpeedForAxis(TurnLeft, turnSpeed.X);
        var tiltRotation = GetSpeedForAxis(TurnUp, turnSpeed.Y) - GetSpeedForAxis(TurnDown, turnSpeed.Y);

        if (turnRotation != 0) angle.X -= turnRotation * Pi180;
        else if (useMouseX && focused) angle.X += mousePositionDelta.X / -mouseSensitivity;

        if (tiltRotation != 0) angle.Y += tiltRotation * Pi180;
        else if (useMouseY && focused) angle.Y += mousePositionDelta.Y / -mouseSensitivity;

        // Angle clamp
        if (angle.Y < minViewY * Pi180) angle.Y = minViewY * Pi180;
        else if (angle.Y > maxViewY * Pi180) angle.Y = maxViewY * Pi180;

        // Recalculate camera target considering translation and rotation
        var target = RayMath.Vector3Transform(new Vector3(0, 0, 1),
            RayMath.MatrixRotateXYZ(new Vector3(angle.Y, -angle.X, 0)));

        _camera.position = position;
        var eyeOffset = playerEyesPos;

        if (viewBobbleFreq > 0)
        {
            var swingDelta = MathF.Max(MathF.Abs(directions[MoveFront] - directions[MoveBack]),
                MathF.Abs(directions[MoveRight] - directions[MoveLeft]));

            // If movement detected (some key pressed), increase swinging
            currentBobble += swingDelta * viewBobbleFreq;

            eyeOffset -= MathF.Sin(currentBobble / ViewBobbleDampen) * viewBobbleMag;
            _camera.up.Z = -(_camera.up.X = MathF.Sin(currentBobble / (ViewBobbleDampen * 2)) * currentBobble);
        }
        else currentBobble = _camera.up.X = _camera.up.Z = 0;

        _camera.position.Y += eyeOffset;
        _camera.target = _camera.position + target;
    }

    public float GetFovX() => fov.X;
    public Vector3 GetCameraPosition() => position;

    public void SetCameraPosition(Vector3 pos)
    {
        position = pos;
        var forward = _camera.target - _camera.position;
        _camera.target = (_camera.position = position) + forward;
    }

    public Vector2 GetViewAngles() => RayMath.Vector2Scale(angle, Pi180);

    // start drawing using the camera, with near/far plane support
    public void BeginMode3D()
    {
        //Raylib.BeginMode3D(Camera);
        var aspect = GetScreenWidth() / (float) GetScreenHeight();

        rlDrawRenderBatchActive(); // Draw Buffers (Only OpenGL 3+ and ES2)
        rlMatrixMode(RL_PROJECTION); // Switch to projection matrix
        rlPushMatrix(); // Save previous matrix, which contains the settings for the 2d ortho projection
        rlLoadIdentity(); // Reset current matrix (projection)

        switch (_camera.projection_)
        {
            case CameraProjection.CAMERA_PERSPECTIVE:
            {
                // Setup perspective projection
                var top = RL_CULL_DISTANCE_NEAR * MathF.Tan(_camera.fovy * .5f * Pi180);
                var right = top * aspect;

                rlFrustum(-right, right, -top, top, nearPlane, farPlane);
                break;
            }
            case CameraProjection.CAMERA_ORTHOGRAPHIC:
            {
                // Setup orthographic projection
                var top = _camera.fovy / 2f;
                var right = top * aspect;

                rlOrtho(-right, right, -top, top, nearPlane, farPlane);
                break;
            }
        }

        // NOTE: zNear and zFar values are important when computing depth buffer values

        rlMatrixMode(RL_MODELVIEW); // Switch back to modelview matrix
        rlLoadIdentity(); // Reset current matrix (modelview)

        // Setup Camera view
        var matView = RayMath.MatrixLookAt(_camera.position, _camera.target, _camera.up);

        // TODO, fix this!
        //float[] mat = rlMathUtils.MatrixToBuffer(matView);
        rlMultMatrixf(matView); // Multiply modelview matrix by view matrix (camera)

        rlEnableDepthTest(); // Enable DEPTH_TEST for 3D
    }

    // end drawing with the camera
    public void EndMode3D() => Raylib.EndMode3D();
}