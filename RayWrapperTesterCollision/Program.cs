﻿using System.Numerics;
using Raylib_cs;
using RayWrapper;
using RayWrapper.CollisionSystem;
using RayWrapper.Vars;
using static Raylib_cs.Color;
using static Raylib_cs.Raylib;
using static RayWrapper.GameBox;

namespace RayWrapperTesterCollision
{
    class Program : GameLoop
    {
        public static GameBox gb;

        static void Main(string[] args)
        {
            gb = new GameBox(new Program(), new Vector2(1280, 720), "collision testing", 500);
            gb.Start(true);
        }

        public override void Init()
        {
            var wx = WindowSize.X;
            var wy = WindowSize.Y;
            CollisionLayerTags.Add(("circle", "bar"));
            new Bar(new Vector2(0, 0), new Vector2(wx, 10));
            new Bar(new Vector2(0, wy - 10), new Vector2(wx, 10));
            new Bar(new Vector2(0, 0), new Vector2(10, wy)) {vert = true};
            new Bar(new Vector2(wx - 10, 0), new Vector2(10, wy)) {vert = true};
        }

        public override void UpdateLoop()
        {
            if (IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
            {
                for (var i = 0; i < 50; i++) new Circle(MousePos);
            }
        }

        public override void RenderLoop()
        {
            DrawFPS(12, 12);
            Text($"{Collider.count - 4}", new Vector2(12, 60), RED);
            Text($@"Collision Time
cur: {CurrentCollision}ms
avg: {TimeAverage}ms
high: {CollisionHigh}ms".Replace("\r", ""), new Vector2(300, 50), SKYBLUE);
        }
    }
}