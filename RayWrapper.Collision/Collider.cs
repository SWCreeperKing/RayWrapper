using System.Numerics;
using RayWrapper.Base.GameObject;

namespace RayWrapper.Collision;

public abstract class Collider : GameObject, IDisposable
{
    public static int ColliderCount;
    // public string tag = "default";

    protected Collider()
    {
        ColliderCount++;
    }

    /// <summary>
    /// Get highest axis position
    /// </summary>
    public abstract float GetFurthestMostPointOnAxis(bool returnXAxis);

    /// <summary>
    /// Get lowest axis position
    /// </summary>
    public abstract float GetFurthestLeastPointOnAxis(bool returnXAxis);

    public abstract Vector2 GetCenter();
    public abstract Vector2 GetClosestPointTo(Vector2 pos);
    public abstract bool IsClosestPointIn(Vector2 pos);

    public virtual void OnCollisionEnter(Collider c)
    {
    }

    // todo: later down the line
    // public virtual void OnCollisionContinue(Collider c)
    // {
    // }
    //
    // public virtual void OnCollisionExit(Collider c)
    // {
    // }

    public virtual bool ToDestroy()
    {
        return false;
    }

    public virtual void OnDestroy()
    {
    }

    ~Collider()
    {
        ColliderCount--;
        OnDestroy();
    }
}