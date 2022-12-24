using RayWrapper.Base.GameBox;

namespace RayWrapper.Collision;

public class CollisionManager
{
    public enum MetaCollisionMethod
    {
        SweepAndPruneX,
        SweepAndPruneY
    }

    public readonly List<Collider> colliders = new();
    public Action collisionMethod;

    private readonly List<Collider> _intervalList = new();

    public CollisionManager(MetaCollisionMethod collisionMethod)
    {
        this.collisionMethod = collisionMethod switch
        {
            MetaCollisionMethod.SweepAndPruneX => () => SweepAndPrune(),
            MetaCollisionMethod.SweepAndPruneY => () => SweepAndPrune(false),
            _ => throw new ArgumentOutOfRangeException(nameof(collisionMethod), collisionMethod, null)
        };
    }

    public CollisionManager(Action collisionMethod)
    {
        this.collisionMethod = collisionMethod;
        Logger.Log(Logger.Level.Warning, "COLLISION IS WIP [DOES NOT WORK 100%]");
    }

    public void UpdateColliders(float dt)
    {
        collisionMethod();
        colliders.RemoveAll(c => c.ToDestroy());
        colliders.ForEach(c => c.Update(dt));
    }

    public void RenderColliders() => colliders.ForEach(c => c.Render());

    public void SweepAndPrune(bool useX = true)
    {
        _intervalList.Clear();

        float axisPos;
        foreach (var collider in colliders)
        {
            if (!_intervalList.Any())
            {
                _intervalList.Add(collider);
                continue;
            }

            axisPos = collider.GetFurthestLeastPointOnAxis(useX);
            _intervalList.RemoveAll(c => c.GetFurthestMostPointOnAxis(useX) < axisPos);

            if (_intervalList.Any())
            {
                foreach (var collidee in _intervalList)
                {
                    var closestFromTo = collider.GetClosestPointTo(collidee.GetCenter());
                    var closestToFrom = collidee.GetClosestPointTo(collider.GetCenter());

                    if (collider.IsClosestPointIn(closestToFrom) || collidee.IsClosestPointIn(closestFromTo))
                    {
                        collider.OnCollisionEnter(collidee);
                        collidee.OnCollisionEnter(collider);
                    }
                }
            }

            _intervalList.Add(collider);
        }

        _intervalList.Clear();
    }
}