using ImGuiNET;
using RayWrapper.Imgui.Widgets.Base;

namespace RayWrapper.Imgui.Widgets;

public class DropDown : Widget
{
    public string name;
    public Func<string[]> array;
    public int selected = -1;
    public int prevSelected = -1;
    public bool selectable = true;
    
    public DropDown(string name, Func<string[]> array)
    {
        this.name = name;
        this.array = array;
    }

    protected override void RenderCall()
    {
        var arr = array.Invoke();
        prevSelected = selected;
        ImGui.Combo(name, ref selected, arr, arr.Length);

        if (selectable) return;
        selected = -1;
    }

    public bool Changed(out int newSelected)
    {
        newSelected = -1;
        if (prevSelected == selected) return false;
        newSelected = selected;
        return true;
    }
}

public partial class Window
{
    public Window AddDropDown(string name, Func<string[]> array, out Func<int> selected, bool selectable = true)
    {
        var dd = new DropDown(name, array) { selectable = selectable };
        selected = () => dd.Changed(out var nSelect) ? nSelect : -1;
        RegisterWidget(dd);
        return this;
    }
}