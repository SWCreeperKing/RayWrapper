using ImGuiNET;

namespace RayWrapper.Imgui.Widgets;

public abstract class WindowBase : WidgetRegister, IWBase
{
    private string _name;

    protected WindowBase(string name) => _name = name;

    public void Update()
    {
        UpdateCall();
        UpdateReg();
    }

    public void Render()
    {
        ImGui.Begin(_name);
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