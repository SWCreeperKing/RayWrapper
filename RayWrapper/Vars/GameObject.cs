using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Var_Interfaces;
using static Raylib_CsLo.Raylib;
using static RayWrapper.GameBox;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Vars;

public abstract class GameObject : GameObjReg, IGameObject
{
    public Actionable<bool> isVisible = true;
    public abstract Vector2 Position { get; set; }
    public abstract Vector2 Size { get; }

    public float FullLength => Position.X + Size.X;
    public float FullHeight => Position.Y + Size.Y;
    public Actionable<string>? debugString = null;
    public bool updateReturnIfNonVis;

    private Rectangle _rect = Zero;
    private Vector2 _freezeV2 = Vector2.Zero;

    public GameObject() => gameObjects++;

    public void Update()
    {
        _rect = AssembleRectFromVec(Position, Size);
        if (debugContext == this && _rect.IsMouseIn() && IsMouseButtonPressed(MOUSE_MIDDLE_BUTTON) &&
            isDebugTool)
        {
            debugContext = null;
        }
        else if (_rect.IsMouseIn() && IsMouseButtonPressed(MOUSE_MIDDLE_BUTTON) &&
                 isDebugTool)
        {
            debugContext = this;
        }
        if (updateReturnIfNonVis && !isVisible) return;
        UpdateCall();
        UpdateReg();
    }

    public void Render()
    {
        if (!isVisible) return;
        RenderCall();
        RenderReg();
        if (isDebugTool) DrawDebugHitbox();
    }

    protected abstract void UpdateCall();
    protected abstract void RenderCall();
    protected virtual void DrawDebugHitbox() => _rect.DrawHallowRect(debugContext == this ? GREEN : RED);

    public Rectangle GetDebugRect() => _rect;
    public void ReserveV2() => _freezeV2 = new Vector2(Position.X, Position.Y);
    public Vector2 GetReserveV2() => _freezeV2;
    public void SetPositionAsReserveV2() => Position = _freezeV2;
    public virtual MouseCursor GetOccupiedCursor() => MouseCursor.MOUSE_CURSOR_RESIZE_ALL;

    ~GameObject() => gameObjects--;
}