using System.Numerics;
using Raylib_CsLo;

namespace RayWrapper.Collision;

public abstract class RectCollider : Collider
{
    public override Vector2 Size => rect.Size();

    public Rectangle rect;

    public RectCollider(Rectangle rect) : base(rect.Pos()) => this.rect = rect;

    public override bool CheckCollision(Collider c) =>
        c switch
        {
            RectCollider rc => Raylib.CheckCollisionRecs(this, rc),
            CircleCollider cc => Raylib.CheckCollisionCircleRec(cc.Position, cc.radius, this),
            _ => false
        };

    public override bool SampleCollision(Rectangle c) => Raylib.CheckCollisionRecs(this, c);
    public static implicit operator Rectangle(RectCollider rc) => rc.rect.Clone();
}