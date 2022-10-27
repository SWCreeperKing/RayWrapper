using ImGuiNET;
using RayWrapper.Imgui.Widgets.Base;

namespace RayWrapper.Imgui.Widgets;

public abstract class WindowBase : WidgetRegister, IWBase
{
    public bool isOpen = true;
    public ImGuiWindowFlags configFlags = ImGuiWindowFlags.AlwaysAutoResize;

    private string _name;

    protected WindowBase(string name) => _name = name;

    protected WindowBase(string name, ImGuiWindowFlags configFlags)
    {
        _name = name;
        this.configFlags = configFlags;
    }

    public void Update()
    {
        UpdateCall();
        UpdateReg();
    }

    public void Render()
    {
        ImGui.Begin(_name, ref isOpen, configFlags);
        RenderCall();
        RenderReg();
        ImGui.End();
    }

    public void Dispose()
    {
        DisposeCall();
        DisposeReg();
    }

    public virtual void UpdateCall()
    {
    }

    public virtual void RenderCall()
    {
    }

    public virtual void DisposeCall()
    {
    }
}

public partial class Window : WindowBase
{
    public Window(string name) : base(name)
    {
    }
}