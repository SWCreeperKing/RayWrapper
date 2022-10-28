using ImGuiNET;
using RayWrapper.Imgui.Widgets;

namespace RayWrapperTesterImgui;

public class ExampleWindow : WindowBase
{
    public CheckBox cb2;
    public CheckBox cb3;

    public ExampleWindow(string name, ImGuiWindowFlags configFlags = ImGuiWindowFlags.AlwaysAutoResize) : base(name,
        configFlags)
    {
        cb2 = new CheckBox("check 2");
        cb3 = new CheckBox("check 3");
        RegisterWidgets(new CheckBox("check 1"), cb2);
    }

    protected override void UpdateCall() => cb3.Update();
    protected override void RenderCall() => cb3.Render();
    protected override void DisposeCall() => cb3.Dispose();
}