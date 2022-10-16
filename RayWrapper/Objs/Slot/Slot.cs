using System.Numerics;
using Rectangle = RayWrapper.Base.Rectangle;

namespace RayWrapper.Objs.Slot;

public class Slot : SlotBase
{
    public Slot(Vector2 pos, Vector2 size) : base(pos, size)
    {
    }

    protected override void Draw() => new Rectangle(Position, Size).DrawLines(color, thickness);
}