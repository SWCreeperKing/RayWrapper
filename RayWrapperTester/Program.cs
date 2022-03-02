﻿using System;
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
using RayWrapperTester.Animations;
using static Raylib_CsLo.Color;
using static Raylib_CsLo.Raylib;
using static RayWrapper.GameBox;
using static RayWrapper.RectWrapper;

namespace RayWrapperTester
{
    class Program : GameLoop
    {
        private float _percent;
        private int _buttonInc;
        private bool saveTesting = false;
        private bool scheduleTesting = false;
        private int currGraf;
        private Rectangle _scissorArea;
        private Button _b;
        private TabView _tbv;
        private Label _l;
        private ListView _lv;
        private DropDown _dd;
        private Graph graf;

        private Func<float, Vector2>[] grafFunc =
        {
            dx => new Vector2(dx * 10, (float) Math.Sin(dx)),
            dx => new Vector2((float) (dx * Math.Cos(dx)), (float) (dx * Math.Sin(dx))),
            dx => new Vector2(dx, (float) Math.Log(dx)),
            dx => new Vector2(dx, (float) Math.Exp(dx)),
            dx => new Vector2(dx / 5f - 5, (float) Math.Tan(dx / 5f - 5)),
            dx => new Vector2(dx, (float) Math.Sin(Math.Pow(dx, 2)))
        };


        private string yes =
            "What the fuck did you just fucking say about me, you little bitch? I'll have you know I graduated top of my class in the Navy Seals, and I've been involved in numerous secret raids on Al-Quaeda, and I have over 300 confirmed kills. I am trained in gorilla warfare and I'm the top sniper in the entire US armed forces. You are nothing to me but just another target. I will wipe you the fuck out with precision the likes of which has never been seen before on this Earth, mark my fucking words. You think you can get away with saying that shit to me over the Internet? Think again, fucker. As we speak I am contacting my secret network of spies across the USA and your IP is being traced right now so you better prepare for the storm, maggot. The storm that wipes out the pathetic little thing you call your life. You're fucking dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that's just with my bare hands. Not only am I extensively trained in unarmed combat, but I have access to the entire arsenal of the United States Marine Corps and I will use it to its full extent to wipe your miserable ass off the face of the continent, you little shit. If only you could have known what unholy retribution your little \"clever\" comment was about to bring down upon you, maybe you would have held your fucking tongue. But you couldn't, you didn't, and now you're paying the price, you goddamn idiot. I will shit fury all over you and you will drown in it. You're fucking dead, kiddo.";

        public class Test
        {
            public int i = 6;
        }

        static void Main(string[] args) => new GameBox(new Program(), new Vector2(1280, 720), "Hallo World");

