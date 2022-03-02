using System.Numerics;
using Raylib_CsLo;

namespace RayWrapper.CollisionSystem
{
    public abstract class CircleCollider : Collider
    {
        public override Vector2 Size => new(radius * 2);
        public float radius;

        protected CircleCollider(string layerId, Vector2 pos, float radius) : base(layerId, pos) => this.radius = radius;

        public override bool CheckCollision(Collider c) =>
            c switch
            {
                CircleCollider cc => Raylib.CheckCollisionCircles(Position, radius, cc.Position, cc.radius),
                RectCollider rc => Raylib.CheckCollisionCircleRec(Position, radius, rc),
                _ => false
            };

        public override bool SampleCollision(Rectangle c) => Raylib.CheckCollisionCircleRec(Position, radius, c);
    }
}