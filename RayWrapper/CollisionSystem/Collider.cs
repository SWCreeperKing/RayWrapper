using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;

namespace RayWrapper.CollisionSystem
{
    public abstract class Collider : GameObject
    {
        public static long count;
        public static long id;
        public Vector2 velocity = Vector2.Zero;
        public long currentId = 0;
        public string layer;
        
        protected Collider(string layerId, Vector2 pos) : base(pos)
        {
            layer = layerId;
            currentId = id;
            GameBox.AddColliders(this);
            id++;
            count++;
        }

        public override void Update()
        {
        }

        protected override void RenderCall()
        {
            if (Position.X < 0 || Position.Y < 0 || GameBox.WindowSize.X < Position.X ||
                GameBox.WindowSize.Y < Position.Y)
            {
                Dispose();
                GameBox.RemoveColliders(this);
                count--;
            }
            else RenderShape(Position);
        }

        public override void PositionChange(Vector2 v2)
        {
        }

        public abstract bool CheckCollision(Collider c);
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
    }
}