        public override void Init()
        {
            // FontManager.fontFilter = TextureFilter.TEXTURE_FILTER_POINT;
            // GameBox.targetTextureFilter = TextureFilter.TEXTURE_FILTER_POINT;
            FontManager.RegisterFont("cas", "CascadiaMono.ttf");
            FontManager.SetDefFont("cas");

            var screen = WindowSize;
            Vector2 pos = new(75, 80);

            if (scheduleTesting)
            {
                AddScheduler(new Scheduler(10,
                    () => Console.WriteLine($"Scheduler: time = {DateTime.Now:HH\\:mm\\:ss\\.fff}")));
            }

            if (saveTesting)
            {
                Logger.Log("save testing start");
                SaveTesting();
                Logger.Log("save testing over");
            }

            _b = new Button(AssembleRectFromVec(pos, new Vector2(200, 200)), "Just a Name")
            {
                Mode = new Actionable<Label.TextMode>(() => (Label.TextMode) (_buttonInc % 3)),
                isDisabled = new Actionable<bool>(false, () => _buttonInc > 10)
            };
            _b.Clicked += () => _buttonInc++;

            _l = new Label(AssembleRectFromVec(pos, new Vector2(200, 200)), "Look! I can move with the arrow keys!",
                Label.TextMode.WrapText);

            var arr = new List<string>
                { "1", "2", "22", "hi", "bye", "no", "u", "yeet", "8", "not 10", "double 1", "yes", "no" };
            _lv = new ListView(new Vector2(40, 100), 500, i => arr[i], () => arr.Count, 12);
            _lv.IndividualClick = i => Console.WriteLine($"{i}: {arr[i]}");

            _dd = new DropDown(pos, "option 1", "option duo", "option non", "option hi",
                "option option", "option setting", "option N");

            _tbv = new(Vector2.Zero, WindowSize.X);
            _tbv.AddTab("Button Test", _b,
                new EmptyRender(() =>
                    DrawText($"Hello, world! [i] is {_buttonInc}", 12, 60, 20, new Color(174, 177, 181, 255))));

            _tbv.AddTab("Label Test", _l);

            RectItem ri = new(new Vector2(400), new Vector2(75)) { slotDependent = false, id = "blue", color = BLUE };
            CircleItem ci = new(new Vector2(400, 475), new Vector2(75)) { color = RED, id = "red" };
            ImageObj io = new("Images/Untitled.png", new Vector2(400, 550));
            ImageObj io2 = new("Images/Untitled.png", new Vector2(1025, 300));
            io.SetSize(new Vector2(75));
            io2.SetSize(new Vector2(75));
            io2.ImageAlpha = 100;
            ImageItem ii = new(io) { slotDependent = false };
            Slot sl = new(new Vector2(800, 300), new Vector2(75)) { color = PURPLE };
            Slot sl1 = new(new Vector2(875, 300), new Vector2(75)) { color = BLUE, idRestriction = "blue" };
            Slot sl2 = new(new Vector2(950, 300), new Vector2(75)) { color = RED, idRestriction = "red" };
            ImageSlot imSl = new(io2) { color = YELLOW };
            ii.SetSlot(imSl);
            _tbv.AddTab("Slot Test", imSl, ri, ci, ii, sl, sl1, sl2);

            _tbv.AddTab("Mask Test", new EmptyRender(() => _scissorArea.MaskDraw(() =>
            {
                _scissorArea.Draw(new Color(255, 255, 255, 10));
                DrawText("Move the mouse around to reveal this text!", 190, 200, 20, LIGHTGRAY);
            })));

            _tbv.AddTab("Tooltip Test",
                new EmptyRender(() =>
                    AssembleRectFromVec(Vector2.Zero, WindowSize).DrawTooltip("Testing Tooltip")));

            Button listViewButton = new(new Rectangle(700, 100, 0, 0), "Clear", Label.TextMode.SizeToText);
            listViewButton.Clicked += () =>
            {
                arr.Clear();
                arr.Add("hi");
            };

            _tbv.AddTab("ListView Test", _lv, listViewButton);
            _tbv.AddTab("DropDown Test", _dd);
            _tbv.AddTab("Checkbox Test", new Checkbox(pos, "Square Check"),
                new Checkbox(pos + new Vector2(0, 50), "Circle") { isCircle = true });

            TreeView tv = new(new NodeChain(new Box(Vector2.One, () => "hi") { completed = true },
                new Box(new Vector2(1, 3), () => "hi2") { completed = true },
                new Ball(new Vector2(3, 1), () => "hi3") { completed = true },
                new Ball(new Vector2(3, 3), () => "hi4") { completed = true },
                new Ball(new Vector2(4, 8), new Vector2(2, 1), () => "yeet")));
            tv.axisOffset = new Vector2(5, 5);
            tv.mask = AssembleRectFromVec(new Vector2(0), screen).ExtendPos(new Vector2(0, -60));

            _tbv.AddTab("TreeView Test", tv);

            KeyButton kb = new(pos, KeyboardKey.KEY_C);
            kb.keyChange = key => Console.WriteLine($"New Key: {key}");

            ScrollView sv = new(new Rectangle(200, 200, 700, 500));
            Label l = new(Vector2.Zero, "Text1");
            Label l2 = new(new Vector2(500), "Text2");
            Label l3 = new(new Vector2(1000, 100), "Text3");
            sv.AddObj(l);
            sv.AddObj(l2);
            sv.AddObj(l3);
            _tbv.AddTab("ScrollView Test", sv);

            _tbv.AddTab("KeyButton Test", kb);

            var b = new Button(AssembleRectFromVec(pos, new Vector2()), "Test", Label.TextMode.SizeToText);
            var bb = new Button(AssembleRectFromVec(pos + new Vector2(0, 60), new Vector2()), "Test info",
                Label.TextMode.SizeToText);
            b.Clicked += () => new AlertBox("Testing", "Just testing alert boxes").Show();
            bb.Clicked += () => new AlertBox("Testing", "Just testing alert boxes", true).Show();

            _tbv.AddTab("AlertBox Test", b, bb);
            _tbv.AddTab("Input Test", new InputBox(pos));

            Button aniB = new(new Rectangle(20, 80, 0, 0), "Queue Animation", Label.TextMode.SizeToText);
            aniB.Clicked += () => Animator.AddToAnimationQueue(new TestAnimation1());

            Button aniBC = new(new Rectangle(20, 120, 0, 0), "Add Animation", Label.TextMode.SizeToText);
            aniBC.Clicked += () => Animator.AddAnimation(new Mover());

            Button aniBT = new(new Rectangle(20, 160, 0, 0), "Queue Trigger Animation", Label.TextMode.SizeToText);
            aniBT.Clicked += () => Animator.AddToAnimationQueue(new InteractionAnimation());

            _tbv.AddTab("Animation Test", aniB, aniBC, aniBT);
            var pb = new ProgressBar(100, 100, 400, 30, () => _percent);
            _tbv.AddTab("Progress/Slider Test", pb, new Slider(100, 300, 400, 30));

            var rt2d = LoadRenderTexture(250, 250);
            Button bRend = new(new Vector2(60, 90), "test");
            _tbv.AddTab("Render 2d test", new EmptyRender(() =>
            {
                RenderRenderTexture(rt2d, new Vector2(40, 80), () => bRend.Update(),
                    () =>
                    {
                        ClearBackground(RED);
                        bRend.Render();
                        tooltip.Add($"{mousePos}");
                    });
            }));

            graf = new(new Rectangle(50, 100, 1000, 400));
            _tbv.AddTab("Graphy", graf,
                new Text(new Actionable<string>(() => $"A: {graf.Amount()}"), new Vector2(120, 520)));
            graf.minConstraint = new Actionable<float>(() => currGraf == 4 ? -6 : float.MinValue);
            graf.maxConstraint = new Actionable<float>(() => currGraf == 4 ? 6 : float.MaxValue);
            graf.ExecuteFunction(grafFunc[0]);

            fpsPos = new Vector2(12, WindowSize.Y - 25);

            RegisterGameObj(_tbv);
        }

