using System.Numerics;

namespace RayWrapper.Vars
{
    public abstract class GhostObject : GameObject
    {
        public override Vector2 Position => Vector2.Zero;
        public override Vector2 Size => Vector2.Zero;
    }
}