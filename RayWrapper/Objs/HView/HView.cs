using System.Numerics;
using RayWrapper.Vars;

namespace RayWrapper.Objs.HView
{
    public class HView<T> : GameObject where T : HViewItem
    {
        public override Vector2 Position { get; set; }
        public override Vector2 Size { get; }
        
        protected override void UpdateCall()
        {
        }

        protected override void RenderCall()
        {
        }
    }
}