using System.Linq;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static Raylib_cs.MouseButton;
using static Raylib_cs.Raylib;
using static RayWrapper.GameBox;

namespace RayWrapper.Objs.Slot
{
    public abstract class Slotable : GameObject
    {
        public Rectangle rect;
        public Vector2 baseSize;
        public Vector2 beforeCords;
        public Slot slot;

        public Slotable(Vector2 pos, Vector2 size) : base(pos) =>
            rect = RectWrapper.AssembleRectFromVec(pos, baseSize = size);

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
                var candidates = dragCollision.Where(s => s.rect.IsMouseIn() && !s.occupied);
                if (!candidates.Any())
                rect = rect.MoveTo(slot.rect.Pos());
                mouseOccupier = null;
            }
        }

        protected override void RenderCall()
        {
        }

        public override void PositionChange(Vector2 v2) => rect = rect.MoveTo(v2);
        public override Vector2 Size() => rect.Size();
        public abstract void Draw(Vector2 pos, Vector2 size, int alpha);
    }
}