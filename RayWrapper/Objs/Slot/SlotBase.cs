using System.Numerics;
using RayWrapper.Base;
using RayWrapper.Base.GameObject;

namespace RayWrapper.Objs.Slot;

public abstract class SlotBase : GameObject
{
    public ColorModule color = new(102);
    public Actionable<string> idRestriction = null;
    public bool occupied = false;
    public int thickness = 3;
    public SlotItem siOccupier = null;

    public SlotBase(Vector2 pos, Vector2 size)
    {
        this.pos = pos;
        this.size = size;
        if (!GameBox.dragCollision.Contains(this)) GameBox.dragCollision.Add(this);
    }

    protected override void RenderCall() => Draw();

    protected abstract void Draw();
}