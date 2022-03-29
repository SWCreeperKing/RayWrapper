using System.Numerics;
using Raylib_CsLo;
using RayWrapperTester;

namespace RayWrapper.Collision;

public class PhysicPolygon : PhysicObject
{
    public override CollisionBind CollisionBinding => CollisionBind.Circle;
    private (Vector2 pos, float radius) _circle;

    public PhysicPolygon(Vector2 pos, float radius = 1, int sides = 5, float density = 1)
    {
        _circle = (pos, radius);
        unsafe
        {
            body = *Physac.CreatePhysicsBodyPolygon(pos, radius, sides, density);
        }
    }

    public override (Vector2 pos, float radius) GetCircle() => _circle;
}