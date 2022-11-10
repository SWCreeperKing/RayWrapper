using System.Numerics;
using RayWrapper.Base.GameObject;
using RayWrapper.Base.Primitives;

namespace RayWrapper.LegacyUI.Slot;

public abstract class SlotBase : GameObject
{
    public static List<SlotBase> dragCollision = new();
    
    public ColorModule color = new(102);
    public Actionable<string> idRestriction = null;
    public bool occupied = false;
    public int thickness = 3;
    public SlotItem siOccupier = null;

    public SlotBase(Vector2 pos, Vector2 size)
    {
        this.pos = pos;
        this.size = size;
        if (!dragCollision.Contains(this)) dragCollision.Add(this);
    }

    protected override void RenderCall() => Draw();

    protected abstract void Draw();
}