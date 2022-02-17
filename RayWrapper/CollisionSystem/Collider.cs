using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;

namespace RayWrapper.CollisionSystem
{
    public abstract class Collider : GameObject
    {
        public override Vector2 Position
        {
            get => _pos;
            set => _pos = value;
        }

        public static long count;
        public static long id;
        public Vector2 velocity = Vector2.Zero;
        public long currentId;
        public string layer;

        private Vector2 _pos;
        private IList<Collider> collisionMem = new List<Collider>();
        private IList<Collider> clearMem = new List<Collider>();

        protected Collider(string layerId, Vector2 pos)
        {
            (_pos, layer, currentId) = (pos, layerId, id);
            GameBox.screenGrid.SubscribeCollider(this);
            id++;
            count++;
        }

        protected override void UpdateCall()
        {
            foreach (var c in collisionMem.Where(c => !CheckCollision(c)))
            {
                ExitCollision(c);
                clearMem.Add(c);
            }

            foreach (var c in clearMem) collisionMem.Remove(c);
            clearMem.Clear();

            if (!(Position.X < 0) && !(Position.Y < 0) && !(GameBox.WindowSize.X < Position.X) &&
                !(GameBox.WindowSize.Y < Position.Y)) return;

            Dispose();
            GameBox.screenGrid.UnSubscribeCollider(this);
            count--;
        }

        public void DoCollision(Collider c)
        {
            if (!CheckCollision(c)) return;
            if (!collisionMem.Contains(c))
            {
                FirstCollision(c);
                collisionMem.Add(c);
            }
            else InCollision(c);
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
    }
}