using System;
using System.Net.Mime;
using System.Numerics;
using Raylib_cs;
using RayWrapper;
using RayWrapper.Objs;
using RayWrapper.Objs.TreeViewShapes;
using RayWrapper.Vars;
using static Raylib_cs.Color;
using static Raylib_cs.Raylib;
using static RayWrapper.RectWrapper;

namespace RayWrapperTester
{
    class Program : GameLoop
    {
        public static GameBox gb;

        private int _buttonInc;
        private Rectangle _scissorArea;
        private Button _b;
        private TabView _tbv;
        private Label _l;

        // private Scrollbar _sb = new(new(400, 50, 20, 380));
        private ListView _lv;
        private DropDown _dd;

        private string yes =
            "What the fuck did you just fucking say about me, you little bitch? I'll have you know I graduated top of my class in the Navy Seals, and I've been involved in numerous secret raids on Al-Quaeda, and I have over 300 confirmed kills. I am trained in gorilla warfare and I'm the top sniper in the entire US armed forces. You are nothing to me but just another target. I will wipe you the fuck out with precision the likes of which has never been seen before on this Earth, mark my fucking words. You think you can get away with saying that shit to me over the Internet? Think again, fucker. As we speak I am contacting my secret network of spies across the USA and your IP is being traced right now so you better prepare for the storm, maggot. The storm that wipes out the pathetic little thing you call your life. You're fucking dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that's just with my bare hands. Not only am I extensively trained in unarmed combat, but I have access to the entire arsenal of the United States Marine Corps and I will use it to its full extent to wipe your miserable ass off the face of the continent, you little shit. If only you could have known what unholy retribution your little \"clever\" comment was about to bring down upon you, maybe you would have held your fucking tongue. But you couldn't, you didn't, and now you're paying the price, you goddamn idiot. I will shit fury all over you and you will drown in it. You're fucking dead, kiddo.";

        public class Test : Setable<Test>
        {
            public int i = 6;

            protected override Test GetThis() => this;
        }

        static void Main(string[] args)
        {
            gb = new GameBox(new Program(), new Vector2(800, 480), "Hallo World");
            gb.Start();
        }

