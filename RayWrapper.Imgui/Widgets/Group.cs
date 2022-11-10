using ImGuiNET;
using RayWrapper.Imgui.Widgets.Base;

namespace RayWrapper.Imgui.Widgets;

public class Group : SubRegister<Group>
{
    public override bool Begin()
    {
        ImGui.BeginGroup();
        return true;
    }

    public override void End() => ImGui.EndGroup();
}

public partial class CompoundWidgetBuilder
{
    public Group ToGroup() => new Group().Add(GetRegistry());

    public CompoundWidgetBuilder ToGroup(out Group group)
    {
        group = new Group().Add(GetRegistry());
        return this;
    }
}