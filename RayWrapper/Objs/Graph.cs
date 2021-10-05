using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    public class Graph : GameObject
    {
        public Rectangle rect;
        public Graph(Rectangle rect) => this.rect = rect;

        // todo: eventually

        public override Vector2 Position
        {
            get => rect.Pos();
            set => rect = rect.MoveTo(value);
        }

        public override Vector2 Size => rect.Size();

        public override void Update()
        {
        }

        protected override void RenderCall()
        {
        }

        public void UpdateGraphValues()
        {
        }
    }
}