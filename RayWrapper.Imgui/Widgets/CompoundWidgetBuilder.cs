using ImGuiNET;
using RayWrapper.Base.GameObject;
using static ImGuiNET.ImGuiWindowFlags;

namespace RayWrapper.Imgui.Widgets;

public abstract class WindowBase : GameObject
{
    public bool closable;
    public bool isOpen = true;
    public ImGuiWindowFlags configFlags;

    private string _name;

    protected WindowBase(string name, ImGuiWindowFlags configFlags = AlwaysAutoResize)
    {
        _name = name;
        this.configFlags = configFlags;
    }

    protected sealed override void UpdateCall(float dt)
    {
        if (!isOpen && closable) return;
        WindowUpdate(dt);
        UpdateReg(dt);
    }

    protected sealed override void RenderCall()
    {
        if (!isOpen && closable) return;
        var begin = closable ? ImGui.Begin(_name, ref isOpen, configFlags) : ImGui.Begin(_name, configFlags);
        if (!begin)
        {
            ImGui.End();
            return;
        }

        WindowRender();
        RenderReg();
        ImGui.End();
    }

    protected sealed override void DisposeCall()
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