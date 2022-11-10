using ImGuiNET;
using RayWrapper.Imgui.Widgets.Base;

namespace RayWrapper.Imgui.Widgets;

public class TreeNode : SubRegister<TreeNode>
{
    public string name;
    public TreeNode(string name) => this.name = name;
    public override bool Begin() => ImGui.TreeNode(name);
    public override void End() => ImGui.TreePop();
}

public partial class CompoundWidgetBuilder
{
    public TreeNode ToTreeNode(string name) => new TreeNode(name).Add(GetRegistry());

    public CompoundWidgetBuilder ToTreeNode(string name, out TreeNode treeNode)
    {
        treeNode = new TreeNode(name).Add(GetRegistry());
        return this;
    }
}