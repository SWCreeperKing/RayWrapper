using System.Linq;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;
using static RayWrapper.GameBox;

namespace RayWrapper.Objs.Slot;

public abstract class SlotItem : GameObject
{
    public string id;
    public Vector2 beforeCords;
    public SlotBase slot;
    public bool slotDependent = true;

    public SlotItem(Vector2 pos, Vector2 size) => (this.pos, this.size) = (pos, size);

    protected override void UpdateCall()
    {
        var rect = GetRect();
        if (rect.IsMouseIn() && IsMouseButtonDown(MOUSE_LEFT_BUTTON) && !IsMouseOccupied)
        {
            mouseOccupier = this;
            beforeCords = rect.Pos();
        }
        else if (mouseOccupier == this && IsMouseButtonUp(MOUSE_LEFT_BUTTON))
            SlotThis(dragCollision.FirstOrDefault(s => s.GetRect().IsMouseIn()));

        if (mouseOccupier == this) Position = mousePos - Size / 2;
    }

    protected override void RenderCall()
    {
        var rect = GetRect();
        var occ = mouseOccupier == this;
        Draw(beforeCords, Size, occ && rect.IsMouseIn() && IsMouseButtonDown(MOUSE_LEFT_BUTTON) ? 128 : 255);
        if (occ) Draw(Position, Size, 32);
    }

    public abstract void Draw(Vector2 pos, Vector2 size, int alpha);

    public void SlotThis(SlotBase candidate)
    {
        if (candidate is not null)
        {
            var idRest = candidate.idRestriction;
            if (!candidate.occupied && (idRest is null || idRest == id))
            {
                if (slot != null)
                {
                    slot.occupied = false;
                    slot.siOccupier = null;
                    slot = null;
                }

                slot = candidate;
                slot.occupied = true;
                slot.siOccupier = this;
                Position = slot.Position;
                OnSlotAttempt(null);
            }
            else
            {
                OnSlotAttempt(candidate.occupied ? candidate.siOccupier : null);
                Position = beforeCords;
            }
        }
        else if (slotDependent) Position = beforeCords;
        else if (slot != null)
        {
            slot.occupied = false;
            slot = null;
        }

        mouseOccupier = null;
    }

    public void SetSlot(SlotBase slot)
    {
        if (slot.occupied || slot.idRestriction is not null && slot.idRestriction == id) return;
        this.slot = slot;
        slot.occupied = true;
        slot.siOccupier = this;
        Position = slot.Position;
    }

    // currentlySlotted is the slotitem that occupies the current slotbase
    // currentlySlotted is null if no slotitem is present in slotbase or cant be slotted
    public virtual void OnSlotAttempt(SlotItem currentlySlotted)
    {
    }

    ~SlotItem()
    {
        if (slot is null) return;
        slot.occupied = false;
        slot.siOccupier = null;
    }
}