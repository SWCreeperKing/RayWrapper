using System.Drawing;
using System.Numerics;
using ImGuiNET;
using Raylib_CsLo;
using RayWrapper.Base.GameObject;
using ZimonIsHimUtils.ExtensionMethods;
using static ImGuiNET.ImGuiWindowFlags;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.Imgui.Widgets;

public abstract class WindowBase : TypeRegister<IGameObject>, IGameObject
{
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
    
    public static long windows;
    
    public bool closable;
    public bool isOpen = true;
    public ImGuiWindowFlags configFlags;

    protected Vector2 pos;
    protected Vector2 size;

    private Vector2 _freezeV2 = Vector2.Zero;
    private string _name;

    protected WindowBase(string name, ImGuiWindowFlags configFlags = AlwaysAutoResize)
    {
        _name = name;
        this.configFlags = configFlags;
        windows++;
    }

    public void Update(float dt)
    {
        if (!isOpen && closable) return;
        WindowUpdate(dt);
        UpdateReg(dt);
    }

    public void Render()
    {
        if (!isOpen && closable) return;
        var begin = closable ? ImGui.Begin(_name, ref isOpen, configFlags) : ImGui.Begin(_name, configFlags);
        if (!begin) return;

        WindowRender();
        RenderReg();
        ImGui.End();
    }

    public void Dispose()
    {
        WindowDispose();
        DisposeReg();
    }

    protected virtual void WindowUpdate(float dt)
    {
    }

    protected virtual void WindowRender()
    {
    }

    protected virtual void WindowDispose()
    {
    }
    
    protected virtual Vector2 GetPosition() => pos;
    protected virtual Vector2 GetSize() => size;

    protected virtual void UpdatePosition(Vector2 newPos) => pos = newPos;
    protected virtual void UpdateSize(Vector2 newSize) => size = newSize;

    public void UpdateReg(float dt) => register.Each(o => o.Update(dt));
    public void RenderReg() => register.Each(o => o.Render());
    public void DisposeReg() => register.Each(o => o.Dispose());
    public virtual MouseCursor GetOccupiedCursor() => MouseCursor.MOUSE_CURSOR_RESIZE_ALL;
    public Raylib_CsLo.Rectangle GetRawRect() => new(Position.X, Position.Y, Size.X, Size.Y);
    public Rectangle GetRect() => new(Position, Size);
    public void ReserveV2() => _freezeV2 = new Vector2(Position.X, Position.Y);
    public Vector2 GetReserveV2() => _freezeV2;
    public void SetPositionAsReserveV2() => Position = _freezeV2;
    
    ~WindowBase() => windows--;
}

public partial class CompoundWidgetBuilder : TypeRegister<IGameObject>
{
    public CompoundWidgetBuilder AddNonWidget(Action nonWidget)
    {
        RegisterGameObj(new EmptyWidget(nonWidget));
        return this;
    }

    public CompoundWidgetBuilder AddWidget(IGameObject widget)
    {
        RegisterGameObj(widget);
        return this;
    }

    public WindowBase ToWindow(string name, ImGuiWindowFlags configFlags = AlwaysAutoResize)
    {
        Window window = new(name, configFlags);
        window.RegisterGameObj(GetRegistry());
        return window;
    }

    public CompoundWidgetBuilder ToWindow(out Window window, string name,
        ImGuiWindowFlags configFlags = AlwaysAutoResize)
    {
        window = new(name, configFlags);
        window.RegisterGameObj(GetRegistry());
        return this;
    }

    public class EmptyWidget : GameObject
    {
        public Action action;
        public EmptyWidget(Action action) => this.action = action;
        protected override void RenderCall() => action?.Invoke();
    }

    public class Window : WindowBase
    {
        public Window(string name, ImGuiWindowFlags configFlags = AlwaysAutoResize) : base(name, configFlags)
        {
        }
    }
}