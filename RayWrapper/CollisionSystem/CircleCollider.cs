using System.Numerics;
using Raylib_cs;

namespace RayWrapper.CollisionSystem
{
    public abstract class CircleCollider : Collider
    {
        public float radius;

        protected CircleCollider(string layerId, Vector2 pos, float radius) : base(layerId, pos) => this.radius = radius;

        public override bool CheckCollision(Collider c) =>
            c switch
            {
                CircleCollider cc => Raylib.CheckCollisionCircles(Position, radius, cc.Position, cc.radius),
                RectCollider rc => Raylib.CheckCollisionCircleRec(Position, radius, rc),
                _ => false
            };
    }
}