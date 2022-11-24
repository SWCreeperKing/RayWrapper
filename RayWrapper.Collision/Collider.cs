using System.Numerics;
using RayWrapper.Base.GameObject;
using static RayWrapper.Collision.Collision;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.Collision;

public abstract class Collider : GameObject
{
    public static long id;
    public Vector2 velocity = Vector2.Zero;

    public long currentId;
    public string tag = "def";

    /// <summary>
    /// if true, will delete object when it reaches the out of bounds of the screen
    /// true by default
    /// </summary>
    public bool removeWhenOutOfBounds = true;

    private readonly List<Collider> _buffer = new();
    private readonly List<Collider> _backBuffer = new();

    protected Collider(Vector2 pos)
    {
        (this.pos, currentId) = (pos, id);
        AddObject(this);
        id++;
    }

    public void PhysicUpdate(float dt) => pos += velocity * dt;

    public void DoCollision(Collider c, bool chain = true)
    {
        if (!CheckCollision(c)) return;
        if (!_buffer.Contains(c)) _buffer.Add(c);
        if (chain) c.DoCollision(this, false);
    }

    public void PostCollision()
    {
        foreach (var c in _buffer)
        {
            if (_buffer.Contains(c) && !_backBuffer.Contains(c)) FirstCollision(c);
            if (_buffer.Contains(c) && _backBuffer.Contains(c)) InCollision(c);
        }

        foreach (var c in _backBuffer)
        {
            // to reduce mem allow, as `Where` will cause like 100mb of extra ram xd
            if (!_buffer.Contains(c) && _backBuffer.Contains(c)) ExitCollision(c);
        }

        _backBuffer.Clear();
        foreach (var c in _buffer) _backBuffer.Add(c);
        _buffer.Clear();

        if (!removeWhenOutOfBounds) return;
        if (Position.X >= 0 && Position.Y >= 0 && Position.X <= GameBox.WindowSize.X &&
            Position.Y <= GameBox.WindowSize.Y) return;

        DestoryObject();
    }

    protected override void RenderCall() => RenderShape(Position);
    public abstract bool CheckCollision(Collider c);
    public abstract bool SampleCollision(Rectangle c);
    public abstract void RenderShape(Vector2 pos);

    public virtual void FirstCollision(Collider c)
    {
    }

    public virtual void InCollision(Collider c)
    {
    }

    public virtual void ExitCollision(Collider c)
    {
    }

    public virtual void Dispose()
    {
    }

    public void DestoryObject()
    {
        Dispose();
        RemoveObject(this);
    }

    ~Collider() => DestoryObject();
}