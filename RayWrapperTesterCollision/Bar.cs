using System.Numerics;
using RayWrapper.Collision;
using static Raylib_CsLo.Raylib;
using Rectangle = RayWrapper.Base.Rectangle;

namespace RayWrapperTesterCollision;

public class Bar : RectCollider
{
    public bool vert;

    public Bar(Vector2 pos, Vector2 size) : base(new Rectangle(pos, size))
    {
        tag = "bar";
    }

    public override void RenderShape(Vector2 pos)
    {
        var nRect = rect.Clone();
        nRect.Pos = pos;
        nRect.Draw(GRAY);
    }

    public override void FirstCollision(Collider c)
    {
        if (c is not Circle) return;
        if (vert) c.velocity.X = -c.velocity.X;
        else c.velocity.Y = -c.velocity.Y;
    }
}