using System.Numerics;

namespace RayWrapper.Objs.Slot
{
    public class Slot : SlotBase
    {
        public Slot(Vector2 pos, Vector2 size) : base(pos, size)
        {
        }

        protected override void Draw() => rect.DrawHallowRect(color, thickness);
    }
}