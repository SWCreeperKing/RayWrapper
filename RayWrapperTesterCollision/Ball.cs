using System;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.Extras;
using RayWrapper.Base.GameBox;
using RayWrapper.Collision;

namespace RayWrapperTesterCollision;

public class Ball : CircleCollider
{
    public static Color fadedBlue = Raylib.DARKBLUE.EditColor(a: -100);
    public static Random r = new();
    public Vector2 velocity;

    public Ball(Vector2 pos) : base(new RayWrapper.Base.Primitives.Circle(pos, 10))
    {
        var phi = 2 * Math.PI * r.NextDouble();
        var vx = .25f * Math.Cos(phi);
        var vy = .25f * Math.Sin(phi);

        velocity = new Vector2((float) vx, (float) vy);
    }

    protected override void UpdateCall(float dt) => Position += velocity * 1000 * dt;
    protected override void RenderCall() => circle.Draw(fadedBlue);
    
    public override bool ToDestroy()
    {
        return circle.position.X < 0 || circle.position.Y < 0 || circle.position.X > GameBox.WindowSize.X ||
               circle.position.Y > GameBox.WindowSize.Y;
    }
}