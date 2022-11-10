using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.Extras;
using RayWrapper.Base.GameBox;
using RayWrapper.Base.GameObject;
using RayWrapper.Base.Primitives;
using static Raylib_CsLo.Raylib;
using static RayWrapper.Base.GameBox.GameBox;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.LegacyUI.UI;

public class InputBox : GameObject
{
    public static Style defaultStyle = new();

    // func: isControl?, cursor pos, max leng, current text
    // func return: (cursorpos, text)
    private readonly IDictionary<KeyboardKey, Func<bool, int, int, string, (int cur, string txt)>> _actions =
        new Dictionary<KeyboardKey, Func<bool, int, int, string, (int cur, string txt)>>
        {
            { KeyboardKey.KEY_LEFT, (_, p, _, s) => (p > 0 ? p - 1 : p, s) },
            { KeyboardKey.KEY_RIGHT, (_, p, _, s) => (p < s.Length ? p + 1 : p, s) },
            { KeyboardKey.KEY_DELETE, (_, p, _, s) => (p, p < s.Length ? s.Remove(p, 1) : s) },
            { KeyboardKey.KEY_HOME, (_, _, _, s) => (0, s) },
            { KeyboardKey.KEY_END, (_, _, _, s) => (s.Length, s) },
            {
                KeyboardKey.KEY_V, (c, p, m, s) =>
                {
                    if (!c || s.Length >= m) return (p, s);
                    var txt = Core.FromClipboard().Replace("\r", string.Empty).Replace("\n", " ");
                    var txtLength = Math.Min(m - s.Length, txt!.Length);
                    return (p + txtLength, s.Insert(p, txt[..txtLength] ?? string.Empty));
                }
            },
            {
                KeyboardKey.KEY_C, (c, p, _, s) =>
                {
                    if (c) SetClipboardText(s);
                    return (p, s);
                }
            }
        };

    public string Text
    {
        get => _text;
        set => (_text, _curPos) = (value, value.Length);
    }

    public Style style = defaultStyle.Copy();
    public bool numbersOnly;
    public bool password;
    public Action<string> onEnter;
    public Action<string> onExit;
    public Action<string> onInput;

    private readonly int _max;
    private float _width;
    private float _h = 30;
    private int _curPos;
    private int _frameTime;
    private long _lastTime;
    private bool _selected;
    private string _text = string.Empty;
    private Cooldown _backspaceCool = new(650);
    private Cooldown _deleteCool = new(55);

    public InputBox(int x = 0, int y = 0, float w = 0, int maxCharacters = 40) : this(new Vector2(x, y), w,
        maxCharacters)
    {
    }

    public InputBox(Vector2 pos, float width = 0, int maxCharacters = 40)
    {
        Position = pos;
        _width = width == 0 ? style.textStyle.MeasureText("Click to start typing").X : width;
        _max = maxCharacters;
        _lastTime = GetTimeMs();
    }

    protected override void UpdateCall(float dt)
    {
        var rect = GetRect();
        var startText = _text;
        var isLeft = IsMouseButtonPressed(MOUSE_LEFT_BUTTON);
        switch (isLeft)
        {
            case true when !rect.IsMouseIn() && _selected:
                _selected = false;
                onExit?.Invoke(_text);
                break;
            case true when !_selected && rect.IsMouseIn():
                _selected = true;
                break;
        }

        if (_selected)
        {
            Input();
            foreach (var (_, v) in _actions.Where(key => IsKeyDown(key.Key) && GetTimeMs() - _lastTime > 133))
            {
                (_curPos, _text) = v(IsControl(), _curPos, _max, _text);
                _lastTime = GetTimeMs();
            }

            BackSpaceCheck();
        }

        if (_text != startText) onInput?.Invoke(_text);
    }

    protected override void RenderCall()
    {
        var fps = GetFPS();
        _frameTime %= Math.Max(1, fps); // fix divide by 0
        _frameTime++;

        var txt = password ? string.Join("", Enumerable.Repeat("*", _text.Length)) : _text;
        style.Draw(txt, Position, _width, _curPos, _selected, _frameTime % (fps * .33) > fps * .18);
    }

    protected override Vector2 GetSize() => new(_width, _h);
    protected override void UpdateSize(Vector2 newSize) => _width = newSize.X;

