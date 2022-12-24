using System.Numerics;
using RayWrapper.Base.Primitives;

namespace RayWrapper.Collision;

public abstract class RectangleCollider : Collider
{
    public Rectangle rect;

    public RectangleCollider(Rectangle rect) => this.rect = rect;

    public override float GetFurthestMostPointOnAxis(bool returnXAxis)
    {
        return (returnXAxis ? rect.W : rect.H) + GetFurthestLeastPointOnAxis(returnXAxis);
    }

    public override float GetFurthestLeastPointOnAxis(bool returnXAxis)
    {
        return returnXAxis ? rect.X : rect.Y;
    }

    public override Vector2 GetCenter()
    {
        return new Vector2(rect.X + rect.W / 2f, rect.Y + rect.H / 2f);
    }

    public override Vector2 GetClosestPointTo(Vector2 pos)
    {
        var x = pos.X;
        if (x < rect.X) x = rect.X;
        else if (x > rect.X + rect.W) x = rect.X + rect.W;

        var y = pos.Y;
        if (y < rect.Y) y = rect.Y;
        else if (y > rect.Y + rect.H) y = rect.Y + rect.H;

        return new Vector2(x, y);
    }

    public override bool IsClosestPointIn(Vector2 pos)
    {
        return rect.X <= pos.X && rect.X + rect.W >= pos.X && rect.Y <= pos.Y && rect.Y + rect.H >= pos.Y;
    }

    protected override Vector2 GetPosition() => rect.Pos;
    protected override Vector2 GetSize() => rect.Size;
    protected override void UpdatePosition(Vector2 newPos) => rect.Pos = newPos;
    protected override void UpdateSize(Vector2 newSize) => rect.Size = newSize;
}