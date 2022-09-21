using System;
using System.Numerics;
using Raylib_CsLo;
using Texture2D = Raylib_CsLo.Texture;
using static Raylib_CsLo.Raylib;

namespace RayWrapper.Vars;

public class Texture : IDisposable
{
    // KEEP NAMED
    // TO FIT WITH BASE TEXTURE NAMES!!!
    public int width => _texture.width;
    public int height => _texture.height;
    public int mipmaps => _texture.mipmaps;
    public int format => _texture.format;

    public PixelFormat format_
    {
        get => _texture.format_;
        set => _texture.format_ = value;
    }

    private Texture2D _texture;

    public Texture(string path) => _texture = LoadTexture(path);
    public Texture(Texture2D texture) => _texture = texture;

    public void Draw(Vector2 pos, Color? tint = null) => DrawTextureV(_texture, pos, tint ?? WHITE);
    public void Draw(int x, int y, Color? tint = null) => DrawTexture(_texture, x, y, tint ?? WHITE);

    public void Draw(Rectangle src, Vector3 pos, float w, float h, float length, Color? tint = null)
    {
        DrawCubeTextureRec(_texture, src, pos, w, h, length, tint ?? WHITE);
    }

    public void Draw(NPatchInfo patch, Rectangle dest, Vector2 origin, float rot, Color? tint = null)
    {
        DrawTextureNPatch(_texture, patch, dest, origin, rot, tint ?? WHITE);
    }

    /// <summary>https://www.raylib.com/examples/textures/loader.html?name=textures_polygon</summary>
    public unsafe void Draw(Vector2 center, Vector2[] points, Vector2[] texCoords, Color? tint = null)
    {
        var pointCount = Math.Min(points.Length, texCoords.Length);
        fixed (Vector2* pointsP = points, texCoordsP = texCoords)
        {
            DrawTexturePoly(_texture, center, pointsP, texCoordsP, pointCount, tint ?? WHITE);
        }
    }

    public void Draw(Vector2 center, Span<Vector2> points, Span<Vector2> texCoords, Color? tint = null)
    {
        var pointCount = Math.Min(points.Length, texCoords.Length);
        DrawTexturePoly(_texture, center, points, texCoords, pointCount, tint ?? WHITE);
    }

    public void Draw(Rectangle src, Rectangle dest, Vector2 origin, float rot, Color? tint = null)
    {
        DrawTexturePro(_texture, src, dest, origin, rot, tint ?? WHITE);
    }

    public void Draw(Vector2 tiling, Vector2 offset, Rectangle quad, Color? tint = null)
    {
        DrawTextureQuad(_texture, tiling, offset, quad, tint ?? WHITE);
    }

    public void Draw(Rectangle src, Vector2 pos, Color? tint = null)
    {
        DrawTextureRec(_texture, src, pos, tint ?? WHITE);
    }

    public void Draw(Rectangle src, Rectangle dest, Vector2 origin, float rot, float scale, Color? tint = null)
    {
        DrawTextureTiled(_texture, src, dest, origin, rot, scale, tint ?? WHITE);
    }

    public void Draw(Vector2 pos, float rot, float scale, Color? tint = null)
    {
        DrawTextureEx(_texture, pos, rot, scale, tint ?? WHITE);
    }

    public static implicit operator Texture(string path) => new(path);
    public static implicit operator Texture(Texture2D texture) => new(texture);
    public void Dispose() => UnloadTexture(_texture);
}