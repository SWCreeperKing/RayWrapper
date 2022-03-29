using System.Numerics;
using Raylib_CsLo;
using RayWrapperTester;

namespace RayWrapper.Collision;

public class PhysicCircle : PhysicObject
{
    public override CollisionBind CollisionBinding => CollisionBind.Circle;
    private (Vector2 pos, float radius) _circle;

    public PhysicCircle(Vector2 pos, float radius = 1, float density = 1)
    {
        _circle = (pos, radius);
        unsafe
        {
            body = *Physac.CreatePhysicsBodyCircle(pos, radius, density);
        }
    }

    public override (Vector2 pos, float radius) GetCircle() => _circle;
}