using RayWrapper.Base.GameObject;

namespace RayWrapper.LegacyUI.UI;

public class EmptyRender : GameObject
{
    public Action toRender;
    public EmptyRender(Action toRender) => this.toRender = toRender;

    protected override void RenderCall() => toRender?.Invoke();
}