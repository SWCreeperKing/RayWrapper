using System.Numerics;
using ImGuiNET;
using RayWrapper;
using RayWrapper.Imgui;
using RayWrapper.Imgui.Widgets;
using RayWrapper.Vars;
using Window = RayWrapper.Imgui.Widgets.Window;

new GameBox(new RayWrapperTesterImgui.Program(), new Vector2(1280, 800));

namespace RayWrapperTesterImgui
{
    public class Program : ImguiLoop
    {
        private string[] items = { "as", "b", "b", "c", "e", "d", "f", "x", "y", "m", "z" };
        private Func<int> w1Out, w2Out;

        public override void NormalInit()
        {
            var w1 = new Window("test 1")
                .AddListView("", () => items, out w1Out)
                .AddButton("button", () => Logger.Log("pushed"))
                .AddInputBox("input", new Hint("", Logger.Log, "Hint"));
            var w2 = new Window("test 2")
                .AddListView("", () => items, out w2Out)
                .AddCheckBox("hi", out _)
                .AddNonWidget(ImGui.SameLine)
                .AddBullet("hi");
            RegisterWidgets(w1, w2, new ExampleWindow("example window"));
        }

        public override void NormalUpdateLoop()
        {
            var w1I = w1Out.Invoke();
            var w2I = w2Out.Invoke();
            if (w1I != -1) Logger.Log($"Selected from w1: [{items[w1I]}]");
            if (w2I != -1) Logger.Log($"Selected from w2: [{items[w2I]}]");
        }

        public int i = 0;
        public double d = 0;
        public int[] ii = { 0, 0, 0, 0 };
        public bool b;

        public override void ImguiRenderLoop()
        {
        }
    }
}