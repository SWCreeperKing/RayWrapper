using System;
using System.Linq;
using System.Numerics;
using Raylib_cs;
using RayWrapper.CollisionSystem;
using RayWrapper.Vars;
using static Raylib_cs.MouseButton;
using static Raylib_cs.Raylib;
using static RayWrapper.GameBox;

namespace RayWrapper.Objs.Slot
{
    public abstract class SlotItem : GameObject
    {
        public override Vector2 Position
        {
            get => rect.Pos();
            set => rect = rect.MoveTo(value);
        }

        public override Vector2 Size => rect.Size();

        public string id;
        public Vector2 beforeCords;
        public Rectangle rect;
        public SlotBase slot;
        public bool drawPhantom = true;
        public bool slotDependent = true;

        public SlotItem(Vector2 pos, Vector2 size) => rect = RectWrapper.AssembleRectFromVec(pos, size);

        public override void Update()
        {
            if (rect.IsMouseIn() && IsMouseButtonDown(MOUSE_LEFT_BUTTON) && !IsMouseOccupied)
            {
                mouseOccupier = this;
                beforeCords = rect.Pos();
            }
            else if (IsMouseButtonDown(MOUSE_LEFT_BUTTON) && mouseOccupier == this)
                rect = rect.MoveTo(mousePos + rect.Size() / 2);
            else if (mouseOccupier == this)
            {
                var candidate = dragCollision.FirstOrDefault(s => s.rect.IsMouseIn());

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

            if (mouseOccupier == this) Position = mousePos - Size / 2;
        }

        protected override void RenderCall()
        {
            var occ = mouseOccupier == this;
            Draw(Position, Size, occ && rect.IsMouseIn() && IsMouseButtonDown(MOUSE_LEFT_BUTTON) ? 128 : 255);
            if (occ && drawPhantom) Draw(beforeCords, Size, 32);
        }

        public abstract void Draw(Vector2 pos, Vector2 size, int alpha);

        // currentlySlotted is the slotitem that occupies the current slotbase
        // currentlySlotted is null if no slotitem is present in slotbase or cant be slotted
        public virtual void OnSlotAttempt(SlotItem currentlySlotted)
        {
            
        }
    }
}