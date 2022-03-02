using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs.Slot
{
    public abstract class SlotBase : GameObject
    {
        public override Vector2 Position
        {
            get => rect.Pos();
            set => rect.MoveTo(value);
        }

        public override Vector2 Size => rect.Size();

        public ColorModule color = new(102);
        public Actionable<string> idRestriction = null;
        public bool occupied = false;
        public Rectangle rect;
        public int thickness = 3;
        public SlotItem siOccupier = null;

        public SlotBase(Vector2 pos, Vector2 size)
        {
            rect = AssembleRectFromVec(pos, size);
            if (!GameBox.dragCollision.Contains(this)) GameBox.dragCollision.Add(this);
        }

        protected override void UpdateCall()
        {
        }

        protected override void RenderCall() => Draw();

        protected abstract void Draw();
    }
}