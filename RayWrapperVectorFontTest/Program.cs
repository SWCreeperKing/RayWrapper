// See https://aka.ms/new-console-template for more information

using System.Numerics;
using RayWrapper;
using RayWrapper.Vars;
using RayWrapper.VectorFont;

new GameBox(new Program(), new Vector2(1280, 720));

public partial class Program : GameLoop
{
    public override void Init()
    {
        RegisterGameObj(new VectorFont());
    }

    public override void UpdateLoop()
    {
    }

    public override void RenderLoop()
    {
    }
}