        public override void UpdateLoop()
        {
            _percent += .005f;
            _percent %= 1;

            var mouse = mousePos;
            _scissorArea = new Rectangle(mouse.X - 100, mouse.Y - 100, 200, 200);

            if (IsKeyDown(KeyboardKey.KEY_LEFT)) _l.Position += new Vector2(-3, 0);
            else if (IsKeyDown(KeyboardKey.KEY_RIGHT)) _l.Position += new Vector2(3, 0);
            if (IsKeyDown(KeyboardKey.KEY_UP)) _l.Position += new Vector2(0, -3);
            else if (IsKeyDown(KeyboardKey.KEY_DOWN)) _l.Position += new Vector2(0, 3);
            if (IsKeyPressed(KeyboardKey.KEY_SPACE)) _tbv.Closable = !_tbv.Closable;

            if (!IsKeyPressed(KeyboardKey.KEY_R)) return;
            currGraf++;
            currGraf %= grafFunc.Length;
            graf.ExecuteFunction(grafFunc[currGraf]);
        }

        public override void RenderLoop()
        {
        }

        public void SaveTesting()
        {
            ISave.Cypher = (s => string.Join("", s.Reverse()), s => string.Join("", s.Reverse()));
            InitSaveSystem("SW_CreeperKing", "SaveTesting");
            var t = new Test();
            // basic encryption
            RegisterSaveItem(t, "test item");
            t.i = 10;
            Logger.Log($"i = {t.i}"); // 10
            SaveItems();
            t.i = 2;
            Logger.Log($"i = {t.i}"); // 2
            LoadItems();
            Logger.Log($"i = {t.i}"); // 10
            t.Set(new Test());
            Logger.Log($"i = {t.i}"); // 6
            LoadItems();
            Logger.Log($"i = {t.i}"); // 10
            t = new Test();
            Logger.Log($"i = {t.i}"); // 6 
            LoadItems();
            Logger.Log($"i = {t.i}"); // 6
        }
    }
}