using System;
using System.Numerics;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    public class EmptyRender : GameObject
    {
        public Action toRender;

        public EmptyRender(Action toRender) : base(new Vector2()) =>
            this.toRender = toRender;

        public override void Update()
        {
        }

        protected override void RenderCall() => toRender?.Invoke();

        public override void PositionChange(Vector2 v2)
        {
        }

        public override Vector2 Size() => Vector2.Zero;
    }
}