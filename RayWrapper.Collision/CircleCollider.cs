using System.Numerics;
using RayWrapper.Base.Primitives;

namespace RayWrapper.Collision;

public abstract class CircleCollider : Collider
{
    public Circle circle;

    protected CircleCollider(Circle circle) => this.circle = circle;

    public override float GetFurthestMostPointOnAxis(bool returnXAxis)
    {
        return (returnXAxis ? circle.position.X : circle.position.Y) + circle.radius;
    }

    public override float GetFurthestLeastPointOnAxis(bool returnXAxis)
    {
        return (returnXAxis ? circle.position.X : circle.position.Y) - circle.radius;
    }

    public override Vector2 GetCenter() => circle.position;

    public override Vector2 GetClosestPointTo(Vector2 pos)
    {
        if (!IsPositionInside(pos)) return pos;
        
        var dx = pos.X - circle.position.X;
        var dy = pos.Y - circle.position.Y;
        var angle = MathF.Atan2(dy, dx);
        var dxx = circle.radius * MathF.Cos(angle);
        var dyy = circle.radius * MathF.Sin(angle);

        var x = circle.position.X + dxx;
        var y = circle.position.Y + dyy;
        return new Vector2(x, y);
    }

    public override bool IsClosestPointIn(Vector2 pos) => DistanceCheck(pos);

    public bool DistanceCheck(Vector2 pos) // narrow phase
    {
        var deltaX = pos.X - circle.position.X;
        var deltaY = pos.Y - circle.position.Y;
        var rad = circle.radius;
        return deltaX * deltaX + deltaY * deltaY <= rad * rad;
    }
    
    public bool IsPositionInside(Vector2 pos) // narrow phase
    {
        var deltaX = pos.X - circle.position.X;
        var deltaY = pos.Y - circle.position.Y;
        var rad = circle.radius;
        return deltaX * deltaX + deltaY * deltaY >= rad * rad;
    }

    protected override Vector2 GetPosition() => circle.position;
    protected override void UpdatePosition(Vector2 newPos) => circle.position = newPos;
}