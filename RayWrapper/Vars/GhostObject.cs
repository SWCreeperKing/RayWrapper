using System.Numerics;

namespace RayWrapper.Vars
{
    public abstract class GhostObject : GameObject
    {
        public override Vector2 Position
        {
            get => Vector2.Zero;
            set { }
        }

        public override Vector2 Size => Vector2.Zero;

        protected override void UpdateCall()
        {
        }

        protected override void RenderCall()
        {
        }
    }
}