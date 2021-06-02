using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace RayWrapper.Vars
{
    public abstract class GameObject : IRayObject
    {
        public Vector2 Position => initPosition + addedPosition;
        protected Vector2 initPosition;
        protected Vector2 addedPosition;

        public void MoveBy(Vector2 v2)
        {
            addedPosition += v2;
            PositionChange(Position);
        }

        public void MoveTo(Vector2 v2)
        {
            addedPosition = v2 - initPosition;
            PositionChange(Position);
        }

        public abstract void Update();
        public abstract void Render();
        public abstract void PositionChange(Vector2 v2);
    }
}