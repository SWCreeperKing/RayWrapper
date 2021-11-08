using System.Numerics;

namespace RayWrapper.Vars
{
    public abstract class SceneMod : GameObject
    {
        public override Vector2 Position { get; set; }
        public override Vector2 Size { get; }

        protected override void UpdateCall()
        {
        }

        protected override void RenderCall()
        {
        }
    }
}