using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs;

public class TextureObj : GameObject
{
    public Rectangle SourceRect { get; }

    public int ImageAlpha
    {
        get => tint.a;
        set => tint = tint.SetAlpha(value);
    } // transparency b/t 0-255

    public Vector2 origin = Vector2.Zero;
    public Color tint = new(255, 255, 255, 255);
    public int rotation; // 0 to 360

    private Texture _texture;
    private Rectangle _destRect;

    // if uses atlas
    private string _id;
    private bool _usesAtlas;
    private TextureAtlas _atlas;

    public TextureObj(string id, TextureAtlas atlas)
    {
        _id = id;
        _atlas = atlas;
        _usesAtlas = true;
    }

    public TextureObj(Texture texture, Vector2 pos)
    {
        (_texture, this.pos, _usesAtlas) = (texture, pos, false);
        SourceRect = AssembleRectFromVec(Vector2.Zero, size = _texture.Size());
        _destRect = AssembleRectFromVec(pos, size);
    }

    protected override void RenderCall()
    {
        if (_usesAtlas) _atlas.Draw(_id, _destRect, origin, rotation % 360, tint);
        else Raylib.DrawTexturePro(_texture, SourceRect, _destRect, origin, rotation % 360, tint);
    }

    
    protected override void UpdatePosition(Vector2 newPos) => _destRect = AssembleRectFromVec(pos = newPos, size);
    protected override void UpdateSize(Vector2 newSize) => _destRect = AssembleRectFromVec(pos, size = newSize);

    public static implicit operator Texture(TextureObj tObj) => tObj._texture;

    ~TextureObj()
    {
        if (_usesAtlas) return;
        Raylib.UnloadTexture(_texture);
    }
}