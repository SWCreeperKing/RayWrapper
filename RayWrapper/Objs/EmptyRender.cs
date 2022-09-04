using System;
using RayWrapper.Vars;

namespace RayWrapper.Objs;

public class EmptyRender : GameObject
{
    public Action toRender;
    public EmptyRender(Action toRender) => this.toRender = toRender;

    protected override void RenderCall() => toRender?.Invoke();
}