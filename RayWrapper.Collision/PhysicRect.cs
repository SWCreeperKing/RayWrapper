using System.Numerics;
using Raylib_CsLo;
using static RayWrapper.Collision.ICollider;

namespace RayWrapper.Collision;

public class PhysicRect : PhysicObject
{
    public override CollisionBind CollisionBinding => CollisionBind.Rect;
    private Rectangle _rect;

    public PhysicRect(Rectangle rect, float density = 1, bool isStatic = false)
    {
        _rect = rect;
        unsafe
        {
            body = *Physac.CreatePhysicsBodyRectangle(rect.Pos() + rect.Size()/2, rect.width, rect.height, density);
            body.enabled = !isStatic;
        }
    }

    public override void SetObjPosition(Vector2 pos) => _rect.MoveTo(pos);
    public override Rectangle GetRect() => _rect;
}