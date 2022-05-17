using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;

namespace RayWrapper.Objs;

public class ImageObj : GameObject
{
    public override Vector2 Position
    {
        get => texture.Position;
        set => texture.Position = value;
    }

    public override Vector2 Size => texture.Size;

    public int ImageAlpha
    {
        get => texture.ImageAlpha;
        set => texture.ImageAlpha = value;
    } // transparency b/t 0-255

    public int Rotation
    {
        get => texture.rotation;
        set => texture.rotation = value;
    } // 0 to 360

    public Rectangle Rect => RectWrapper.AssembleRectFromVec(texture.Position, texture.Size);
    public TextureObj texture;
    
    private Image _image;
    private Rectangle _textSize;

    public ImageObj(string imageFile) : this(Raylib.LoadImage(imageFile), Vector2.Zero)
    {
    }

    public ImageObj(string imageFile, Vector2 pos) : this(Raylib.LoadImage(imageFile), pos)
    {
    }

    public ImageObj(Image image) : this(image, Vector2.Zero)
    {
    }

    public ImageObj(Image image, Vector2 pos)
    {
        _image = image;
        texture = new TextureObj(image.Texture(), pos);
        _textSize = RectWrapper.AssembleRectFromVec(Vector2.Zero, texture.Size);
    }

    protected override void UpdateCall() => texture.Update();
    protected override void RenderCall() => texture.Render();

    public void RenderTo(Rectangle rect, Color? tint = null, Vector2? origin = null, float rotation = 0)
    {
        Raylib.DrawTexturePro(texture, _textSize, rect, origin ?? Vector2.Zero, rotation, tint ?? Raylib.WHITE);
    }
    
    public void SetSize(Vector2 size) => texture.SetSize(size);

    public static implicit operator ImageObj(string path) => new(path);
    
    ~ImageObj() => Raylib.UnloadImage(_image);
}