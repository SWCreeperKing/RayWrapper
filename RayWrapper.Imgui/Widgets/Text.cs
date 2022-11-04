using System.Numerics;
using ImGuiNET;
using Raylib_CsLo;
using RayWrapper.Imgui.Widgets.Base;

namespace RayWrapper.Imgui.Widgets;

public class Text : Widget
{
    public enum Format
    {
        Normal,
        Label,
        Wrapped,
        Unformatted,
        Colored,
        Disabled
    }

    public string text;
    public string fmt;
    public Vector4 color;

    private Format _textFormat;

    public Text(string text, Color? color = null, Format format = Format.Normal)
    {
        this.text = text;
        this.color = (color ?? Raylib.WHITE).ToV4();
        _textFormat = color is null ? format : Format.Colored;
    }

    public Text(string text, string fmt)
    {
        this.text = text;
        this.fmt = fmt;
        _textFormat = Format.Label;
    }

    protected override void RenderCall()
    {
        switch (_textFormat)
        {
            case Format.Normal:
                ImGui.Text(text);
                break;
            case Format.Label:
                ImGui.LabelText(text, fmt);
                break;
            case Format.Wrapped:
                ImGui.TextWrapped(text);
                break;
            case Format.Disabled:
                ImGui.TextDisabled(text);
                break;
            case Format.Unformatted:
                ImGui.TextUnformatted(text);
                break;
            case Format.Colored:
                ImGui.TextColored(color, text);
                break;
        }
    }

    public float GetTextLineHeight() => ImGui.GetTextLineHeight();
    public float GetTextLineHeightWithSpacing() => ImGui.GetTextLineHeightWithSpacing();
}

public partial class CompoundWidgetBuilder
{
    public CompoundWidgetBuilder AddText(string text, Color? color = null, Text.Format format = Text.Format.Normal)
    {
        RegisterWidget(new Text(text, color, format));
        return this;
    }

    public CompoundWidgetBuilder AddText(string text, string fmt)
    {
        RegisterWidget(new Text(text, fmt));
        return this;
    }
}