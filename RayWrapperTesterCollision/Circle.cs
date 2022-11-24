using System;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.Extras;
using RayWrapper.Collision;

namespace RayWrapperTesterCollision;

public class Circle : CircleCollider
{
    public static Random r = new();

    public Circle(Vector2 pos) : base(new RayWrapper.Base.Primitives.Circle(pos, 10))
    {
        var phi = 2 * Math.PI * r.NextDouble();
        var vx = .25f * Math.Cos(phi);
        var vy = .25f * Math.Sin(phi);

        velocity = new Vector2((float) vx, (float) vy);
        tag = "ball";
    }

    public override void RenderShape(Vector2 pos)
    {
        Raylib.DrawCircle((int) pos.X, (int) pos.Y, radius, Raylib.DARKBLUE.EditColor(a: -100));
    }
}