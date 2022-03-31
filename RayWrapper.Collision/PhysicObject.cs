using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;

namespace RayWrapper.Collision;

public abstract class PhysicObject : GhostObject, ICollider
{
    public override Vector2 Position
    {
        get => body.position;
        set => body.position = value;
    }

    public bool IsStatic
    {
        get => body.enabled;
        set => body.enabled = !value;
    }

    public abstract ICollider.CollisionBind CollisionBinding { get; }

    public ColorModule color = RED;
    public PhysicsBodyData body;

    public PhysicObject()
    {
        Starter.CollisionObjects.Add(this);
    }

    protected override void UpdateCall() => SetObjPosition(Position);
    protected override void RenderCall() => DebugDraw();

    public void DebugDraw()
    {
        var vertexData = body.shape.vertexData;
        var vertexCount = vertexData.vertexCount;
        for (var vertexIndex = 0; vertexIndex < vertexCount; vertexIndex++)
        {
            var v1 = body.GetVertex(vertexIndex);
            var plus1 = vertexIndex + 1;
            var v2 = body.GetVertex(plus1 == vertexCount ? 0 : plus1);
            v1.DrawLine(v2, color);
        }
    }

    public bool HasCollision(Rectangle rect) =>
        CollisionBinding switch
        {
            ICollider.CollisionBind.Circle => GetCircle().CheckCollision(rect),
            ICollider.CollisionBind.Rect => CheckCollisionRecs(GetRect(), rect),
            _ => false
        };

    public bool HasCollision(PhysicObject obj) =>
        CollisionBinding switch
        {
            ICollider.CollisionBind.Circle when obj.CollisionBinding is ICollider.CollisionBind.Circle =>
                GetCircle().CheckCollision(obj.GetCircle()),
            ICollider.CollisionBind.Circle when obj.CollisionBinding is ICollider.CollisionBind.Rect =>
                GetCircle().CheckCollision(obj.GetRect()),
            ICollider.CollisionBind.Rect when obj.CollisionBinding is ICollider.CollisionBind.Circle =>
                obj.GetCircle().CheckCollision(GetRect()),
            ICollider.CollisionBind.Rect when obj.CollisionBinding is ICollider.CollisionBind.Rect =>
                CheckCollisionRecs(GetRect(), obj.GetRect()),
            _ => false
        } && ExtraCollisionChecks();


    public void ExecuteCollision(PhysicObject obj, bool chain = true)
    {
        // todo: collision events
        if (chain) obj.ExecuteCollision(this, false);
    }

    public abstract void SetObjPosition(Vector2 pos);
    public virtual Rectangle GetRect() => RectWrapper.Zero;
    public virtual Circle GetCircle() => Circle.Zero;
    public virtual bool ExtraCollisionChecks() => true;

    ~PhysicObject()
    {
        Starter.CollisionObjects.Remove(this);
        unsafe
        {
            fixed (PhysicsBodyData* bData = &body) Physac.DestroyPhysicsBody(bData);
        }
    }
}