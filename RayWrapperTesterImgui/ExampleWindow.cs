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

        var r = new Random();
        var arr = Enumerable.Repeat(0f, r.Next(40, 50)).Select(i => (float) (r.Next(60, 80) * r.NextDouble()))
            .ToArray();
        var plot = new PlotLines("plot", () => arr);
        RegisterGameObj(new CheckBox("check 1"), cb2, plot);
    }

    protected override void WindowUpdate(float dt) => cb3.Update(dt);
    protected override void WindowRender() => cb3.Render();
    protected override void WindowDispose() => cb3.Dispose();
}