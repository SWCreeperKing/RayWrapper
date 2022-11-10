using System.Numerics;
using ImGuiNET;
using Raylib_CsLo;
using RayWrapper.Base.GameBox;
using RayWrapper.Imgui.Widgets;
using ZimonIsHimUtils.ExtensionMethods;
using static Raylib_CsLo.KeyboardKey;

namespace RayWrapper.Imgui.Extras;

public class TextEditor : DrawWindowBase
{
    public static readonly float holdTime = .330f;
    public static readonly float spamTime = .070f;

    public static readonly KeyboardKey[] keys = { KEY_UP, KEY_DOWN, KEY_RIGHT, KEY_LEFT, KEY_BACKSPACE };

    public uint color = new Vector4(0, 255, 255, 255).ToUint();
    public uint color2 = new Vector4(75, 75, 75, 255).ToUint();
    public string rawIn;
    public HashSet<HashSet<Glyph>> lines = new();
    public HashSet<HashSet<Glyph>> numberLines = new();
    public Vector2 cursorPos;
    public List<string> text;
    public Dictionary<KeyboardKey, float> timers = new();

    private long _lastTime;

    public TextEditor(string name, ImGuiWindowFlags configFlags =
        ImGuiWindowFlags.AlwaysHorizontalScrollbar | ImGuiWindowFlags.AlwaysVerticalScrollbar) : base(name, configFlags)
    {
        throw new ApplicationException("Text Editor is still a [WIP]");
        text = Enumerable.Range(1, 150).Select(i => $"{i} |").ToList();
        text = text.Select(s => string.Join("", Enumerable.Repeat(" ", 8 - s.Length)) + s).ToList();
    }

    protected override void WindowUpdate(float dt)
    {
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
        
        LineUpdate();
        
        _lastTime = now;
    }

    public void KeyboardInput(KeyboardKey key)
    {
        switch (key)
        {
            case KEY_UP:
                cursorPos.Y--;
                break;
            case KEY_DOWN:
                cursorPos.Y++;
                break;
        }

        if (cursorPos.Y < 0) cursorPos.Y = 0;
    }

    public void LineUpdate()
    {
        
    }

    public override void DrawRender(ImDrawListPtr drawLayer, Vector2 offset, Vector2 size)
    {
        var maxSize = Vector2.Zero;

        foreach (var textSize in text.Select(s => ImGui.CalcTextSize(s)))
        {
            maxSize.X = Math.Max(textSize.X, maxSize.X);
            maxSize.Y += textSize.Y;
        }

        var scroll = new Vector2(ImGui.GetScrollX(), ImGui.GetScrollY());

        ImGui.BeginChild("subspace", maxSize, true, ImGuiWindowFlags.ChildWindow);
        var trueOffset = offset + ImGui.GetCursorPos() - maxSize * (scroll / maxSize);

        var addY = 0f;
        for (var i = 0; i < text.Count; i++)
        {
            var s = text[i];
            var sSize = ImGui.CalcTextSize(s);
            var beforeOffset = trueOffset with { Y = trueOffset.Y + addY };
            drawLayer.AddText(beforeOffset, color, s);
            addY += sSize.Y;
            if (cursorPos.Y == i) drawLayer.AddRect(beforeOffset, beforeOffset + sSize, color2);
        }

        ImGui.EndChild();

        ImGui.SetCursorPos(maxSize);
    }

    public readonly struct Glyph
    {
        public readonly char glyph;
        public readonly uint color;
        public readonly Vector2 size;

        public Glyph(char glyph, uint color)
        {
            this.glyph = glyph;
            this.color = color;
            size = ImGui.CalcTextSize($"{glyph}");
        }
    }
}