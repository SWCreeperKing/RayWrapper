using System;
using System.Numerics;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    public class EmptyRender : GameObject
    {
        public Action ToRender;

        public EmptyRender(Action toRender) : base(new Vector2()) => ToRender = toRender;

        public override void Update()
        {
        }

        protected override void RenderCall()
        {
            ToRender.Invoke();
        }

        public override void PositionChange(Vector2 v2)
        {
        }
    }
}