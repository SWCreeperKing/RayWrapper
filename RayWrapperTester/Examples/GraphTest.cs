using System;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base;
using RayWrapper.Objs;
using RayWrapperTester.Example_Setup;
using static Raylib_CsLo.Raylib;
using Rectangle = Raylib_CsLo.Rectangle;

namespace RayWrapperTester.Examples;

[Example("Graph Test")]
public class GraphTest : Example
{
    private Func<float, Vector2>[] grafFunc =
    {
        dx => new Vector2(dx * 10, (float) Math.Sin(dx)),
        dx => new Vector2((float) (dx * Math.Cos(dx)), (float) (dx * Math.Sin(dx))),
        dx => new Vector2(dx, (float) Math.Log(dx)),
        dx => new Vector2(dx, (float) Math.Exp(dx)),
        dx => new Vector2(dx / 5f - 5, (float) Math.Tan(dx / 5f - 5)),
        dx => new Vector2(dx, (float) Math.Sin(Math.Pow(dx, 2)))
    };
    
    private int _currGraf;
    private readonly Graph _graf;

    public GraphTest(string tabName) : base(tabName)
    {
        _graf = new Graph(new Rectangle(50, 100, 1000, 400))
        {
            minConstraint = new Actionable<float>(() => _currGraf == 4 ? -6 : float.MinValue),
            maxConstraint = new Actionable<float>(() => _currGraf == 4 ? 6 : float.MaxValue)
        };
        _graf.ExecuteFunction(grafFunc[0]);

        var grafText = new Text(new Actionable<string>(() => $"A: {_graf.Amount()}"), new Vector2(120, 520));
        
        RegisterGameObj(grafText, _graf);
    }

    protected override void UpdateCall()
    {
        if (!IsKeyPressed(KeyboardKey.KEY_R)) return;
        _currGraf++;
        _currGraf %= grafFunc.Length;
        _graf.ExecuteFunction(grafFunc[_currGraf]);
    }
}