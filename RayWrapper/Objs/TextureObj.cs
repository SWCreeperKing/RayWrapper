using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Animation.SinglePurposeObjects;
using RayWrapper.Vars;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs;

public class TextureObj : GameObject
{
    public Rectangle SourceRect => _srcRect;

    public int ImageAlpha
    {
        get => tint.a;
        set => tint = tint.SetAlpha(value);
    } // transparency b/t 0-255

    public Vector2 origin = Vector2.Zero;
    public Color tint = new(255, 255, 255, 255);
    public int rotation; // 0 to 360

    private Texture _texture;
    private Rectangle _srcRect;
    private Rectangle _destRect;

    // if uses atlas
    private bool _usesAtlas;
    protected string _id;
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
        _srcRect = AssembleRectFromVec(Vector2.Zero, size = _texture.Size());
        Change();
    }

    protected override void RenderCall()
    {
        if (_usesAtlas) _atlas.Draw(_id, _destRect, origin, rotation % 360, tint);
        else Raylib.DrawTexturePro(_texture, _srcRect, _destRect, origin, rotation % 360, tint);
    }

    protected override void UpdatedSize(Vector2 newSize)
    {
        if (newSize == size) return;
        size = newSize;
        Change();
    }

    public static implicit operator Texture(TextureObj tObj) => tObj._texture;

    public void Change() => _destRect = AssembleRectFromVec(pos, size);

    ~TextureObj()
    {
        if (_usesAtlas) return;
        Raylib.UnloadTexture(_texture);
    }
}