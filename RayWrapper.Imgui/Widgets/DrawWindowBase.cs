using System.Numerics;
using ImGuiNET;

namespace RayWrapper.Imgui.Widgets;

public abstract class DrawWindowBase : WindowBase
{
    public DrawWindowBase(string name, ImGuiWindowFlags configFlags = ImGuiWindowFlags.AlwaysAutoResize) : base(name,
        configFlags)
    {
    }

    protected override void WindowRender()
    {
        var drawLayer = ImGui.GetWindowDrawList();
        var pos = ImGui.GetWindowPos();
        DrawRender(drawLayer, pos with {Y = pos.Y + 20}, ImGui.GetWindowSize());
    }

    public abstract void DrawRender(ImDrawListPtr drawLayer, Vector2 offset, Vector2 size);
}