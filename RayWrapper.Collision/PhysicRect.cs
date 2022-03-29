using Raylib_CsLo;
using RayWrapperTester;

namespace RayWrapper.Collision;

public class PhysicRect : PhysicObject
{
    public override CollisionBind CollisionBinding => CollisionBind.Rect;
    private Rectangle _rect;

    public PhysicRect(Rectangle rect, float density = 1)
    {
        _rect = rect;
        unsafe
        {
            body = *Physac.CreatePhysicsBodyRectangle(rect.Pos(), rect.width, rect.height, density);
        }
    }

    public override Rectangle GetRect() => _rect;
}