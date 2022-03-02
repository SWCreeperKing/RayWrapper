using System.Numerics;
using Raylib_CsLo;
using RayWrapper;
using RayWrapper.CollisionSystem;
using RayWrapper.Objs;
using RayWrapper.Vars;
using static Raylib_CsLo.Color;
using static Raylib_CsLo.Raylib;
using static RayWrapper.GameBox;

namespace RayWrapperTesterCollision
{
    class Program : GameLoop
    {
        static void Main(string[] args)
        {
            new GameBox(new Program(), new Vector2(1280, 720), "collision testing", 500);
        }

        public override void Init()
        {
            InitCollision();
            var wx = WindowSize.X;
            var wy = WindowSize.Y;
            CollisionLayerTags.Add(("circle", "bar"));
            new Bar(new Vector2(0, 0), new Vector2(wx, 10));
            new Bar(new Vector2(0, wy - 10), new Vector2(wx, 10));
            new Bar(new Vector2(0, 0), new Vector2(10, wy)) { vert = true };
            new Bar(new Vector2(wx - 10, 0), new Vector2(10, wy)) { vert = true };
            RegisterGameObj(new Text($"{Collider.count - 4}", new Vector2(12, 60), RED), new Text(
                new Actionable<string>(() => $@"Collision Time
cur: {CurrentCollision}ms
avg: {TimeAverage}ms
high: {CollisionHigh}ms".Replace("\r", "")), new Vector2(300, 50), SKYBLUE));
        }

        public override void UpdateLoop()
        {
            if (!IsMouseButtonPressed(MOUSE_LEFT_BUTTON)) return;
            for (var i = 0; i < 50; i++) new Circle(mousePos);
        }

        public override void RenderLoop() => DrawFPS(12, 12);
    }
}