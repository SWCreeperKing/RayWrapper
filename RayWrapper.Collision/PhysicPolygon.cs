using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static RayWrapper.Collision.ICollider;

namespace RayWrapper.Collision;

public class PhysicPolygon : PhysicObject
{
    public override CollisionBind CollisionBinding => CollisionBind.Circle;
    private Circle _circle;

    public PhysicPolygon(Vector2 pos, float radius = 10, int sides = 5, float density = 10, bool isStatic = false)
    {
        _circle = new Circle(pos, radius);
        unsafe
        {
            body = *Physac.CreatePhysicsBodyPolygon(pos, radius, sides, density);
            body.enabled = !isStatic;
        }
    }

    public override void SetObjPosition(Vector2 pos) => _circle.position = pos;
    public override Circle GetCircle() => _circle;
}