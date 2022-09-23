using System.Numerics;
using static System.Numerics.Vector2;
using static Raylib_CsLo.Raylib;

namespace RayWrapper.Base;

public static class VectorUtil
{
    public static int maskingLayer;

    /// <summary>
    /// mask a draw action within the bounds of 2 <see cref="Vector2"/>s
    /// </summary>
    /// <param name="pos">top left of mask</param>
    /// <param name="size">size of mask</param>
    /// <param name="draw">draw action to mask</param>
    public static void MaskDraw(this Vector2 pos, Vector2 size, Action draw)
    {
        maskingLayer++;
        BeginScissorMode((int) pos.X, (int) pos.Y, (int) size.X, (int) size.Y);
        draw.Invoke();
        if (maskingLayer == 1) EndScissorMode();
        maskingLayer--;
    }
    
    public static Vector2 Center(this Vector2 p1, Vector2 p2) => (p1 + p2) / 2;
    public static Vector2 Center(this Vector2[] poses) => poses.Aggregate((v1, v2) => v1 + v2) / poses.Length;
    public static float DistanceTo(this Vector2 p1, Vector2 p2) => Distance(p1, p2);
    public static float SquaredDistanceTo(this Vector2 p1, Vector2 p2) => DistanceSquared(p1, p2);
}