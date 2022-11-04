using System.Numerics;
using ImGuiNET;
using Raylib_CsLo;
using RayWrapper.Imgui.Widgets.Base;

namespace RayWrapper.Imgui.Widgets;

public class ColorPicker : Widget
{
    public string name;
    public Action<Color> onChange;
    public bool pickAlpha;
    public Vector4 color4;
    public Vector3 color3;

    public ColorPicker(string name, Action<Color> onChange, Color? color = null, bool pickAlpha = false)
    {
        this.name = name;
        this.onChange = onChange;
        this.pickAlpha = pickAlpha;
        var rColor = color ?? Raylib.WHITE;
        color4 = rColor.ToV4();
        color3 = rColor.ToV3();
    }

    protected override void RenderCall()
    {
        if (pickAlpha)
        {
            if (ImGui.ColorPicker4(name, ref color4)) onChange.Invoke(color4.ToColor());
        } else if (ImGui.ColorPicker3(name, ref color3)) onChange.Invoke(color3.ToColor());
    }
}

public partial class CompoundWidgetBuilder
{
    public CompoundWidgetBuilder AddColorPicker(string name, Action<Color> onChange, Color? color = null, bool pickAlpha = false)
    {
        RegisterWidget(new ColorPicker(name, onChange, color, pickAlpha));
        return this;
    }
}