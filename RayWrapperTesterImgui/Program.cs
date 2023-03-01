using System.Numerics;
using System.Text.RegularExpressions;
using ImGuiNET;
using Raylib_CsLo;
using RayWrapper.Base.GameBox;
using RayWrapper.Imgui.Extras;
using RayWrapper.Imgui.Widgets;
using RayWrapperTesterImgui.Testing;
using static ImGuiNET.ImGuiTableFlags;
using Group = RayWrapper.Imgui.Widgets.Group;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

new GameBox(new RayWrapperTesterImgui.Program(), new Vector2(1280, 800));

namespace RayWrapperTesterImgui
{
    public partial class Program : GameLoop
    {
        public int i;
        private string[] items = { "as", "b", "b", "c", "e", "d", "f", "x", "y", "m", "z" };
        private float[] vals = { 53.3f, 35.6f, 34, 23, 74 };
        private float num = 40;
        private (float, float, float)[] arrayFloats;
        private Func<int> w1Out, w2Out;
        private Table _table;
        private Tooltip _tooltip;
        private Popup _tooltipMenu;
        private TreeNode _treeNode;
        private Group _group;
        private Rectangle rect = new(10, 10, 50, 50);
        
        public override void Init()
        {
            var r = GameBox.Random;
            arrayFloats = Enumerable.Repeat(0f, r.Next(30, 40))
                .Select(_ => ((float) (r.Next(60, 80) * r.NextDouble()), (float) (r.Next(60, 80) * r.NextDouble()),
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

            _tooltip = new CompoundWidgetBuilder()
                .AddText("tool tip test")
                .AddPlotLines("plot", () => vals)
                .ToGroup(out _group)
                .ToPopup("tooltip menu", out _tooltipMenu)
                .ToTreeNode("node", out _treeNode)
                .ToTooltip();

            var w1 = new CompoundWidgetBuilder()
                .AddListView("", () => items, out w1Out)
                .AddButton("button", () => Logger.Log("pushed"))
                .AddInputBox("input", new InputHint("", Logger.Log, "Hint"))
                .AddInputBox("int3 input",
                    new InputInts(new[] { 3, 5, 6, 9 }, arr => Logger.Log($"{arr[0]}, {arr[1]}, {arr[2]}, {arr[3]}")))
                .AddNumberDrag("number", new DragFloat(num, f => num = f))
                .ToWindow("test 1");

            var w2 = new CompoundWidgetBuilder()
                .AddListView("", () => items, out w2Out)
                .AddCheckBox("hi", out _)
                .AddNonWidget(ImGui.SameLine)
                .AddBullet("hi")
                .AddProgressBar(() => i / 1000f, new Vector2(250, 25))
                .AddSlider("slider", new SliderFloat(50, f => Logger.Log($"Slider: {f}")))
                .ToWindow("test 2");

            var te = new TextEditor("editor");

            RegisterGameObj(w1, w2, new ExampleWindow("example window"),
                new ExampleDrawWindow("draw window", ImGuiWindowFlags.None), te, new FileExplorer(te.OpenFile),
                new ImGuiConsole());
        }

        public override void UpdateLoop(float dt)
        {
            i++;
            i %= 1000;
            var w1I = w1Out();
            var w2I = w2Out();
            if (w1I != -1) Logger.Log($"Selected from w1: [{items[w1I]}]");
            if (w2I != -1) Logger.Log($"Selected from w2: [{items[w2I]}]");
        }

        public override void RenderLoop()
        {
            ImGui.Text("HELLO");
            rect.Draw(Raylib.BLUE);
            if (rect.IsMouseIn() && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT))
            {
                ImGui.OpenPopup("tooltip menu");
            }
            else if (rect.IsMouseIn() && !ImGui.IsPopupOpen("tooltip menu")) _tooltip.Render();
            
            _treeNode.Render();
            _tooltipMenu.Render();
            _group.Render();
            _table.Render();
            ImGui.ShowDemoWindow();
        }
    }
}