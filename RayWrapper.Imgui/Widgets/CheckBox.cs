using ImGuiNET;
using RayWrapper.Base.GameObject;

namespace RayWrapper.Imgui.Widgets;

public class CheckBox : GameObject
{
    public string label;
    public bool isChecked;
    public Action clicked;

    private bool _prevIsChecked;

    public CheckBox(string label, bool isChecked = false, Action clicked = null)
    {
        this.label = label;
        this.isChecked = isChecked;
        this.clicked = clicked;
    }

    protected override void RenderCall()
    {
        _prevIsChecked = isChecked;
        if (ImGui.Checkbox(label, ref isChecked)) clicked?.Invoke();
    }

    public bool Changed() => _prevIsChecked != isChecked;
}

public partial class CompoundWidgetBuilder
{
    public CompoundWidgetBuilder AddCheckBox(string label, out Func<bool> changed, bool isChecked = false, Action clicked = null)
    {
        var cb = new CheckBox(label, isChecked, clicked);
        changed = cb.Changed; 
        RegisterGameObj(cb);
        return this;
    }
}