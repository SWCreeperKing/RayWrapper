using System.Numerics;
using Raylib_CsLo;
using Texture2D = Raylib_CsLo.Texture;
using Img = Raylib_CsLo.Image;
using static Raylib_CsLo.Raylib;

namespace RayWrapper.Base;

public class Texture : IDisposable
{
    public int Width => _texture.width;
    public int Height => _texture.height;
    public int Mipmaps => _texture.mipmaps;
    public int Format => _texture.format;

    public PixelFormat Format_
    {
        get => _texture.format_;
        set => _texture.format_ = value;
    }
    
    public Texture2D Texture2D => _texture;

    public Color tint = WHITE;

    private Texture2D _texture;

    public Texture(string path) => _texture = LoadTexture(path);
    public Texture(Texture2D texture) => _texture = texture;
    public Texture(Img image) => _texture = LoadTextureFromImage(image);  

    public void Draw(Vector2 pos, Color? tint = null) => DrawTextureV(_texture, pos, tint ?? WHITE);
    public void Draw(int x, int y, Color? tint = null) => DrawTexture(_texture, x, y, tint ?? WHITE);

    public void Draw(Rectangle src, Vector3 pos, float w, float h, float length)
    {
        DrawCubeTextureRec(_texture, src, pos, w, h, length, tint);
    }

    public void Draw(NPatchInfo patch, Rectangle dest, Vector2 origin, float rot)
    {
        DrawTextureNPatch(_texture, patch, dest, origin, rot, tint);
    }

    /// <summary>https://www.raylib.com/examples/textures/loader.html?name=textures_polygon</summary>
    public unsafe void Draw(Vector2 center, Vector2[] points, Vector2[] texCoords)
    {
        var pointCount = Math.Min(points.Length, texCoords.Length);
        fixed (Vector2* pointsP = points, texCoordsP = texCoords)
        {
            DrawTexturePoly(_texture, center, pointsP, texCoordsP, pointCount, tint);
        }
    }

    public void Draw(Vector2 center, Span<Vector2> points, Span<Vector2> texCoords)
    {
        var pointCount = Math.Min(points.Length, texCoords.Length);
        DrawTexturePoly(_texture, center, points, texCoords, pointCount, tint);
    }

    public void Draw(Rectangle src, Rectangle dest, Vector2 origin, float rot)
    {
        DrawTexturePro(_texture, src, dest, origin, rot, tint);
    }

    public void Draw(Vector2 tiling, Vector2 offset, Rectangle quad)
    {
        DrawTextureQuad(_texture, tiling, offset, quad, tint);
    }

    public void Draw(Rectangle src, Vector2 pos)
    {
        DrawTextureRec(_texture, src, pos, tint);
    }

    public void Draw(Rectangle src, Rectangle dest, Vector2 origin, float rot, float scale)
    {
        DrawTextureTiled(_texture, src, dest, origin, rot, scale, tint);
    }

    public void Draw(Vector2 pos, float rot, float scale)
    {
        DrawTextureEx(_texture, pos, rot, scale, tint);
    }

    public static implicit operator Texture(string path) => new(path);
    public static implicit operator Texture(Texture2D texture) => new(texture);
    public static implicit operator Texture(Img image) => new(image);
    public static implicit operator Texture2D(Texture texture) => texture.Texture2D; 
    public void Dispose() => UnloadTexture(_texture);
}