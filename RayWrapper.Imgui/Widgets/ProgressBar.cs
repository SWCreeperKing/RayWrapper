using System.Numerics;
using ImGuiNET;
using RayWrapper.Imgui.Widgets.Base;

namespace RayWrapper.Imgui.Widgets;

public class ProgressBar : Widget
{
    public Func<float> percent;
    public Vector2 size;
    public Func<float, string> format;

    public ProgressBar(Func<float> percent, Vector2 size, Func<float, string> format = null)
    {
        this.percent = percent;
        this.size = size;
        this.format = format;
    }

    protected override void RenderCall()
    {
        var fraction = percent.Invoke();
        ImGui.ProgressBar(fraction, size, format?.Invoke(fraction));
    }
}

public partial class Window
{
    public Window AddProgressBar(Func<float> percent, Vector2 size, Func<float, string> format = null)
    {
        RegisterWidget(new ProgressBar(percent, size, format));
        return this;
    }
}