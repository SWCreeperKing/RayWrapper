using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper;
using RayWrapper.Objs;
using RayWrapper.Vars;
using static Raylib_cs.Raylib;

namespace RayWrapperTester
{
    class Program : GameLoop
    {
        public static GameBox gb;
        
        private int _buttonInc;
        private Rectangle _rect = new(100, 100, 200, 100);
        private Rectangle _scissorArea;
        private Button _b;
        private Label _l;
        // private Scrollbar _sb = new(new(400, 50, 20, 380));
        private Listview _lv;
        private DropDown _dd;

        public class test : ISetable
        {
            public int i = 6;
            
            public void Set(ISetable set)
            {
                if (set is not test ti) return;
                i = ti.i;
            }
        }

        static void Main(string[] args)
        {
            gb = new(new Program(), new(800, 480), "Hallo World");
            gb.Start();
        }

        public override void Init()
        {
            // save testing 
            // gb.InitSaveSystem("SW_CreeperKing", "SaveTesting");
            // var t = new test();
            // gb.RegisterSaveItem(t, "test item");
            // t.i = 10;
            // Console.WriteLine($"i = {t.i}");
            // gb.SaveItems();
            // t.i = 2;
            // Console.WriteLine($"i = {t.i}");
            // gb.LoadItems();
            // Console.WriteLine($"i = {t.i}");
            
            _b = new(_rect, "Just a Name");
            _b.Clicked += () =>
            {
                if (_buttonInc > 10) _b.isDisabled = true;
            };
            _b.Clicked += () => _buttonInc++;
            _b.Clicked += () => _b.buttonMode = (Button.ButtonMode) (_buttonInc % 3);

            _l = new(new(250, 250, 150, 150), "Label Test");

            // _sb.amount = 1000;
            // _sb.OnMoveEvent += v => Console.WriteLine(v);

            var arr = new[] {"1", "2", "22", "hi", "bye", "no", "u", "yeet", "8", "not 10", "double 1", "yes"};
            _lv = new Listview(new(450, 50), 175, i => arr[i], () => arr.Length, 7);
            _lv.IndividualClick = i => Console.WriteLine($"{i}: {arr[i]}");

            // _dd = new DropDown(new (10, 210), "option 1", "option duo", "option non", "option hi", "option option", "option setting", "option N");
            
            // RegisterGameObj(false, _b, _l, _lv, _dd);
        }

        public override void UpdateLoop()
        {
            var mouse = GetMousePosition();
            _scissorArea = new(mouse.X - 100, mouse.Y - 100, 200, 200);
        }

        public override void RenderLoop()
        {
            DrawText($"Hello, world! [i] is {_buttonInc}", 12, 12, 20, new(174, 177, 181, 255));
            DrawFPS(12, 34);

            RayWrapper.RayWrapper.AssembleRectFromVec(GameBox.WindowSize / 2, Vector2.Zero).Grow(100).DrawTooltip("Testing Tooltip");
            // _scissorArea.MaskDraw(() =>
            // {
            //     _scissorArea.Draw(new(255, 255, 255, 10));
            //     DrawText("Move the mouse around to reveal this text!", 190, 200, 20, LIGHTGRAY);
            // });
        }
    }
}