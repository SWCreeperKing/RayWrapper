using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.Primitives;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.LegacyUI.Slot;

public class RectItem : SlotItem
{
    public ColorModule color = new(128);

    public RectItem(Vector2 pos, Vector2 size) : base(pos, size)
    {
    }

    public override void Draw(Vector2 pos, Vector2 size, int alpha)
    {
        var (r, g, b, _) = color;
        new Rectangle(pos.X, pos.Y, size.X, size.Y).Draw(new Color(r,g,b,alpha));
    }
}