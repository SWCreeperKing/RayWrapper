using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;

namespace RayWrapper.Objs.Slot
{
    public class Slot : GameObject
    {
        public Rectangle rect;
        public ColorModule color = new(102);
        public Actionable<string> idRestriction = null; 
        public int thickness = 3;
        public bool occupied = false;
        
        public Slot(Vector2 pos, Vector2 size) : base(pos)
        {
            rect = RectWrapper.AssembleRectFromVec(pos, size);
            if (!GameBox.dragCollision.Contains(this)) GameBox.dragCollision.Add(this);
        }

        public override void Update()
        {
        }

        protected override void RenderCall() => rect.DrawHallowRect(color, thickness);
        public override void PositionChange(Vector2 v2) => rect = rect.MoveTo(v2);
        public override Vector2 Size() => rect.Size();
    }
}