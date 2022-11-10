using ImGuiNET;
using RayWrapper.Imgui.Widgets.Base;

namespace RayWrapper.Imgui.Widgets;

public class Menu : SubRegister<Menu>
{
    public string label;
    public Menu(string label) => this.label = label;
    public override bool Begin() => ImGui.BeginMenu(label);
    public override void End() => ImGui.EndMenu();
}

public partial class CompoundWidgetBuilder
{
    public Menu ToMenu(string label) => new Menu(label).Add(GetRegistry());

    public CompoundWidgetBuilder ToMenu(string label, out Menu menu)
    {
        menu = new Menu(label).Add(GetRegistry());
        return this;
    }
}