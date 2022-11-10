using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.Primitives;
using ZimonIsHimUtils.ExtensionMethods;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.Base.GameObject;

public abstract class GameObject : TypeRegister<IGameObject>, IGameObject
{
    public static long gameObjects;
    
    public Actionable<bool> isVisible = true;

    public Vector2 Position
    {
        get => GetPosition();
        set => UpdatePosition(value);
    }

    public Vector2 Size
    {
        get => GetSize();
        set => UpdateSize(value);
    }

    public bool updateReturnIfNonVis;

    protected Vector2 pos;
    protected Vector2 size;

    private Vector2 _freezeV2 = Vector2.Zero;

    public GameObject() => gameObjects++;

    public void Update(float dt)
    {
        if (updateReturnIfNonVis && !isVisible) return;
        UpdateCall(dt);
        UpdateReg(dt);
    }

    public void Render()
    {
        if (!isVisible) return;
        RenderCall();
        RenderReg();
    }

    public void Dispose()
    {
        DisposeCall();
        DisposeReg();
    }

    protected virtual void UpdateCall(float dt)
    {
    }

    protected virtual void RenderCall()
    {
    }

    protected virtual void DisposeCall()
    {
    }
    
    protected virtual Vector2 GetPosition() => pos;
    protected virtual Vector2 GetSize() => size;

    protected virtual void UpdatePosition(Vector2 newPos) => pos = newPos;
    protected virtual void UpdateSize(Vector2 newSize) => size = newSize;

    public void UpdateReg(float dt) => register.Each(o => o.Update(dt));
    public void RenderReg() => register.Each(o => o.Render());
    public void DisposeReg() => register.Each(o => o.Dispose());
    public Raylib_CsLo.Rectangle GetRawRect() => new(Position.X, Position.Y, Size.X, Size.Y);
    public Rectangle GetRect() => new(Position, Size);
    public void ReserveV2() => _freezeV2 = new Vector2(Position.X, Position.Y);
    public Vector2 GetReserveV2() => _freezeV2;
    public void SetPositionAsReserveV2() => Position = _freezeV2;
    public virtual MouseCursor GetOccupiedCursor() => MouseCursor.MOUSE_CURSOR_RESIZE_ALL;

    ~GameObject() => gameObjects--;
}