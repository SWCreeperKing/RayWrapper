using System.Numerics;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    public class ScrollView : GameObject
    {
        public override Vector2 Position { get; set; }
        public override Vector2 Size { get; }
        
        public override void UpdateCall()
        {
        }

        protected override void RenderCall()
        {
        }
    }
}