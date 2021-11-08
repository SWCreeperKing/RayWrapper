using System.Numerics;
using Raylib_cs;
using RayWrapper;
using RayWrapper.CollisionSystem;

namespace RayWrapperTesterCollision
{
    public class Bar : RectCollider
    {
        public bool vert;

        public Bar(Vector2 pos, Vector2 size) : base("bar", pos, new Rectangle(0, 0, size.X, size.Y))
        {
        }

        public override void RenderShape(Vector2 pos) => rect.MoveTo(pos).Draw(Color.GRAY);

        public override void FirstCollision(Collider c)
        {
            if (c is not Circle) return;
            if (vert) c.velocity.X = -c.velocity.X;
            else c.velocity.Y = -c.velocity.Y;
        }
    }
}