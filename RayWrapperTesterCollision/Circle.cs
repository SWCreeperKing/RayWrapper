using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper;
using RayWrapper.CollisionSystem;

namespace RayWrapperTesterCollision
{
    public class Circle : CircleCollider
    {
        public static Random r = new();

        public Circle(Vector2 pos) : base("circle", pos, 10)
        {
            var phi = 2 * Math.PI * r.NextDouble();
            var vx = .25f * Math.Cos(phi);
            var vy = .25f * Math.Sin(phi);

            velocity = new Vector2((float) vx, (float) vy);
        }

        public override void RenderShape(Vector2 pos) =>
            Raylib.DrawCircle((int) pos.X, (int) pos.Y, radius, Color.DARKBLUE.EditColor(a: -100));
    }
}