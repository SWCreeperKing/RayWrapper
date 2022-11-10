using ImGuiNET;
using RayWrapper.Base.GameObject;

namespace RayWrapper.Imgui.Widgets;

public class ListView : GameObject
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

    protected override void RenderCall()
    {
        var arr = array();
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

public partial class CompoundWidgetBuilder
{
    public CompoundWidgetBuilder AddListView(string name, Func<string[]> array, out Func<int> selected, bool selectable = true)
    {
        var ls = new ListView(name, array) { selectable = selectable };
        selected = () => ls.Changed(out var nSelect) ? nSelect : -1;
        RegisterGameObj(ls);
        return this;
    }
}