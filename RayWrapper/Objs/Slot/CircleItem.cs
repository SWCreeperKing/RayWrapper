using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base;
using Rectangle = RayWrapper.Base.Rectangle;

namespace RayWrapper.Objs.Slot;

public class CircleItem : SlotItem
{
    public ColorModule color = new(128);

    public CircleItem(Vector2 pos, Vector2 size) : base(pos, size)
    {
    }

    public override void Draw(Vector2 pos, Vector2 size, int alpha)
    {
        var (r, g, b, _) = color;
        new Rectangle(pos.X, pos.Y, size.X, size.Y).DrawRounded(new Color(r,g,b,alpha));
    }
}