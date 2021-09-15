using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    public class Graph : GameObject
    {
        public Rectangle rect;

        public Graph(Rectangle rect) : base(rect.Pos()) => this.rect = rect;

        public override void Update()
        {
        }

        protected override void RenderCall()
        {
        }

        public override void PositionChange(Vector2 v2)
        {
        }

        public override Vector2 Size() => rect.Size();

        public void UpdateGraphValues()
        {
        }
    }
}