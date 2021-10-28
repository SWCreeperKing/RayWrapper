using System;
using System.Numerics;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    public class EmptyRender : GameObject
    {
        public override Vector2 Position { get; set; }
        public override Vector2 Size { get; }
        public Action toRender;
        public EmptyRender(Action toRender) => this.toRender = toRender;

        protected override void UpdateCall()
        {
        }

        protected override void RenderCall() => toRender?.Invoke();
    }
}