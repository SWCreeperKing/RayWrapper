using System.Numerics;
using RayWrapper.Collision;
using static Raylib_CsLo.Raylib;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapperTesterCollision;

public class Bar : RectangleCollider
{
    public bool vert;

    public Bar(Vector2 pos, Vector2 size) : base(new Rectangle(pos, size))
    {
        // tag = "bar";
    }

    // public override void RenderShape(Vector2 pos)
    protected override void RenderCall() => rect.Draw(GRAY);

    // public override void FirstCollision(Collider c)
    public override void OnCollisionEnter(Collider c)
    {
        if (c is not Ball b) return;
        if (vert) b.velocity.X = -b.velocity.X;
        else b.velocity.Y = -b.velocity.Y;
    }
}