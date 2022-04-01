using System.Numerics;
using RayWrapper;
using RayWrapper.Collision;
using static Raylib_CsLo.Raylib;

namespace RayWrapperTesterCollision
{
    public class Bar : RectCollider
    {
        public bool vert;

        public Bar(Vector2 pos, Vector2 size) : base(RectWrapper.AssembleRectFromVec(pos, size))
        {
        }

        public override void RenderShape(Vector2 pos) => rect.NewMoveTo(pos).Draw(GRAY);

        public override void FirstCollision(Collider c)
        {
            if (c is not Circle) return;
            if (vert) c.velocity.X = -c.velocity.X;
            else c.velocity.Y = -c.velocity.Y;
        }
    }
}