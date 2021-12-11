using System.Numerics;
using RayWrapper.Animation.SinglePurposeObjects;
using RayWrapper.Vars;

namespace RayWrapper.Animation.Transitions
{
    public class SizeTransition<T> : Transition where T : GameObject, ISizeable
    {
        public T t;
        public Vector2 moveBy;
        private Vector2 _snapSize;

        public SizeTransition(Vector2 moveBy, T t, float duration = 5) : base(t, duration) =>
            (this.moveBy, this.t) = (moveBy, t);

        public override void InitTransition() => _snapSize = obj.Size + moveBy;
        public override void UpdateTransition(float deltaTime) => t.AddSize(moveBy * deltaTime / duration);
        public override void SnapTransition() => t.SetSize(_snapSize);
    }
}