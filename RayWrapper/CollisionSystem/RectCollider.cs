using System.Numerics;
using Raylib_cs;

namespace RayWrapper.CollisionSystem
{
    public abstract class RectCollider : Collider
    {
        public override Vector2 Size => rect.Size();

        public Rectangle rect;

        public RectCollider(string layerId, Vector2 pos, Rectangle rect) : base(layerId, pos) => this.rect = rect;

        public override bool CheckCollision(Collider c) =>
            c switch
            {
                RectCollider rc => Raylib.CheckCollisionRecs(this, rc),
                CircleCollider cc => Raylib.CheckCollisionCircleRec(cc.Position, cc.radius, this),
                _ => false
            };

        public override bool SampleCollision(Rectangle c) => Raylib.CheckCollisionRecs(this, c);
        public static implicit operator Rectangle(RectCollider rc) => rc.rect.MoveTo(rc.Position);
    }
}