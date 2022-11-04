using ImGuiNET;
using RayWrapper.Imgui.Widgets.Base;
using static ImGuiNET.ImGuiWindowFlags;

namespace RayWrapper.Imgui.Widgets;

public abstract class WindowBase : WidgetRegister, IWBase
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

    public void Update()
    {
        if (!isOpen && closable) return;
        UpdateCall();
        UpdateReg();
    }

    public void Render()
    {
        if (!isOpen && closable) return;
        var begin = closable ? ImGui.Begin(_name, ref isOpen, configFlags) : ImGui.Begin(_name, configFlags);
        if (!begin)
        {
            ImGui.End();
            return;
        }

        RenderCall();
        RenderReg();
        ImGui.End();
    }

    public void Dispose()
    {
        DisposeCall();
        DisposeReg();
    }

    protected virtual void UpdateCall()
    {
    }

    protected virtual void RenderCall()
    {
    }

    protected virtual void DisposeCall()
    {
    }
}

public partial class Window : WindowBase
{
    public Window(string name, ImGuiWindowFlags configFlags = AlwaysAutoResize) : base(name, configFlags)
    {
    }

    public Window AddNonWidget(Action nonWidget)
    {
        RegisterWidget(new EmptyWidget(nonWidget));
        return this;
    }

    public class EmptyWidget : Widget
    {
        public Action action;
        public EmptyWidget(Action action) => this.action = action;
        protected override void RenderCall() => action?.Invoke();
    }
}