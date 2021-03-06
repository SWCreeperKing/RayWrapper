using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper;
using RayWrapper.Objs;
using RayWrapper.Vars;
using RayWrapperTester;
using RayWrapperTester.Example_Setup;
using static RayWrapper.GameBox;

new GameBox(new Program(), new Vector2(1280, 720), "Hallo World");

public partial class Program : GameLoop
{
    public static TabView exampleView;
    public static List<string> localExamples;

    public override void Init()
    {
        InitSaveSystem("SW_CreeperKing", "SaveTesting");

        // basic encryption
        ISave.Cypher = (s => string.Join("", s.Reverse()), s => string.Join("", s.Reverse()));

        ExampleRegister.Init();
        localExamples = ExampleRegister.Examples.Keys.ToList();

        exampleView = new TabView(Vector2.Zero, WindowSize.X);
        exampleView.AddTab("Home", new ExamplesHome());

        RegisterGameObj(exampleView);
    }

    public override void UpdateLoop()
    {
    }

    public override void RenderLoop()
    {
    }
}