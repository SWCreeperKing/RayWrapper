using System.Numerics;
using ImGuiNET;
using RayWrapper.Base.GameObject;

namespace RayWrapper.Imgui.Widgets;

public class ProgressBar : GameObject
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
        var fraction = percent();
        ImGui.ProgressBar(fraction, size, format?.Invoke(fraction));
    }
}

public partial class CompoundWidgetBuilder
{
    public CompoundWidgetBuilder AddProgressBar(Func<float> percent, Vector2 size, Func<float, string> format = null)
    {
        RegisterGameObj(new ProgressBar(percent, size, format));
        return this;
    }
}