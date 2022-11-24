using System.Numerics;
using ImGuiNET;
using Raylib_CsLo;
using RayWrapper.Base.GameBox;
using RayWrapper.Imgui.Widgets;
using ZimonIsHimUtils.ExtensionMethods;
using static Raylib_CsLo.KeyboardKey;

namespace RayWrapperTesterImgui.Testing;

//todo: support multiline (future)
public class TextEditor : DrawWindowBase
{
    public static readonly float holdTime = .330f;
    public static readonly float spamTime = .055f;

    public static readonly KeyboardKey[] keys = { KEY_UP, KEY_DOWN, KEY_RIGHT, KEY_LEFT, KEY_BACKSPACE };
    public static readonly Dictionary<string, Vector2> tokenSizes = new();

    public string data;
    public Vector2 dataSize;
    public List<Token> lines;
    public Vector2 cursorPos;
    public Dictionary<KeyboardKey, float> timers = new();

    private long _lastTime;
    private bool _updateCursor;

    public TextEditor(string name, ImGuiWindowFlags configFlags =
        ImGuiWindowFlags.AlwaysHorizontalScrollbar | ImGuiWindowFlags.AlwaysVerticalScrollbar) : base(name, configFlags)
    {
    }

    protected override void WindowUpdate(float dt)
    {
        if (lines is null) return;
        var now = GameBox.GetTimeMs();

        var pressed = keys.Where(Raylib.IsKeyPressed);
        pressed.Each(KeyboardInput);

        var held = keys.Where(Raylib.IsKeyDown).Where(k => !pressed.Contains(k));
        timers.Where(dict => dict.Value <= 0).Select(dict => dict.Key).Each(key =>
        {
            if (held.Contains(key))
            {
                timers[key] = spamTime;
                KeyboardInput(key);
            }
            else timers.Remove(key);
        });
        timers.Keys.Each(k => timers[k] -= dt);
        held.Where(k => !timers.ContainsKey(k)).Each(k => timers[k] = holdTime);
        keys.Where(Raylib.IsKeyUp).Where(timers.ContainsKey).Each(k => timers.Remove(k));

        _lastTime = now;
    }

    public void KeyboardInput(KeyboardKey key)
    {
        _updateCursor = true;
        switch (key)
        {
            case KEY_UP:
                cursorPos.Y--;
                break;
            case KEY_DOWN:
                cursorPos.Y++;
                break;
            default:
                _updateCursor = false;
                break;
        }

        if (cursorPos.Y < 0) cursorPos.Y = 0;
        if (cursorPos.Y >= lines.Count) cursorPos.Y = lines.Count - 1;
    }

    public override void DrawRender(ImDrawListPtr drawLayer, Vector2 offset, Vector2 size)
    {
        if (lines is null) return;
        var maxSize = dataSize;
        var cursorScroll = 0f;

        var scroll = new Vector2(ImGui.GetScrollX(), ImGui.GetScrollY());

        ImGui.BeginChild("subspace", maxSize, true, ImGuiWindowFlags.ChildWindow);
        var trueOffset = offset + ImGui.GetCursorPos() - maxSize * (scroll / maxSize);

        var newPosition = trueOffset;
        var newCursor = -1;

        lines.Aggregate(newPosition, (current, line) => DrawToken(drawLayer, line, current, trueOffset.X));

        ImGui.EndChild();
        ImGui.SetCursorPos(maxSize);
    }

    public Vector2 DrawToken(ImDrawListPtr drawLayer, Token token, Vector2 pos, float xOff)
    {
        var sSize = GetSize(token.value);
        if (token.tokenType is FileLexers.BaseTokens.NewLine) return new Vector2(xOff, pos.Y + sSize.Y);

        drawLayer.AddText(pos, FileLexers.lexerColors[token.tokenType], token.value);
        return pos with { X = pos.X + sSize.X };
    }

    public Vector2 GetSize(string s)
    {
        if (!tokenSizes.ContainsKey(s)) tokenSizes[s] = ImGui.CalcTextSize(s);
        return tokenSizes[s];
    }

    public void OpenFile(string file)
    {
        try
        {
            if (!File.Exists(file)) return;
            var fileExt = file.Split('.')[^1];
            data = File.ReadAllText(file);

            lines = FileLexers.fileLexers[FileLexers.fileLexers.ContainsKey(fileExt) ? fileExt : "default"]
                .GetTokens(data);

            dataSize = ImGui.CalcTextSize(data);
            // Logger.Log($"[{string.Join(",", lines.Select(t => $"{t.value}={t.tokenType}"))}]");
        }
        catch (Exception e)
        {
            Logger.Log(Logger.Level.SoftError, e.Message);
        }
    }
}