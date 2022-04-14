using System.Numerics;
using Raylib_CsLo;
using RayWrapper;
using RayWrapper.Objs;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;
using static RayWrapper.Collision.Collision;
using static RayWrapper.GameBox;

namespace RayWrapperTesterCollision;

class Program : GameLoop
{
    static void Main(string[] args)
    {
        new GameBox(new Program(), new Vector2(1280, 720), "collision testing", 500);
    }

    public override void Init()
    {
        InitPhysics(8, 8);
        var wx = WindowSize.X;
        var wy = WindowSize.Y;

        collisionRules.TryAdd("bar", "ball");

        new Bar(new Vector2(0, 0), new Vector2(wx, 10));
        new Bar(new Vector2(0, wy - 10), new Vector2(wx, 10));
        new Bar(new Vector2(0, 0), new Vector2(10, wy)) { vert = true };
        new Bar(new Vector2(wx - 10, 0), new Vector2(10, wy)) { vert = true };
        // new Bar(new Vector2(400), new Vector2(600, 1));
            
        RegisterGameObj(new Text(new Actionable<string>(() => $"{CountColliders() - 4}"), new Vector2(12, 60), RED),
            new Text(
                new Actionable<string>(() => $@"Collision Time
cur: {CurrentCollision}ms
avg: {TimeAverage}ms
high: {CollisionHigh}ms".Replace("\r", "")), new Vector2(300, 50), SKYBLUE));
    }

    public override void UpdateLoop()
    {
        if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
        {
            for (var i = 0; i < 100; i++) new Circle(mousePos);
        }
    }

    public override void RenderLoop() => DrawFPS(12, 12);
}