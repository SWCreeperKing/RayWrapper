using System.Numerics;
using RayWrapper.Vars;

namespace RayWrapper.Animation.Transitions
{
    public class MoveTransition : Transition
    {
        public Vector2 moveBy;
        private Vector2 _snapDest;

        public MoveTransition(Vector2 moveBy, GameObject obj, float duration = 5) : base(obj, duration) =>
            this.moveBy = moveBy;

        public override void InitTransition() => _snapDest = obj.Position + moveBy;
        public override void UpdateTransition(float deltaTime) => obj.Position += moveBy * deltaTime / duration;
        public override void SnapTransition() => obj.Position = _snapDest;
    }
}