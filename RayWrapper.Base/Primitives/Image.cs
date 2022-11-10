using System.Numerics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using Img = Raylib_CsLo.Image;
using Texture2D = Raylib_CsLo.Texture;

namespace RayWrapper.Base.Primitives;

public class Image : IDisposable
{
    public Color Tint
    {
        get => _texture.tint;
        set => _texture.tint = value;
    }
    
    public Texture Texture => _texture;
    public int Width => _texture.Width;
    public int Height => _texture.Height;
    public int Mipmaps => _texture.Mipmaps;
    public int Format => _texture.Format;
    public Img Img => _image;

    private Texture _texture;
    private Img _image;

    public Image(string path) => _texture = _image = LoadImage(path);
    public Image(Img image) => _texture = _image = image;
    public Image(Texture2D texture) => _texture = _image = LoadImageFromTexture(texture);

    public void Draw(Vector2 pos, Color? tint = null) => DrawTextureV(_texture, pos, tint ?? WHITE);
    public void Draw(int x, int y, Color? tint = null) => DrawTexture(_texture, x, y, tint ?? WHITE);

    public void Draw(Rectangle src, Vector3 pos, float w, float h, float length)
    {
        DrawCubeTextureRec(_texture, src, pos, w, h, length, Tint);
    }

    public void Draw(NPatchInfo patch, Rectangle dest, Vector2 origin, float rot)
    {
        DrawTextureNPatch(_texture, patch, dest, origin, rot, Tint);
    }

    /// <summary>https://www.raylib.com/examples/textures/loader.html?name=textures_polygon</summary>
    public unsafe void Draw(Vector2 center, Vector2[] points, Vector2[] texCoords)
    {
        var pointCount = Math.Min(points.Length, texCoords.Length);
        fixed (Vector2* pointsP = points, texCoordsP = texCoords)
        {
            DrawTexturePoly(_texture, center, pointsP, texCoordsP, pointCount, Tint);
        }
    }

    public void Draw(Vector2 center, Span<Vector2> points, Span<Vector2> texCoords)
    {
        var pointCount = Math.Min(points.Length, texCoords.Length);
        DrawTexturePoly(_texture, center, points, texCoords, pointCount, Tint);
    }

    public void Draw(Rectangle src, Rectangle dest, Vector2 origin, float rot)
    {
        DrawTexturePro(_texture, src, dest, origin, rot, Tint);
    }

    public void Draw(Vector2 tiling, Vector2 offset, Rectangle quad)
    {
        DrawTextureQuad(_texture, tiling, offset, quad, Tint);
    }

    public void Draw(Rectangle src, Vector2 pos)
    {
        DrawTextureRec(_texture, src, pos, Tint);
    }

    public void Draw(Rectangle src, Rectangle dest, Vector2 origin, float rot, float scale)
    {
        DrawTextureTiled(_texture, src, dest, origin, rot, scale, Tint);
    }

    public void Draw(Vector2 pos, float rot, float scale)
    {
        DrawTextureEx(_texture, pos, rot, scale, Tint);
    }

    public void Dispose()
    {
        UnloadImage(Img);
        _texture.Dispose();
    }
}