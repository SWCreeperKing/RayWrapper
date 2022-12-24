using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.GameBox;
using RayWrapper.Base.Primitives;
using RayWrapper.Collision;
using RayWrapper.LegacyUI.UI;
using RayWrapperTesterCollision;
using static Raylib_CsLo.Raylib;
using static RayWrapper.Base.GameBox.GameBox;

new GameBox(new Program(), new Vector2(1280, 720), "collision testing", 500);

public partial class Program : GameLoop
{
    public CollisionManager cm = new(CollisionManager.MetaCollisionMethod.SweepAndPruneX);

    public override void Init()
    {
        var wx = WindowSize.X;
        var wy = WindowSize.Y;

        cm.colliders.AddRange(new[]
        {
            new Bar(new Vector2(0), new Vector2(wx, 10)),
            new Bar(new Vector2(0, wy - 10), new Vector2(wx, 10)),
            new Bar(new Vector2(0), new Vector2(10, wy)) { vert = true },
            new Bar(new Vector2(wx - 10, 0), new Vector2(10, wy)) { vert = true }
        });

        // new Bar(new Vector2(400), new Vector2(600, 1));
        RegisterGameObj(new Text(new Actionable<string>(() => $"{Collider.ColliderCount - 4:###,###}"),
            new Vector2(12, 60), RED));
//             RegisterGameObj(new Text(new Actionable<string>(() => $"{CountColliders() - 4}"), new Vector2(12, 60), RED),
//                 new Text(
//                     new Actionable<string>(() => $@"Collision Time
// cur: {CurrentCollision}ms
// avg: {TimeAverage}ms
// high: {CollisionHigh}ms".Replace("\r", "")), new Vector2(300, 50), SKYBLUE));
    }

    public override void UpdateLoop(float dt)
    {
        if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
        {
            for (var i = 0; i < 10; i++) cm.colliders.Add(new Ball(Input.MousePosition.currentPosition));
        }

        cm.UpdateColliders(dt);
    }

    public override void RenderLoop()
    {
        DrawFPS(12, 12);
        cm.RenderColliders();
    }
}