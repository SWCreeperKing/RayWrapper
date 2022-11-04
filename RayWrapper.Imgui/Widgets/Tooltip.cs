using ImGuiNET;
using RayWrapper.Imgui.Widgets.Base;

namespace RayWrapper.Imgui.Widgets;

public class Tooltip : WidgetRegister, IWidget
{
    public void Update() => UpdateReg();

    public void Render()
    {
        ImGui.BeginTooltip();
        RenderReg();
        ImGui.EndTooltip();
    }

    public void Dispose() => DisposeReg();

    public Tooltip Add(IWidget widget)
    {
        RegisterWidget(widget);
        return this;
    }
    
    public Tooltip Add(params IWidget[] widget)
    {
        RegisterWidget(widget);
        return this;
    }
    
    public Tooltip Remove(IWidget widget)
    {
        DeRegisterWidget(widget);
        return this;
    }
    
    public Tooltip Remove(params IWidget[] widget)
    {
        DeRegisterWidget(widget);
        return this;
    }
}