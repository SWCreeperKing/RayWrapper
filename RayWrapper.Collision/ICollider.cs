namespace RayWrapper.Collision;

public interface ICollider
{
    public enum CollisionBind
    {
        Circle,
        Rect
    }

    public CollisionBind CollisionBinding { get; }
}