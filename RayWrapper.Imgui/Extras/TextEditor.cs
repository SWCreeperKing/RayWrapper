using System.Numerics;
using System.Text.RegularExpressions;
using ImGuiNET;
using Microsoft.VisualBasic;
using Raylib_CsLo;
using RayWrapper.Base.GameBox;
using RayWrapper.Imgui.Widgets;
using ZimonIsHimUtils.ExtensionMethods;
using static Raylib_CsLo.KeyboardKey;
using static Raylib_CsLo.MouseButton;

namespace RayWrapper.Imgui.Extras;

//todo: support multiline (future)
public class TextEditor : DrawWindowBase
{
    public static readonly float holdTime = .330f;
    public static readonly float spamTime = .055f;

    public static readonly KeyboardKey[] keys = { KEY_UP, KEY_DOWN, KEY_RIGHT, KEY_LEFT, KEY_BACKSPACE };

    public List<Line> lines;
    public Vector2 cursorPos;
    public Dictionary<KeyboardKey, float> timers = new();

    private long _lastTime;
    private bool _updateCursor;

    public TextEditor(string name, ImGuiWindowFlags configFlags =
        ImGuiWindowFlags.AlwaysHorizontalScrollbar | ImGuiWindowFlags.AlwaysVerticalScrollbar) : base(name, configFlags)
    {
        // throw new ApplicationException("Text Editor is still a [WIP]");
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

        LineUpdate();

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
        var clicked = Raylib.IsMouseButtonPressed(MOUSE_BUTTON_LEFT);
        var maxSize = Vector2.Zero;
        var cursorScroll = 0f;

        foreach (var textSize in lines.Select(l => l.Size))
        {
            maxSize.X = Math.Max(textSize.X, Math.Max(maxSize.X, size.X - 40));
            maxSize.Y += textSize.Y;
        }

        var scroll = new Vector2(ImGui.GetScrollX(), ImGui.GetScrollY());

        ImGui.BeginChild("subspace", maxSize, true, ImGuiWindowFlags.ChildWindow);
        var trueOffset = offset + ImGui.GetCursorPos() - maxSize * (scroll / maxSize);
        var wSize = offset + ImGui.GetItemRectSize();

        var addY = 0f;
        var newCursor = -1;
        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            var before = trueOffset with { Y = trueOffset.Y + addY };
            addY += line.Draw(drawLayer, before, wSize.X, cursorPos, clicked, out var nc);
            if (nc) newCursor = i;

            if (cursorPos.Y != i) continue;
            cursorScroll = before.Y;
        }

        if (newCursor != -1)
        {
            cursorPos.Y = newCursor;
            _updateCursor = true;
        }

        ImGui.EndChild();

        if (_updateCursor && newCursor == -1)
        {
            ImGui.SetScrollFromPosY(cursorScroll);
            _updateCursor = false;
        }

