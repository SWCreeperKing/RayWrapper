using System.Numerics;
using RayWrapper;
using RayWrapper.Imgui;
using RayWrapper.Imgui.Widgets;
using RayWrapper.Vars;

new GameBox(new RayWrapperTesterImgui.Program(), new Vector2(1280, 800));

namespace RayWrapperTesterImgui
{
    public class Program : ImguiLoop
    {
        private string[] items = { "a", "b", "b", "c", "e", "d", "f", "x", "y", "m", "z" };
        private Func<int> w1Out, w2Out;
            
        public override void NormalInit()
        {
            var w1 = new Window("test 1").AddListView("", () => items, out w1Out);
            var w2 = new Window("test 2").AddListView("", () => items, out w2Out);
            RegisterWidgets(w1, w2);
        }

        public override void NormalUpdateLoop()
        {
            var w1i = w1Out.Invoke();
            var w2i = w2Out.Invoke();
            if (w1i != -1) Logger.Log($"Selected from w1: [{items[w1i]}]");
            if (w2i != -1) Logger.Log($"Selected from w2: [{items[w2i]}]");
        }
    }
}