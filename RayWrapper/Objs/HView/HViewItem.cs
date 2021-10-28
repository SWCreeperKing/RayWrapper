using System.Numerics;

namespace RayWrapper.Objs.HView
{
    public abstract class HViewItem
    {
        public int id;

        public abstract Vector2 Size();
        public abstract void Draw(Vector2 pos);
    }
}