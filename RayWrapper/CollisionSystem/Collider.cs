using System.Numerics;
using RayWrapper.Vars;

namespace RayWrapper.CollisionSystem
{
    public abstract class Collider : GameObject
    {
        public override Vector2 Position { get => _pos; set => _pos = value; }

        public static long count;
        public static long id;
        public Vector2 velocity = Vector2.Zero;
        public long currentId;
        public string layer;

        private Vector2 _pos;
        
        protected Collider(string layerId, Vector2 pos)
        {
            (_pos, layer, currentId) = (pos, layerId, id);
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