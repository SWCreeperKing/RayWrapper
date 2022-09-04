﻿using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Var_Interfaces;
using static RayWrapper.GameBox;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Vars;

public abstract class GameObject : GameObjReg, IGameObject
{
    public Actionable<bool> isVisible = true;

    public Vector2 Position
    {
        get => GetPosition();
        set => UpdatePosition(value);
    }

    public Vector2 Size
    {
        get => GetSize();
        set => UpdatedSize(value);
    }

    public bool updateReturnIfNonVis;

    protected Vector2 pos;
    protected Vector2 size;

    private Vector2 _freezeV2 = Vector2.Zero;

    public GameObject() => gameObjects++;

    public void Update()
    {
        if (updateReturnIfNonVis && !isVisible) return;
        UpdateCall();
        UpdateReg();
    }

    public void Render()
    {
        if (!isVisible) return;
        RenderCall();
        RenderReg();
    }

    protected virtual void UpdateCall()
    {
    }

    protected virtual void RenderCall()
    {
    }

    protected virtual Vector2 GetPosition() => pos;
    protected virtual Vector2 GetSize() => size;

    protected virtual void UpdatePosition(Vector2 newPos) => pos = newPos;
    protected virtual void UpdatedSize(Vector2 newSize) => size = newSize;

    public Rectangle GetRect() => AssembleRectFromVec(Position, Size);
    public void ReserveV2() => _freezeV2 = new Vector2(Position.X, Position.Y);
    public Vector2 GetReserveV2() => _freezeV2;
    public void SetPositionAsReserveV2() => Position = _freezeV2;
    public virtual MouseCursor GetOccupiedCursor() => MouseCursor.MOUSE_CURSOR_RESIZE_ALL;

    ~GameObject() => gameObjects--;
}