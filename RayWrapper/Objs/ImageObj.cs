using System;
using System.IO;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.GameObject;

namespace RayWrapper.Objs;

// todo: see if its possible to add support for SixLabors.ImageSharp (can probs make RayWrapper.ImageProcessing)
public class ImageObj : GameObject
{
    public readonly string Path;

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

    public ImageObj(string imageFile, Vector2? pos = null) : this(Raylib.LoadImage(imageFile), pos ?? Vector2.Zero)
    {
        Path = imageFile;
    }

    public ImageObj(Image image, Vector2? pos = null)
    {
        Path = null;
        _image = image;
        texture = new TextureObj(image.Texture(), pos ?? Vector2.Zero);
    }

    public ImageObj(ImageObj imageObj, Vector2? pos = null)
    {
        if (imageObj.Path is null || !File.Exists(imageObj.Path))
        {
            throw new ArgumentException($"ImageObj given does not have a valid path: {imageObj.Path}");
        }

        Path = imageObj.Path;
        _image = Raylib.LoadImage(imageObj.Path);
        texture = new TextureObj(_image.Texture(), pos ?? Vector2.Zero);
    }

    protected override void RenderCall() => texture.Render();

    public void RenderTo(Rectangle rect, Color? tint = null, Vector2? origin = null, float rotation = 0)
    {
        Raylib.DrawTexturePro(texture, texture.SourceRect, rect, origin ?? Vector2.Zero, rotation,
            tint ?? Raylib.WHITE);
    }

    protected override Vector2 GetPosition() => texture.Position;
    protected override Vector2 GetSize() => texture.Size;
    protected override void UpdatePosition(Vector2 newPos) => texture.Position = newPos;
    protected override void UpdateSize(Vector2 newSize) => texture.Size = newSize;

    public static implicit operator ImageObj(string path) => new(path);
    ~ImageObj() => Raylib.UnloadImage(_image);
}