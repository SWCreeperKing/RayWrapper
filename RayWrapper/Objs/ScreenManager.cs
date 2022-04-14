using System.Numerics;
using RayWrapper.Vars;

namespace RayWrapper.Objs;

public class ScreenManager : GameObject
{
    public override Vector2 Position { get; set; }
    public override Vector2 Size { get; }

    public ScreenManager()
    {
        
    }
    
    protected override void UpdateCall()
    {
    }

    protected override void RenderCall()
    {
    }
}