    public void BackSpaceCheck()
    {
        if (IsKeyUp(KeyboardKey.KEY_BACKSPACE))
        {
            _deleteCool.ResetTick();
            _backspaceCool.ResetTick();
            return;
        }

        void BackSpacePressed()
        {
            if (!IsControl())
            {
                if (_curPos > 0) _text = _text.Remove(--_curPos, 1);
            }
            else
            {
                var lastSpace = Math.Max(0, _text[.._curPos].LastIndexOf(' '));
                _text = _text.Remove(lastSpace, _curPos - lastSpace);
                _curPos = lastSpace;
            }
        }

        if (IsKeyPressed(KeyboardKey.KEY_BACKSPACE)) BackSpacePressed();
        if (!IsKeyDown(KeyboardKey.KEY_BACKSPACE)) return;
        _backspaceCool.UpdateTime(false);
        if (_backspaceCool.IsTime() && _deleteCool.UpdateTime()) BackSpacePressed();
    }

    public void Input()
    {
        var (c, cc) = (GetCharPressed(), GetKeyPressed_());
        while (c > 0 || cc > 0)
        {
            if (cc == KeyboardKey.KEY_ENTER) onEnter?.Invoke(_text);
            if (numbersOnly)
            {
                if (c is >= 48 and <= 57 or 69 or 101 or 44 or 46 && _text.Length < _max)
                    _text = _text.Insert(_curPos++, $"{(char) c}");
            }
            else if (c is >= 32 and <= 125 && _text.Length < _max) _text = _text.Insert(_curPos++, $"{(char) c}");

            (c, cc) = (GetCharPressed(), GetKeyPressed_());
        }
    }

    public bool IsControl() => IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) || IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL);
    public void Clear() => (_text, _curPos) = (string.Empty, 0);

    public class Style : IStyle<Style>
    {
        public string phantomText = "Click to start typing";
        public RectStyle backStyle = new();
        public OutlineStyle outline = new();
        public Text.Style textStyle = new();
        public Color phantomColor = WHITE.SetAlpha(150);
        public Color fontColor = SKYBLUE;
        public char cursorChar = '|';
        public char emptyChar = ' ';

        private bool _showPhantom;

        public void Draw(string text, Vector2 pos, float width, int cursorPosition, bool selected, bool cursorVisible)
        {
            _showPhantom = !text.Trim().Any() && !selected;
            textStyle.color = _showPhantom ? phantomColor : fontColor;

            var rect = new Rectangle(pos, textStyle.MeasureText(text) with { X = width });
            if (rect.Grow(4).IsMouseIn()) GameBox.SetMouseCursor(MouseCursor.MOUSE_CURSOR_IBEAM);

            Rectangle vRect, g4;
            if (selected)
            {
                var nText = text.Insert(cursorPosition, (cursorVisible ? cursorChar : emptyChar).ToString());
                var nRect = rect.Clone();
                var size = textStyle.MeasureText(nText);
                size = size with { X = size.X - rect.W };

                vRect = rect.Clone();
                vRect.H = size.Y;
                
                g4 = vRect.Grow(4);
                backStyle.Draw(g4);
                outline.Draw(g4);

                if (size.X <= 0)
                {
                    vRect.MaskDraw(() => textStyle.Draw(nText, rect));
                    return;
                }

                var percent = (float) cursorPosition / text.Length;
                nRect.X -= size.X * percent;

                vRect.MaskDraw(() => textStyle.Draw(nText, nRect.Pos));
                return;
            }

            var rText = _showPhantom ? phantomText : text;
            vRect = rect.Clone();
            vRect.H = textStyle.MeasureText(rText).Y;
            
            g4 = vRect.Grow(4);
            backStyle.Draw(g4);
            outline.Draw(g4);
            vRect.MaskDraw(() => textStyle.Draw(rText, rect));
        }

        public Style Copy()
        {
            return new Style
            {
                backStyle = backStyle.Copy(), textStyle = textStyle.Copy(), cursorChar = cursorChar, outline = outline,
                emptyChar = emptyChar, phantomText = phantomText, phantomColor = phantomColor, fontColor = fontColor
            };
        }
    }
}