using ImGuiNET;
using RayWrapper.Imgui.Widgets.Base;

namespace RayWrapper.Imgui.Widgets;

public class ListView : Widget
{
    public string name;
    public Func<string[]> array;
    public int selected = -1;
    public int prevSelected = -1;
    public bool selectable = true;

    public ListView(string name, Func<string[]> array)
    {
        this.name = name;
        this.array = array;
    }

    public override void RenderCall()
    {
        var arr = array.Invoke();
        prevSelected = selected;
        ImGui.ListBox(name, ref selected, arr, arr.Length);

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
    public Window AddListView(string name, Func<string[]> array, out Func<int> selected, bool selectable = true)
    {
        var ls = new ListView(name, array) { selectable = selectable };
        selected = () => ls.Changed(out var nSelect) ? nSelect : -1;
        RegisterWidget(ls);
        return this;
    }
}