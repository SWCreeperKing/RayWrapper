using System.Numerics;
using ImGuiNET;
using RayWrapper.Base.GameObject;

namespace RayWrapper.Imgui.Widgets;

public class PlotLines : GameObject
{
    private string _label;
    private string _overlayText;
    private int _offset;
    private Func<float[]> _arrFunc;
    private Vector2? _scale;
    private Vector2? _size;

    public PlotLines(string label, Func<float[]> arrFunc, string overlayText = "", int offset = 0,
        Vector2? scale = null, Vector2? size = null)
    {
        _label = label;
        _overlayText = overlayText;
        _offset = offset;
        _arrFunc = arrFunc;
        _scale = scale;
        _size = size;
    }

    protected override void RenderCall()
    {
        var arr = _arrFunc();
        if (!arr.Any()) return;

        if (_scale is null) ImGui.PlotLines(_label, ref arr[0], arr.Length, _offset, _overlayText);
        else if (_size is null)
        {
            var scle = _scale.Value;
            ImGui.PlotLines(_label, ref arr[0], arr.Length, _offset, _overlayText, scle.X, scle.Y);
        }
        else
        {
            var scle = _scale.Value;
            ImGui.PlotLines(_label, ref arr[0], arr.Length, _offset, _overlayText, scle.X, scle.Y, _size.Value);
        }
    }
}

public partial class CompoundWidgetBuilder
{
    public CompoundWidgetBuilder AddPlotLines(string label, Func<float[]> arrFunc, string overlayText = "", int offset = 0,
        Vector2? scale = null, Vector2? size = null)
    {
        RegisterGameObj(new PlotLines(label, arrFunc, overlayText, offset, scale, size));
        return this;
    }
}

