using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Objs;
using RayWrapper.Vars;

namespace RayWrapper.Animation.SinglePurposeObjects;

public class TextureAtlasDrawing : GameObject
{
    public TextureAtlas atlas;
    public string id;
    public Vector2? origin = null;
    public int rotation = 0;
    public Color? tint = null;

    public TextureAtlasDrawing(TextureAtlas atlas, string id)
    {
        this.id = id;
        this.atlas = atlas;
        size = new Vector2(atlas.pixelScale);
    }

    protected override void RenderCall()
    {
        atlas.Draw(id, pos.X, pos.Y, size.X, size.Y, origin, rotation, tint);
    }
}