using ImGuiNET;
using Raylib_CsLo;
using RayWrapper.Base.CommandRegister;
using RayWrapper.Base.GameBox;
using RayWrapper.Imgui.Widgets;
using static RayWrapper.Base.GameBox.Logger.Level;

namespace RayWrapper.Imgui.Extras;

public class ImGuiConsole : WindowBase
{
    public static readonly Dictionary<Logger.Level, uint> colors = new()
    {
        [Info] = Raylib.DARKGREEN.ToUint(),
        [Debug] = Raylib.ORANGE.ToUint(),
        [Warning] = Raylib.YELLOW.ToUint(),
        [SoftError] = Raylib.RED.ToUint(),
        [Error] = Raylib.RED.ToUint(),
        [Other] = Raylib.BLUE.ToUint(),
        [Special] = Raylib.SKYBLUE.ToUint()
    };

    public int maxScrollback = 200;
    public bool scrollToBottom = true;

    private List<(Logger.Level, string)> scrollback = new();
    private string input = "";
    private bool _toScroll;

    public ImGuiConsole() : base("Console", ImGuiWindowFlags.None)
    {
        Logger.LoggerListeners += (level, s) =>
        {
            scrollback.Add((level, s));
            UpdateScrollback();
        };
    }

    protected override void WindowRender()
    {
        var wSize = ImGui.GetContentRegionMax();
        ImGui.BeginChild("console.child", wSize with { Y = wSize.Y - 50 }, true,
            ImGuiWindowFlags.ChildWindow | ImGuiWindowFlags.AlwaysHorizontalScrollbar);

        foreach (var (level, s) in scrollback)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, colors[level]);
            ImGui.Text(s);
            ImGui.PopStyleColor();
        }

        if (scrollToBottom && _toScroll) ImGui.SetScrollHereY();
        if (_toScroll) _toScroll = false;

        ImGui.EndChild();

        if (!ImGui.InputTextWithHint("", "Command", ref input, 999, ImGuiInputTextFlags.EnterReturnsTrue)) return;
        scrollback.AddRange(CommandRegister.ExecuteCommand(input).Reverse());
        UpdateScrollback();
        input = "";
    }

    private void UpdateScrollback()
    {
        while (scrollback.Count > maxScrollback) scrollback.RemoveAt(0);
        if (scrollToBottom) _toScroll = true;
    }
}