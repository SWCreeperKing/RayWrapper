using System.Numerics;
using ImGuiNET;
using Raylib_CsLo;
using RayWrapper.Base.GameBox;
using RayWrapper.Imgui;
using RayWrapper.Imgui.Widgets;

namespace RayWrapperTesterImgui;

public class ExampleDrawWindow : DrawWindowBase
{
    public uint color = Raylib.SKYBLUE.ToV4().ToUint();

    public ExampleDrawWindow(string name, ImGuiWindowFlags configFlags = ImGuiWindowFlags.AlwaysAutoResize) : base(name,
        configFlags)
    {
    }

    public override void DrawRender(ImDrawListPtr drawLayer, Vector2 offset, Vector2 size)
    {
        drawLayer.DrawBezierCurve(ImGui.GetWindowPos() + ImGui.GetWindowSize() / 2, Input.MousePosition.currentPosition,
            color, 15);
    }
}