using System.Numerics;
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
}