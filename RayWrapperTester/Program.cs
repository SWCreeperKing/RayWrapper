using System;
using System.Numerics;
using System.Threading.Tasks;
using Raylib_cs;
using RayWrapper;
using RayWrapper.Objs;
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
        private Listview _lv;
        private DropDown _dd;

        public class Test : ISetable
        {
            public int i = 6;

            public void Set(ISetable set)
            {
                if (set is not Test ti) return;
                i = ti.i;
            }
        }

        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                await Task.Delay( /*time in ms or TimeSpan*/ 5000);
            }); // says hi there after 5sec without stopping everything

            gb = new GameBox(new Program(), new Vector2(800, 480), "Hallo World");
            gb.Start();
        }

        public override void Init()
        {
            Vector2 pos = new(75, 80);

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

            var arr = new[] {"1", "2", "22", "hi", "bye", "no", "u", "yeet", "8", "not 10", "double 1", "yes"};
            _lv = new Listview(pos, 175, i =>
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
            }, () => arr.Length, 7);
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

            // _scissorArea.MaskDraw(() =>
            // {
            //     _scissorArea.Draw(new(255, 255, 255, 10));
            //     DrawText("Move the mouse around to reveal this text!", 190, 200, 20, LIGHTGRAY);
            // });
        }
    }
}