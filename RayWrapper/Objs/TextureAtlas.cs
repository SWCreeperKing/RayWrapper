using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Raylib_CsLo;
using ZimonIsHimUtils.ExtensionMethods;

namespace RayWrapper.Objs;

public class TextureAtlas
{
    public Image image;
    public Texture texture;
    public int pixelScale;
    public Dictionary<string, Rectangle> imageRegistry = new();

    public TextureAtlas(string imagePath, int pixelScale) : this(Raylib.LoadImage(imagePath), pixelScale)
    {
        if (!File.Exists(imagePath)) throw new FileNotFoundException($"[{imagePath}] is not a valid file path");
    }

    public TextureAtlas(Image image, int pixelScale)
    {
        this.image = image;
        this.pixelScale = pixelScale;
        texture = image.Texture();
    }

    public void Draw(string id, Rectangle destRect, Vector2? origin = null,
        float rotation = 0, Color? tint = null)
    {
        Raylib.DrawTexturePro(texture, imageRegistry[id], destRect, origin ?? Vector2.Zero, rotation,
            tint ?? Raylib.WHITE);
    }

    public void Draw(string id, Vector2 pos, Vector2? size = null, Vector2? origin = null,
        float rotation = 0, Color? tint = null)
    {
        var dest = RectWrapper.AssembleRectFromVec(pos, size ?? new Vector2(pixelScale));
        Raylib.DrawTexturePro(texture, imageRegistry[id], dest, origin ?? Vector2.Zero, rotation, tint ?? Raylib.WHITE);
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

    public TextureAtlas RegisterRow(int start, int row, params string[] ids)
    {
        for (var i = start; i < ids.Length; i++) Register(ids[i - start], i, row);
        return this;
    }

    public TextureAtlas RegisterCol(int start, int col, params string[] ids)
    {
        for (var i = start; i < ids.Length; i++) Register(ids[i - start], col, i);
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

public record AtlasTexture(TextureAtlas Atlas, string Id, Vector2? Origin = null,
    float Rotation = 0, Color? Tint = null)
{
    public void Draw(Vector2 pos, Vector2 size, float scale)
    {
        Atlas.Draw(Id, pos, size, (Origin ?? Vector2.Zero) * scale, Rotation, Tint ?? Raylib.WHITE);
    }
}

public record CompoundAtlasTexture(params AtlasTexture[] Textures)
{
    public void Draw(Vector2 pos, Vector2 size, float scale) => Textures.Each(t => t.Draw(pos, size, scale));

    public void Draw(Rectangle dest, float scale)
    {
        var pos = dest.Pos();
        var size = dest.Size();
        Textures.Each(t => t.Draw(pos, size, scale));
    }

    public static implicit operator CompoundAtlasTexture(AtlasTexture t) => new(t);
}