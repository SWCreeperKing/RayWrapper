using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.GameObject;
using Texture = RayWrapper.Base.Texture;
using Rectangle = RayWrapper.Base.Rectangle;

namespace RayWrapper.Objs;

public class TextureObj : GameObject
{
    public Rectangle SourceRect { get; }

    public int ImageAlpha
    {
        get => Tint.a;
        set => Tint = Tint.SetAlpha(value);
    } // transparency b/t 0-255

    public Color Tint
    {
        get => _texture.tint;
        set => _texture.tint = value;
    }

    public Vector2 origin = Vector2.Zero;
    public int rotation; // 0 to 360

    private Texture _texture;
    private Rectangle _destRect;

    public TextureObj(Texture texture, Vector2 pos)
    {
        _texture = texture;
        SourceRect = new Rectangle(Vector2.Zero, Size = _texture.Texture2D.Size());
        _destRect = new Rectangle(pos, Size);
    }

    protected override void RenderCall() => _texture.Draw(SourceRect, _destRect, origin, rotation % 360);
    protected override void UpdatePosition(Vector2 newPos) => _destRect.Pos = newPos;
    protected override void UpdateSize(Vector2 newSize) => _destRect.Size = newSize;

    public static implicit operator Texture(TextureObj tObj) => tObj._texture;
}