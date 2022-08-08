using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Objs;
using RayWrapper.Vars;
using static RayWrapper.VectorFont.LineUtil;
using static RayWrapper.VectorFont.VectorFontRegister;

namespace RayWrapper.VectorFont;

public class VectorFont : GameObject
{
    public float scale = 200;
    public int segments = 15;
    public Vector2 center;

    public Drawable[] b =
    {
        // "line": [0, 0, 0, 1, 0.1]
        new Line(new Vector2(0, 0), new Vector2(0, 1), .1f),
        // "line": [0, 0.5, 0, 0.92, 0.5, 0.5, 0.5, 1, 0.1]
        new CubeBezierLine(new Vector2(0, .5f), new Vector2(0, .92f), new Vector2(.5f, .5f), new Vector2(.5f, 1), .1f)
    };

    public VectorFont()
    {
        center = GameBox.WindowSize / 2;
        // RegisterGameObj(new Text("b", center));
    }

    protected override void UpdateCall()
    {
        // Console.WriteLine($"{scale} | {segments}");
        var mouseWheel = Raylib.GetMouseWheelMove();
        if (mouseWheel == 0) return;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT))
        {
            segments += (int) mouseWheel;
            segments = Math.Max(segments, 1);
        }
        else
        {
            scale += mouseWheel;
            scale = Math.Max(scale, 1);
        }
    }

    protected override void RenderCall()
    {
        for (var i = 0; i < b.Length; i++)
        {
            b[i].Draw(center, scale, Raylib.BLUE);
        }
        // var pos1 = new Vector2(50, 200);
        // var pos2 = new Vector2(200);
        //
        // // inner and outter -+ .05
        // Raylib.DrawRing(center, .2f * scale, .3f * scale, 0, 145, segments, Raylib.BLUE);
        // DrawBezQuadLineTriangle(pos1, pos2, GameBox.mousePos, 30, Raylib.GOLD);
        // // Raylib.DrawLineBezierQuad(pos1, pos2, GameBox.mousePos, 30, Raylib.BLUE);
    }

    public override Vector2 Position { get; set; }
    public override Vector2 Size { get; }
}