        ImGui.SetCursorPos(maxSize);
    }

    public void LineUpdate()
    {
    }

    public void OpenFile(string file)
    {
        try
        {
            if (!File.Exists(file)) return;
            var i = 0;
            lines = File.ReadLines(file).Select(l => new Line(i++).SetLine(l)).ToList();
        }
        catch (Exception e)
        {
            Logger.Log(Logger.Level.SoftError, e.Message);
        }
    }

    /// <summary>
    /// syntax highlighting only works for 1 line string/comments
    /// </summary>
    public class Line
    {
        public static uint NumberColor = Raylib.SKYBLUE.ToUint();
        public static uint RectColor = Raylib.RAYWHITE.ToUint();
        public static uint BaseColor = Raylib.RAYWHITE.ToUint();

        // todo: move to syntax class
        public static Dictionary<Regex, uint> SyntaxHighlightingRegex = new()
        {
            [new Regex("(//.*)", RegexOptions.Compiled)] = Raylib.GRAY.ToUint(),
            [new Regex("((?:\\$|)\".*\")", RegexOptions.Compiled)] = Raylib.SKYBLUE.ToUint(),
            [new Regex("(\\w+?)\\(", RegexOptions.Compiled)] = Raylib.BLUE.ToUint()
        };

        // todo: move to syntax class
        public static Dictionary<string, uint> SyntaxHighlighting = new()
        {
            ["public"] = Raylib.PURPLE.ToUint(),
            ["private"] = Raylib.PURPLE.ToUint(),
            ["using"] = Raylib.PURPLE.ToUint(),
            ["void"] = Raylib.PURPLE.ToUint(),
            ["static"] = Raylib.PURPLE.ToUint(),
            ["var"] = Raylib.PURPLE.ToUint(),
            ["("] = Raylib.YELLOW.ToUint(),
            [")"] = Raylib.YELLOW.ToUint(),
            ["readonly"] = Raylib.PURPLE.ToUint(),
            ["string"] = Raylib.PURPLE.ToUint(),
            ["bool"] = Raylib.PURPLE.ToUint(),
            ["float"] = Raylib.PURPLE.ToUint(),
            ["uint"] = Raylib.PURPLE.ToUint(),
            ["int"] = Raylib.PURPLE.ToUint(),
            ["while"] = Raylib.PURPLE.ToUint(),
            ["foreach"] = Raylib.PURPLE.ToUint(),
            ["switch"] = Raylib.PURPLE.ToUint(),
            ["case"] = Raylib.PURPLE.ToUint(),
            ["break"] = Raylib.PURPLE.ToUint(),
            ["continue"] = Raylib.PURPLE.ToUint(),
        };

        private (Range range, uint color)[] _colorSetRange;
        private string _lineRaw;
        private char[] _line;
        private int _number;
        private bool _sizeInvalid = true;
        private bool _syntaxInvalid = true;
        private Vector2 _lineSize = Vector2.Zero;

        public Line(int lineNumber) => _number = lineNumber;

        public void CheckSelfForSyntaxHighlight()
        {
            _syntaxInvalid = false;
            var _colorRanges = new Dictionary<Range, uint>();
            _colorRanges[..12] = NumberColor;

            foreach (var (regex, color) in SyntaxHighlightingRegex)
            {
                var start = 12;
                while (regex.IsMatch(_line, start))
                {
                    var match = regex.Match(_lineRaw, start);
                    var g = match.Groups[1].Value;
                    start = _lineRaw.IndexOf(match.Value, start, StringComparison.Ordinal);
                    var index = _lineRaw.IndexOf(g, start, StringComparison.Ordinal);
                    var end = index + g.Length;
                    _colorRanges[index..end] = color;
                    start = end;
                }
            }

            while (_colorRanges.Keys.Any(k => _colorRanges.Keys.Any(kk => !k.Equals(kk) && k.Intercept(kk))))
            {
                var mainR = _colorRanges.Keys.First(k => _colorRanges.Keys.Any(kk => !k.Equals(kk) && k.Intercept(kk)));
                var intercepts = _colorRanges.Keys.Where(k => !mainR.Equals(k) && mainR.Intercept(k));
                var lowest = intercepts.Aggregate(mainR, (r1, r2) => r1.Start.Value < r2.Start.Value ? r1 : r2);
                var lowerestColor = _colorRanges[lowest];
                _colorRanges.Remove(mainR);
                intercepts.Each(k => _colorRanges.Remove(k));
                _colorRanges.Add(lowest, lowerestColor);
            }

            foreach (var (find, color) in SyntaxHighlighting)
            {
                var start = 12;
                int index;
                while ((index = _lineRaw.IndexOf(find, start, StringComparison.Ordinal)) != -1)
                {
                    if (_colorRanges.Keys.Any(r => r.IsInRange(index)))
                    {
                        start += find.Length;
                        continue;
                    }

                    var end = index + find.Length;
                    _colorRanges[index..end] = color;
                    start = end;
                }
            }

            var order = _colorRanges.Keys.OrderBy(r => r.Start.Value).ToArray();
            for (var i = 1; i < order.Length; i++)
            {
                var end = order[i - 1].End.Value;
                var start = order[i].Start.Value;
                if (start != end) _colorRanges.Add(end..start, BaseColor);
            }

            if (order[^1].End.Value < _lineRaw.Length) _colorRanges[order[^1].End.Value..] = BaseColor;

            _colorSetRange = _colorRanges.Select(kv => (kv.Key, kv.Value)).OrderBy(kv => kv.Key.Start.Value).ToArray();
        }

        public float Draw(ImDrawListPtr drawLayer, Vector2 pos, float mWidth, Vector2 cursor, bool clicked,
            out bool newCursor)
        {
            if (_sizeInvalid) CalcLineSize();
            if (_syntaxInvalid) CheckSelfForSyntaxHighlight();
            var xOff = pos.X;
            var yOff = 0f;
            Span<char> span = new(_line);

            foreach (var (range, color) in _colorSetRange)
            {
                try
                {
                    var str = span[range];
                    var sSize = RlImgui.CalcTextSpan(str);
                    yOff = Math.Max(yOff, sSize.Y);
                    drawLayer.AddText(pos with { X = xOff }, color, str);
                    xOff += sSize.X;
                }
                catch
                {
                    // ignored
                }
            }

            var maxW = new Vector2(Math.Max(xOff, mWidth), pos.Y + yOff);
            newCursor = cursor.Y != _number && ImGui.IsMouseHoveringRect(pos, maxW) && clicked;
            if (_number == cursor.Y) drawLayer.AddRect(pos, maxW, RectColor);

            return yOff;
        }

        public void CalcLineSize()
        {
            _lineSize = RlImgui.CalcTextSpan(_line);
            _sizeInvalid = false;
        }

        public Line SetLine(string line)
        {
            _line = (_lineRaw = $"{_number + 1,10} |{line}").ToCharArray();
            _sizeInvalid = true;
            return this;
        }

        public Vector2 Size
        {
            get
            {
                if (_sizeInvalid) CalcLineSize();
                return _lineSize;
            }
        }
    }
}