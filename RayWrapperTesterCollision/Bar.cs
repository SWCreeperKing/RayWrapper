using System.Numerics;
using Raylib_CsLo;
using RayWrapper;

namespace RayWrapperTesterCollision
{
    // public class Bar : RectCollider
    // {
    //     public bool vert;
    //
    //     public Bar(Vector2 pos, Vector2 size) : base("bar", pos, new Rectangle(0, 0, size.X, size.Y))
    //     {
    //     }
    //
    //     public override void RenderShape(Vector2 pos) => rect.NewMoveTo(pos).Draw(Raylib.GRAY);
    //
    //     public override void FirstCollision(Collider c)
    //     {
    //         if (c is not Circle) return;
    //         if (vert) c.velocity.X = -c.velocity.X;
    //         else c.velocity.Y = -c.velocity.Y;
    //     }
    // }
}