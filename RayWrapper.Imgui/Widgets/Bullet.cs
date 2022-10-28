using ImGuiNET;
using RayWrapper.Imgui.Widgets.Base;

namespace RayWrapper.Imgui.Widgets;

public class Bullet : Widget
{
    public string text;

    public Bullet(string text = "") => this.text = text;

    protected override void RenderCall()
    {
        if (text == "") ImGui.Bullet();
        else ImGui.BulletText(text);
    }
}

public partial class Window
{
    public Window AddBullet(string text = "")
    {
        RegisterWidget(new Bullet(text));
        return this;
    }
}