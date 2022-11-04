using ImGuiNET;
using Raylib_CsLo;

namespace RayWrapper.Imgui.Widgets;

public class ColorEditor : ColorPicker
{
    public ColorEditor(string name, Action<Color> onChange, Color? color = null, bool pickAlpha = false) : base(name, onChange, color, pickAlpha)
    {
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
    public CompoundWidgetBuilder AddColorEditor(string name, Action<Color> onChange, Color? color = null, bool pickAlpha = false)
    {
        RegisterWidget(new ColorEditor(name, onChange, color, pickAlpha));
        return this;
    }
}