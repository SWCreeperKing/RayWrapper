using System;
using System.IO;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;

namespace RayWrapper.Objs;

// todo: see if its possible to add support for SixLabors.ImageSharp
// can probs make RayWrapper.ImageProcessing
public class ImageObj : GameObject
{
    public readonly string Path;
    
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

    public ImageObj(string imageFile, Vector2? pos = null) : this(Raylib.LoadImage(imageFile), pos ?? Vector2.Zero)
    {
        Path = imageFile;
    }

    public ImageObj(Image image, Vector2? pos = null)
    {
        Path = null;
        _image = image;
        texture = new TextureObj(image.Texture(), pos ?? Vector2.Zero);
        _textSize = RectWrapper.AssembleRectFromVec(Vector2.Zero, texture.Size);
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