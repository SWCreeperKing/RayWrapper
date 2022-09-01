using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Objs;
using RayWrapper.Vars;

namespace RayWrapper.Animation.SinglePurposeObjects;

public class TextureAtlasDrawing : GameObject
{
    public override Vector2 Position
    {
        get => _pos;
        set => _pos = value;
    }

    public override Vector2 Size => size ?? new Vector2(atlas.pixelScale);

    public TextureAtlas atlas;
    public string id;
    public Vector2? size = null;
    public Vector2? origin = null;
    public int rotation = 0;
    public Color? tint = null;
    
    private Vector2 _pos;

    public TextureAtlasDrawing(TextureAtlas atlas, string id)
    {
        this.id = id;
        this.atlas = atlas;
    }

    protected override void UpdateCall()
    {
    }

    protected override void RenderCall()
    {
        atlas.Draw(id, _pos.X, _pos.Y, size?.X, size?.Y, origin, rotation, tint);
    }
}