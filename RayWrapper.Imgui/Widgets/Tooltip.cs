using ImGuiNET;
using RayWrapper.Imgui.Widgets.Base;

namespace RayWrapper.Imgui.Widgets;

public class Tooltip : SubRegister<Tooltip>
{
    public override bool Begin()
    {
        ImGui.BeginTooltip();
        return true;
    }

    public override void End() => ImGui.EndTooltip();
}

public partial class CompoundWidgetBuilder
{
    public Tooltip ToTooltip() => new Tooltip().Add(GetRegistry());

    public CompoundWidgetBuilder ToTooltip(out Tooltip tooltip)
    {
        tooltip = new Tooltip().Add(GetRegistry());
        return this;
    }
}