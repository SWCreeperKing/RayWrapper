using System.Numerics;
using ImGuiNET;
using RayWrapper;
using RayWrapper.Imgui;
using RayWrapper.Imgui.Widgets;
using RayWrapper.Vars;
using static ImGuiNET.ImGuiTableFlags;
using Window = RayWrapper.Imgui.Widgets.Window;

new GameBox(new RayWrapperTesterImgui.Program(), new Vector2(1280, 800));

namespace RayWrapperTesterImgui
{
    public class Program : ImguiLoop
    {
        private string[] items = { "as", "b", "b", "c", "e", "d", "f", "x", "y", "m", "z" };
        private Func<int> w1Out, w2Out;
        private Table _table;
        private (float, float, float)[] arrayFloats;

        public override void NormalInit()
        {
            var r = new Random();
            arrayFloats = Enumerable.Repeat(0f, r.Next(30, 40))
                .Select(i => ((float) (r.Next(60, 80) * r.NextDouble()), (float) (r.Next(60, 80) * r.NextDouble()),
                    (float) (r.Next(60, 80) * r.NextDouble()))).ToArray();

            var arrSize = () => arrayFloats.Length;

            _table = new Table("table obj", RowBg | Borders);
            _table.AddColumn(new Column("h1", arrSize, i => ImGui.Text($"{arrayFloats[i].Item1}")));
            _table.AddColumn(new Column("h2", arrSize, i => ImGui.Text($"{arrayFloats[i].Item2}")));
            _table.AddColumn(new SortableColumn("sort", arrSize, i => ImGui.Text($"{arrayFloats[i].Item3}"),
                s => arrayFloats = s switch
                {
                    ImGuiSortDirection.Ascending => arrayFloats.OrderBy(f => f.Item3).ToArray(),
                    ImGuiSortDirection.Descending => arrayFloats.OrderByDescending(f => f.Item3).ToArray(),
                    _ => arrayFloats
                }));

            var w1 = new Window("test 1")
                .AddListView("", () => items, out w1Out)
                .AddButton("button", () => Logger.Log("pushed"))
                .AddInputBox("input", new InputHint("", Logger.Log, "Hint"))
                .AddInputBox("int3 input",
                    new InputInts(new[] { 3, 5, 6, 9 }, arr => Logger.Log($"{arr[0]}, {arr[1]}, {arr[2]}, {arr[3]}")));
            var w2 = new Window("test 2")
                .AddListView("", () => items, out w2Out)
                .AddCheckBox("hi", out _)
                .AddNonWidget(ImGui.SameLine)
                .AddBullet("hi")
                .AddProgressBar(() => i / 1000f, new Vector2(250, 25))
                .AddSlider("slider", new SliderFloat(50, f => Logger.Log(f)));
            RegisterWidgets(w1, w2, new ExampleWindow("example window"));
            // RlImgui.SetScale(2);
        }

        public int i = 0;

        public override void NormalUpdateLoop()
        {
            i++;
            i %= 1000;
            var w1I = w1Out.Invoke();
            var w2I = w2Out.Invoke();
            if (w1I != -1) Logger.Log($"Selected from w1: [{items[w1I]}]");
            if (w2I != -1) Logger.Log($"Selected from w2: [{items[w2I]}]");
        }

        public int a = -500;
        public int b = 500;
        public float[] vals = { 53.3f, 35.6f, 34, 23, 74 };
        public float rad;
        public ImGuiSortDirection dirCache = ImGuiSortDirection.None;
        
        static int flags = (int) (Borders | RowBg);
        
        public override void ImguiRenderLoop()
        {
            _table.Render();
            // ImGui.ShowDemoWindow();
        }
    }
}