        public override void Init()
        {
            var screen = GameBox.WindowSize;
            Vector2 pos = new(75, 80);

            gb.AddScheduler(new Scheduler(1000,
                () => Console.WriteLine($"Scheduler: time = {DateTime.Now:HH\\:mm\\:ss\\.ffff}")));

            // save testing 
            // gb.InitSaveSystem("SW_CreeperKing", "SaveTesting");
            // var t = new Test();
            // gb.RegisterSaveItem(t, "test item");
            // t.i = 10;
            // Console.WriteLine($"i = {t.i}");
            // gb.SaveItems();
            // t.i = 2;
            // Console.WriteLine($"i = {t.i}");
            // gb.LoadItems();
            // Console.WriteLine($"i = {t.i}");

            _b = new Button(AssembleRectFromVec(pos, new Vector2(200, 200)), "Just a Name");
            _b.Clicked += () =>
            {
                if (_buttonInc > 10) _b.isDisabled = true;
            };
            _b.Clicked += () => _buttonInc++;
            _b.Clicked += () => _b.buttonMode = (Button.ButtonMode) (_buttonInc % 3);

            _l = new Label(AssembleRectFromVec(pos, new Vector2(200, 200)), "Look! I can move with the arrow keys!",
                Label.TextMode.WrapText);

            // _sb.amount = 1000;
            // _sb.OnMoveEvent += v => Console.WriteLine(v);

            var arr = new[] {"1", "2", "22", "hi", "bye", "no", "u", "yeet", "8", "not 10", "double 1", "yes", "no"};
            _lv = new ListView(pos, 175, i =>
            {
                try
                {
                    return arr[i];
                }
                catch
                {
                    Console.WriteLine(i);
                }

                return "null";
            }, () => arr.Length, 12);
            _lv.IndividualClick = i => Console.WriteLine($"{i}: {arr[i]}");

            _dd = new DropDown(pos, "option 1", "option duo", "option non", "option hi",
                "option option", "option setting", "option N");

            _tbv = new(new Vector2(0), GameBox.WindowSize.X);
            _tbv.AddTab("Button Test", _b,
                new EmptyRender(() =>
                    DrawText($"Hello, world! [i] is {_buttonInc}", 12, 60, 20, new Color(174, 177, 181, 255))));

            _tbv.AddTab("Label Test", _l);

            _tbv.AddTab("Mask Test", new EmptyRender(() => _scissorArea.MaskDraw(() =>
            {
                _scissorArea.Draw(new(255, 255, 255, 10));
                DrawText("Move the mouse around to reveal this text!", 190, 200, 20, LIGHTGRAY);
            })));

            _tbv.AddTab("Tooltip Test",
                new EmptyRender(() =>
                    AssembleRectFromVec(GameBox.WindowSize / 2, Vector2.Zero).Grow(1000)
                        .DrawTooltip("Testing Tooltip")));

            _tbv.AddTab("ListView Test", _lv);
            _tbv.AddTab("DropDown Test", _dd);
            _tbv.AddTab("Checkbox Test", new Checkbox(pos, "Square Check"),
                new Checkbox(pos + new Vector2(0, 50), "Circle") {isCircle = true});

            TreeView tv = new();
            tv.mask = AssembleRectFromVec(new Vector2(0), screen).ExtendPos(new Vector2(0, -60));
            // lines
            tv.AddNode(new Line(new Vector2(1, 1), new Vector2(1, 3), () => false),
                new Line(new Vector2(3, 1), new Vector2(3, 3), () => true),
                new Line(new Vector2(1, 1), new Vector2(3, 3), () => true));
            // circles
            tv.AddNode(new Box(new Rectangle(1, 1, 1, 1), () => false, "hi", "yo"),
                new Box(new Rectangle(1, 3, 1, 1), () => true, "hi2", "yo2"));
            //boxes
            tv.AddNode(new Circle(new Rectangle(3, 1, 1, 1), () => false, "hi3", "yo3"),
                new Circle(new Rectangle(3, 3, 1, 1), () => true, "hi4", "yo4"));
            tv.OnClick += Console.WriteLine;

            _tbv.AddTab("TreeView Test", tv);

            KeyButton kb = new(pos, KeyboardKey.KEY_C);
            kb.keyChange = key => Console.WriteLine($"New Key: {key}");

            _tbv.AddTab("KeyButton Test", kb);

            var b = new Button(AssembleRectFromVec(pos, new Vector2()), "Test", Button.ButtonMode.SizeToText);
            b.Clicked += () => new AlertBox("Testing", "Just testing alert boxes").Show();

            _tbv.AddTab("AlertBox Test", b);
            _tbv.AddTab("DrawTextRecEx Test", new EmptyRender(() =>
                DrawTextRecEx(GameBox.font, yes, new Rectangle(100, 100, 600, 340), 24, 1.5f, 
                    true, SKYBLUE, 4, 8, RED, GOLD)));

            RegisterGameObj(true, _tbv);
        }

        public override void UpdateLoop()
        {
            var mouse = GetMousePosition();
            _scissorArea = new Rectangle(mouse.X - 100, mouse.Y - 100, 200, 200);

            if (IsKeyDown(KeyboardKey.KEY_LEFT)) _l.MoveBy(new Vector2(-3, 0));
            else if (IsKeyDown(KeyboardKey.KEY_RIGHT)) _l.MoveBy(new Vector2(3, 0));
            if (IsKeyDown(KeyboardKey.KEY_UP)) _l.MoveBy(new Vector2(0, -3));
            else if (IsKeyDown(KeyboardKey.KEY_DOWN)) _l.MoveBy(new Vector2(0, 3));
            if (IsKeyPressed(KeyboardKey.KEY_SPACE)) _tbv.Closable = !_tbv.Closable;
        }

        public override void RenderLoop()
        {
            var size = GameBox.WindowSize;
            DrawFPS(12, (int) (size.Y - 25));
        }
    }
}