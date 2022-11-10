using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.Extras;
using RayWrapper.Base.GameObject;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;
using Texture = RayWrapper.Base.Primitives.Texture;

namespace RayWrapper.LegacyUI.UI;

// todo: see if its possible to add support for SixLabors.ImageSharp (can probs make RayWrapper.ImageProcessing)
public class ImageObj : GameObject
{
    public readonly string Path;

    public int ImageAlpha
    {
        get => texture.tint.a;
        set => texture.tint = texture.tint with { a = (byte) value };
    }

    public Texture texture;

    private Image _image;
    private Rectangle _rect;

    public ImageObj(string imageFile, Vector2? pos = null) : this(Raylib.LoadImage(imageFile), pos ?? Vector2.Zero)
    {
        Path = imageFile;
    }

    public ImageObj(Image image, Vector2? pos = null)
    {
        Position = pos ?? Vector2.Zero;
        Size = image.Size();
        _rect = new Rectangle(Vector2.Zero, Size);
        Path = null;
        _image = image;
        texture = image.Texture();
    }

    public ImageObj(ImageObj imageObj, Vector2? pos = null)
    {
        if (imageObj.Path is null || !File.Exists(imageObj.Path))
        {
            throw new ArgumentException($"ImageObj given does not have a valid path: {imageObj.Path}");
        }

        Position = pos ?? Vector2.Zero;
        Size = imageObj.Size;
        _rect = imageObj._rect;
        Path = imageObj.Path;
        _image = Raylib.LoadImage(imageObj.Path);
        texture = imageObj.texture;
    }

    protected override void RenderCall() => RenderTo(GetRect());

    public void RenderTo(Rectangle rect, Vector2? origin = null, float rotation = 0)
    {
        texture.Draw(_rect, rect, origin ?? Vector2.Zero, rotation);
    }

    public static implicit operator ImageObj(string path) => new(path);
    ~ImageObj() => Raylib.UnloadImage(_image);
}