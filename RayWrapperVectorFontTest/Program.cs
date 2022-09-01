// See https://aka.ms/new-console-template for more information

using System.Numerics;
using Raylib_CsLo;
using RayWrapper;
using RayWrapper.Objs;
using RayWrapper.Vars;
using RayWrapper.VectorFont;

new GameBox(new Program(), new Vector2(1280, 720));

[Obsolete("Looks good at big sizes, but not small, might remake later for normal graphical use and not just font use")]
public partial class Program : GameLoop
{
    public override void Init()
    {
        GameBox.targetTextureFilter = TextureFilter.TEXTURE_FILTER_POINT;
        RegisterGameObj(new VectorFont(), new Text("b", GameBox.WindowSize / 2 + new Vector2(-30, 0)));
    }

    public override void UpdateLoop()
    {
    }

    public override void RenderLoop()
    {
    }
}