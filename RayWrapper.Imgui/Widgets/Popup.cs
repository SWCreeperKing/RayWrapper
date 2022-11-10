using ImGuiNET;
using RayWrapper.Imgui.Widgets.Base;

namespace RayWrapper.Imgui.Widgets;

public class Popup : SubRegister<Popup>
{
    public string nameId;
    public Popup(string nameId) => this.nameId = nameId;
    public override bool Begin() => ImGui.BeginPopup(nameId);
    public override void End() => ImGui.EndPopup();
}

public partial class CompoundWidgetBuilder
{
    public Popup ToPopup(string name) => new Popup(name).Add(GetRegistry());

    public CompoundWidgetBuilder ToPopup(string name, out Popup popup)
    {
        popup = new Popup(name).Add(GetRegistry());
        return this;
    }
}