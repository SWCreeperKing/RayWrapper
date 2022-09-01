using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_CsLo;

namespace RayWrapper.Objs;

public class TextureAtlas
{
    public Image image;
    public Texture texture;
    public int pixelScale;
    public Dictionary<string, Rectangle> imageRegistry = new();

    public TextureAtlas(string imagePath, int pixelScale) : this(Raylib.LoadImage(imagePath), pixelScale)
    {
    }

    public TextureAtlas(Image image, int pixelScale)
    {
        this.image = image;
        this.pixelScale = pixelScale;
        texture = image.Texture();
    }

    public void Draw(string id, float x, float y, float? w = null, float? h = null, Vector2? origin = null,
        float rotation = 0, Color? tint = null)
    {
        var dest = new Rectangle(x, y, w ?? pixelScale, h ?? pixelScale);
        Raylib.DrawTexturePro(texture, imageRegistry[id], dest, origin ?? Vector2.Zero, rotation, tint ?? Raylib.WHITE);
    }

    public void Draw(int srcX, int srcY, float x, float y, int? srcW = null, int? srcH = null, float? w = null,
        float? h = null, Vector2? origin = null, float rotation = 0, Color? tint = null)
    {
        var src = new Rectangle(srcX * pixelScale, srcY * pixelScale, srcW ?? pixelScale, srcH ?? pixelScale);
        var dest = new Rectangle(x, y, w ?? pixelScale, h ?? pixelScale);
        Raylib.DrawTexturePro(texture, src, dest, origin ?? Vector2.Zero, rotation, tint ?? Raylib.WHITE);
    }

    public TextureAtlas Register(string id, int x, int y, int w = 1, int h = 1)
    {
        var src = new Rectangle(x * pixelScale, y * pixelScale, w * pixelScale, h * pixelScale);
        imageRegistry.Add(id, src);
        return this;
    }

    public TextureAtlas RegisterAll(params (string id, int x, int y)[] images)
    {
        if (!images.Any()) return this;
        foreach (var (id, x, y) in images)
        {
            var src = new Rectangle(x * pixelScale, y * pixelScale, pixelScale, pixelScale);
            imageRegistry.Add(id, src);
        }

        return this;
    }

    ~TextureAtlas()
    {
        Raylib.UnloadImage(image);
        Raylib.UnloadTexture(texture);
    }
}