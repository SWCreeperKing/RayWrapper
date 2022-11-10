using System.Numerics;
using ImGuiNET;
using RayWrapper.Base.GameObject;

namespace RayWrapper.Imgui.Widgets;

public class PlotHistogram : GameObject
{
    private string _label;
    private string _overlayText;
    private int _offset;
    private int? _stride;
    private Func<float[]> _arrFunc;
    private Vector2? _scale;
    private Vector2? _size;

    public PlotHistogram(string label, Func<float[]> arrFunc, string overlayText = "", int offset = 0,
        Vector2? scale = null, Vector2? size = null, int? stride = null)
    {
        _label = label;
        _overlayText = overlayText;
        _offset = offset;
        _arrFunc = arrFunc;
        _scale = scale;
        _size = size;
        _stride = stride;
    }

    protected override void RenderCall()
    {
        var arr = _arrFunc();
        if (!arr.Any()) return;

        if (_scale is null) ImGui.PlotHistogram(_label, ref arr[0], arr.Length, _offset, _overlayText);
        else if (_size is null)
        {
            var scle = _scale.Value;
            ImGui.PlotHistogram(_label, ref arr[0], arr.Length, _offset, _overlayText, scle.X, scle.Y);
        }
        else if (_stride is null)
        {
            var scle = _scale.Value;
            ImGui.PlotHistogram(_label, ref arr[0], arr.Length, _offset, _overlayText, scle.X, scle.Y, _size.Value);
        }
        else
        {
            var scle = _scale.Value;
            ImGui.PlotHistogram(_label, ref arr[0], arr.Length, _offset, _overlayText, scle.X, scle.Y, _size.Value,
                _stride.Value);
        }
    }
}

public partial class CompoundWidgetBuilder
{
    public CompoundWidgetBuilder AddPlotHistogram(string label, Func<float[]> arrFunc, string overlayText = "", int offset = 0,
        Vector2? scale = null, Vector2? size = null, int? stride = null)
    {
        RegisterGameObj(new PlotHistogram(label, arrFunc, overlayText, offset, scale, size, stride));
        return this;
    }
}