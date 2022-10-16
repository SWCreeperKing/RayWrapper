using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base;
using Rectangle = RayWrapper.Base.Rectangle;

namespace RayWrapper.Collision;

public abstract class CircleCollider : Collider
{
    public float radius;

    protected CircleCollider(Circle circle) : base(circle.position) => radius = circle.radius;

    public override bool CheckCollision(Collider c) =>
        c switch
        {
            CircleCollider cc => Raylib.CheckCollisionCircles(Position, radius, cc.Position, cc.radius),
            RectCollider rc => Raylib.CheckCollisionCircleRec(Position, radius, rc.rect),
            _ => false
        };

    protected override Vector2 GetSize() => new(radius * 2);
    protected override void UpdateSize(Vector2 newSize) => radius = newSize.X;
    public override bool SampleCollision(Rectangle c) => Raylib.CheckCollisionCircleRec(Position, radius, c);
}