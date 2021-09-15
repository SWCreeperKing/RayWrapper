using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using RayWrapper;
using RayWrapper.Animation;
using RayWrapper.Animation.AnimationShapes;
using RayWrapper.Objs;
using RayWrapper.Objs.TreeView;
using RayWrapper.Objs.TreeView.Shapes;
using RayWrapper.Vars;
using static Raylib_cs.Color;
using static Raylib_cs.Raylib;
using static RayWrapper.RectWrapper;

namespace RayWrapperTester
{
    class Program : GameLoop
    {
        public static GameBox gb;

        private float percent;
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

        public class Test
        {
            public int i = 6;
        }

        static void Main(string[] args)
        {
            gb = new GameBox(new Program(), new Vector2(1280, 720), "Hallo World");
            gb.Start(false);
        }

        public override void Init()
        {
            GameBox.Font = LoadFont("CascadiaMono.ttf");

            var screen = GameBox.WindowSize;
            Vector2 pos = new(75, 80);

            // gb.AddScheduler(new Scheduler(100,
            //     () => Console.WriteLine($"Scheduler: time = {DateTime.Now:HH\\:mm\\:ss\\.ffff}")));

            // save testing
            // Console.WriteLine("save testing start");
            // SaveTesting();
            // Console.WriteLine("save testing over");

            _b = new Button(AssembleRectFromVec(pos, new Vector2(200, 200)), "Just a Name");
            _b.isDisabled = new(false, () => _buttonInc > 10);
            _b.Clicked += () => _buttonInc++;
            _b.Clicked += () => _b.buttonMode = (Button.ButtonMode)(_buttonInc % 3);

            _l = new Label(AssembleRectFromVec(pos, new Vector2(200, 200)), "Look! I can move with the arrow keys!",
                Label.TextMode.WrapText);

            // _sb.amount = 1000;
            // _sb.OnMoveEvent += v => Console.WriteLine(v);

            var arr = new List<string>
                { "1", "2", "22", "hi", "bye", "no", "u", "yeet", "8", "not 10", "double 1", "yes", "no" };
            _lv = new ListView(new Vector2(40, 100), 500, i => arr[i], () => arr.Count, 12);
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

            Button listViewButton = new(new Rectangle(700, 100, 0, 0), "Clear", Button.ButtonMode.SizeToText);
            listViewButton.Clicked += () =>
            {
                arr.Clear();
                arr.Add("hi");
            };

            _tbv.AddTab("ListView Test", _lv, listViewButton);
            _tbv.AddTab("DropDown Test", _dd);
            _tbv.AddTab("Checkbox Test", new Checkbox(pos, "Square Check"),
                new Checkbox(pos + new Vector2(0, 50), "Circle") { isCircle = true });

            TreeView tv = new(new Box("hi", false, tooltip: () => "hi"),
                new Box("hi2", new Vector2(1, 3), true, tooltip: () => "hi2"),
                new Ball("hi3", new Vector2(3, 1), false, tooltip: () => "hi3"),
                new Ball("hi4", new Vector2(3, 3), true, tooltip: () => "hi4"),
                new Ball("hi5", new Vector2(4, 8), new Vector2(2, 1), false, tooltip: () => "yeet"),
                new Line("hi", "hi2", false), new Line("hi3", "hi4", false),
                new Line("hi", new Vector2(4.5f, 8), true) { prog = false });
            tv.axisOffset = new Vector2(5, 5);
            tv.mask = AssembleRectFromVec(new Vector2(0), screen).ExtendPos(new Vector2(0, -60));

            _tbv.AddTab("TreeView Test", tv);

            KeyButton kb = new(pos, KeyboardKey.KEY_C);
            kb.keyChange = key => Console.WriteLine($"New Key: {key}");

            _tbv.AddTab("KeyButton Test", kb);

            var b = new Button(AssembleRectFromVec(pos, new Vector2()), "Test", Button.ButtonMode.SizeToText);
            var bb = new Button(AssembleRectFromVec(pos + new Vector2(0, 60), new Vector2()), "Test info",
                Button.ButtonMode.SizeToText);
            b.Clicked += () => new AlertBox("Testing", "Just testing alert boxes").Show();
            bb.Clicked += () => new AlertBox("Testing", "Just testing alert boxes", true).Show();

            _tbv.AddTab("AlertBox Test", b, bb);
            // _tbv.AddTab("DrawTextRecEx Test", new EmptyRender(() =>
            //     DrawTextRecEx(GameBox.font, yes, new Rectangle(100, 100, 600, 340), 24, 1.5f,
            //         true, SKYBLUE, 4, 8, RED, GOLD)));

            _tbv.AddTab("Input Test", new InputBox(pos));

            Animation ani = new AnimationBuilder()
                .AddShape(new Square("test") { pos = screen / 2 - new Vector2(50, 50), size = new Vector2(100, 100) })
                .AddStep(.25f)
                .Slide("test", new Vector2(-50, -50))
                .AddStep(.25f)
                .Shrink("test", new Vector2(60, 60))
                .AddStep(1)
                .Slide("test", new Vector2(-100, 20))
                .AddStep(.75f)
                .Move("test", new Vector2(-200, -20))
                .Grow("test", new Vector2(200, 20))
                .AddStep(1)
                .SetVisible("test", false);

            Animation continueAni = new AnimationBuilder()
                .AddShape(new Square("mover", RED)
                    { pos = new Vector2(0, screen.Y / 2 - 25), size = new Vector2(50, 50) })
                .Move("mover", new Vector2(screen.X, 0));

            Animation triggerAni = new AnimationBuilder()
                .AddShape(new Square("block") { pos = new Vector2(25, screen.Y / 2 - 40), size = new Vector2(20, 20) })
                .WaitForTrigger(ab => ab.GetRectOfId("block").IsMouseIn())
                .AddStep(.5f)
                .Move("block", new Vector2(screen.X - 55, 0))
                .AddStep(-1)
                .WaitForTrigger(ab => ab.GetRectOfId("block").IsMouseIn())
                .AddStep(.5f)
                .Move("block", new Vector2(-screen.X / 2 - 10, 0))
                .AddStep(-1)
                .WaitForTrigger(ab =>
                    ab.GetRectOfId("block").IsMouseIn() && IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                .AddStep(.5f)
                .Move("block", new Vector2(0, screen.Y / 2 + 50));

            Button aniB = new(new Rectangle(20, 80, 0, 0), "Queue Animation", Button.ButtonMode.SizeToText);
            aniB.Clicked += () => { GameBox.animator.CopyQueueAnimation(ani); };

            Button aniBC = new(new Rectangle(20, 120, 0, 0), "Add Animation", Button.ButtonMode.SizeToText);
            aniBC.Clicked += () => { GameBox.animator.CopyAddAnimation(continueAni); };

            Button aniBT = new(new Rectangle(20, 160, 0, 0), "Queue Trigger Animation", Button.ButtonMode.SizeToText);
            aniBT.Clicked += () => { GameBox.animator.CopyQueueAnimation(triggerAni); };

            _tbv.AddTab("Animation Test", aniB, aniBC, aniBT);
            _tbv.AddTab("Progress/Slider Test", new ProgressBar(100, 100, 400, 30, () => percent),
                new Slider(100, 300, 400, 30));

            var rt2d = LoadRenderTexture(250, 250);
            Button bRend = new(new Vector2(60, 90), "test");
            _tbv.AddTab("Render 2d test", new EmptyRender(() =>
            {
                gb.RenderRenderTexture(rt2d, new Vector2(40, 80), () => bRend.Update(),
                    () =>
                    {
                        ClearBackground(RED);
                        bRend.Render();
                        GameBox.MousePos.DrawToolTipAtPoint($"{GameBox.MousePos}", BLUE);
                    });
            }));

            RegisterGameObj(true, _tbv);

            // W: [(%, 16)] H: [(!, 24)]
            // (char c, int i) w = (' ', 0);
            // (char c, int i) h = (' ', 0);
            // for (var i = 32; i < 127; i++)
            // {
            //     var c = (char) i;
            //     var wh = GameBox.font.MeasureText($"{c}");
            //     if ((int) wh.X > w.i) w = (c, (int) wh.X);
            //     else if ((int) wh.Y > h.i) h = (c, (int) wh.Y);
            // }
            //
            // Console.WriteLine($"W: [{w}] H: [{h}]");
        }

        public override void UpdateLoop()
        {
            percent += .005f;
            percent %= 1;

            var mouse = GameBox.MousePos;
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
            DrawFPS(12, (int)(size.Y - 25));
        }

        public void SaveTesting()
        {
            gb.InitSaveSystem("SW_CreeperKing", "SaveTesting");
            var t = new Test();
            gb.RegisterSaveItem(t, "test item");
            t.i = 10;
            Console.WriteLine($"i = {t.i}"); // 10
            gb.SaveItems();
            t.i = 2;
            Console.WriteLine($"i = {t.i}"); // 2
            gb.LoadItems();
            Console.WriteLine($"i = {t.i}"); // 10
            t.Set(new Test());
            Console.WriteLine($"i = {t.i}"); // 6
            gb.LoadItems();
            Console.WriteLine($"i = {t.i}"); // 10
            t = new Test();
            Console.WriteLine($"i = {t.i}"); // 6 
            gb.LoadItems();
            Console.WriteLine($"i = {t.i}"); // 6
        }
    }
}