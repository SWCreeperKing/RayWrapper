using ImGuiNET;
using RayWrapper.Base.GameObject;

namespace RayWrapper.Imgui.Widgets;

public class Bullet : GameObject
{
    public string text;

    public Bullet(string text = "") => this.text = text;

    protected override void RenderCall()
    {
        if (text == "") ImGui.Bullet();
        else ImGui.BulletText(text);
    }
}

public partial class CompoundWidgetBuilder
{
    public CompoundWidgetBuilder AddBullet(string text = "")
    {
        RegisterGameObj(new Bullet(text));
        return this;
    }
}