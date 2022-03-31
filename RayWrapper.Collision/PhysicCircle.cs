using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static RayWrapper.Collision.ICollider;

namespace RayWrapper.Collision;

public class PhysicCircle : PhysicObject
{
    public override CollisionBind CollisionBinding => CollisionBind.Circle;
    private Circle _circle;

    public PhysicCircle(Vector2 pos, float radius = 10, float density = 10, bool isStatic = false)
    {
        _circle = new Circle(pos, radius);
        unsafe
        {
            body = *Physac.CreatePhysicsBodyCircle(pos, radius, density);
            body.enabled = !isStatic;
        }
    }

    public override void SetObjPosition(Vector2 pos) => _circle.position = pos;
    public override Circle GetCircle() => _circle;
    protected override void RenderCall() => _circle.Draw();
}