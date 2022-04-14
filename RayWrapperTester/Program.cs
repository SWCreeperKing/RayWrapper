using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper;
using RayWrapper.Animation;
using RayWrapper.Objs;
using RayWrapper.Objs.Slot;
using RayWrapper.Objs.TreeView;
using RayWrapper.Objs.TreeView.TreeNodeChain;
using RayWrapper.Objs.TreeView.TreeNodeChain.NodeShapes;
using RayWrapper.Vars;
using RayWrapperTester;
using RayWrapperTester.Animations;
using RayWrapperTester.Example_Setup;
using static Raylib_CsLo.Raylib;
using static RayWrapper.GameBox;
using static RayWrapper.RectWrapper;

new GameBox(new Program(), new Vector2(1280, 720), "Hallo World");

public partial class Program : GameLoop
{
    // private float _percent;
    // private bool saveTesting = false;
    // private bool scheduleTesting = false;
    // private int currGraf;
    // private Rectangle _scissorArea;
    // private Button _b;
    // private TabView _tbv;
    // private Label _l;
    // private ListView _lv;
    // private DropDown _dd;
    // private Graph graf;
    //
    //
    // private string yes =
    //     "What the fuck did you just fucking say about me, you little bitch? I'll have you know I graduated top of my class in the Navy Seals, and I've been involved in numerous secret raids on Al-Quaeda, and I have over 300 confirmed kills. I am trained in gorilla warfare and I'm the top sniper in the entire US armed forces. You are nothing to me but just another target. I will wipe you the fuck out with precision the likes of which has never been seen before on this Earth, mark my fucking words. You think you can get away with saying that shit to me over the Internet? Think again, fucker. As we speak I am contacting my secret network of spies across the USA and your IP is being traced right now so you better prepare for the storm, maggot. The storm that wipes out the pathetic little thing you call your life. You're fucking dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that's just with my bare hands. Not only am I extensively trained in unarmed combat, but I have access to the entire arsenal of the United States Marine Corps and I will use it to its full extent to wipe your miserable ass off the face of the continent, you little shit. If only you could have known what unholy retribution your little \"clever\" comment was about to bring down upon you, maybe you would have held your fucking tongue. But you couldn't, you didn't, and now you're paying the price, you goddamn idiot. I will shit fury all over you and you will drown in it. You're fucking dead, kiddo.";
    //
    // public class Test
    // {
    //     public int i = 6;
    // }

    public static TabView exampleView;
    public static List<string> localExamples;

    public override void Init()
    {
        ExampleRegister.Init();
        localExamples = ExampleRegister.Examples.Keys.ToList();

        exampleView = new TabView(Vector2.Zero, WindowSize.X);
        exampleView.AddTab("Home", new ExamplesHome());

        RegisterGameObj(exampleView);

        // Logger.Log(LoadJsonFromWeb("https://api.dictionaryapi.dev/api/v2/entries/en/hello").Result);
        //
        // var screen = WindowSize;
        // Vector2 pos = new(75, 80);
        //
        // if (saveTesting)
        // {
        //     Logger.Log("save testing start");
        //     SaveTesting();
        //     Logger.Log("save testing over");
        // 
        // fpsPos = new Vector2(12, WindowSize.Y - 25);
        //
        // RegisterGameObj(_tbv);
    }

    public override void UpdateLoop()
    {
    }

    public override void RenderLoop()
    {
    }
    
    // public void SaveTesting()
    // {
    //     ISave.Cypher = (s => string.Join("", s.Reverse()), s => string.Join("", s.Reverse()));
    //     InitSaveSystem("SW_CreeperKing", "SaveTesting");
    //     var t = new Test();
    //     // basic encryption
    //     RegisterSaveItem(t, "test item");
    //     t.i = 10;
    //     Logger.Log($"i = {t.i}"); // 10
    //     SaveItems();
    //     t.i = 2;
    //     Logger.Log($"i = {t.i}"); // 2
    //     LoadItems();
    //     Logger.Log($"i = {t.i}"); // 10
    //     t.Set(new Test());
    //     Logger.Log($"i = {t.i}"); // 6
    //     LoadItems();
    //     Logger.Log($"i = {t.i}"); // 10
    //     t = new Test();
    //     Logger.Log($"i = {t.i}"); // 6 
    //     LoadItems();
    //     Logger.Log($"i = {t.i}"); // 6
    // }
}