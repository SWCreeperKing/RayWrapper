using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Var_Interfaces;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;
using static RayWrapper.GameBox;
using static RayWrapper.GeneralWrapper;

namespace RayWrapper.Objs;

public class InputBox : GameObject
{
    public static Style defaultStyle = new();

    // func: isControl?, cursor pos, max leng, current text
    // func return: (cursorpos, text)
    private readonly IDictionary<KeyboardKey, Func<bool, int, int, string, (int cur, string txt)>> _actions =
        new Dictionary<KeyboardKey, Func<bool, int, int, string, (int cur, string txt)>>()
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
                    var txt = FromClipboard().Replace("\r", string.Empty).Replace("\n", " ");
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

    public override Vector2 Position
    {
        get => _label.Position;
        set => _label.Position = value;
    }

    public override Vector2 Size => _label.Size;

    public Style style = defaultStyle.Copy();
    public Action<string> onEnter;
    public Action<string> onExit;

    private readonly Label _label;
    private readonly int _max;
    private readonly int _show;
    private int _curPos;
    private int _frameTime;
    private long _lastTime;
    private bool _selected;
    private string _text = string.Empty;
    private Cooldown _backspaceCool = new(650);
    private Cooldown _deleteCool = new(55);

    public InputBox(Vector2 pos, int maxCharacterShow = 20, int maxCharacters = 40)
    {
        _show = maxCharacterShow;
        _max = maxCharacters;
        _label = new Label(
            new Rectangle(pos.X, pos.Y, 16 * _show, style.labelStyle.textStyle.MeasureText("!").Y + 8),
            string.Join(",", Enumerable.Repeat(" ", _show)))
        {
            style = style.labelStyle
        };
        _lastTime = GetTimeMs();
    }

    protected override void UpdateCall()
    {
        var isLeft = IsMouseButtonPressed(MOUSE_LEFT_BUTTON);
        switch (isLeft)
        {
            case true when !_label.Rect.IsMouseIn() && _selected:
                _selected = false;
                onExit?.Invoke(_text);
                break;
            case true when !_selected && _label.Rect.IsMouseIn():
                _selected = true;
                break;
        }

        if (_selected)
        {
            Input();
            foreach (var (_, v) in _actions.Where(key => IsKeyDown(key.Key) && GetTimeMs() - _lastTime > 133))
            {
                (_curPos, _text) = v.Invoke(IsControl(), _curPos, _max, _text);
                _lastTime = GetTimeMs();
            }
            BackSpaceCheck();
        }

        var fps = GetFPS();

        _frameTime %= Math.Max(1, fps); // fix divide by 0
        _frameTime++;

        var flash = _frameTime % (fps * .33) > fps * .18;
        var start = 0;
        var end = Math.Min(_show, _text.Length);
        var curs = _curPos;

        if (curs >= _show / 2)
        {
            (start, curs, end) = (curs - _show / 2, _show / 2, Math.Min(_text.Length, _curPos + _show / 2));
        }

        if (start > _max - _show)
        {
            (start, curs) = (_max - _show, _curPos - (_max - _show));
        }

        _label.text =
            $"{style.startChar} {(_selected ? _text[start..end].Insert(curs, $"{(flash ? ' ' : style.cursorChar)}") : _text[start..end])}";
        _label.Update();
    }

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

    protected override void RenderCall()
    {
        _label.Render();
        if (_label.Rect.IsMouseIn()) GameBox.SetMouseCursor(MouseCursor.MOUSE_CURSOR_IBEAM);
    }

    public void Input()
    {
        var (c, cc) = (GetCharPressed(), GetKeyPressed_());
        while (c > 0 || cc > 0)
        {
            if (cc == KeyboardKey.KEY_ENTER) onEnter?.Invoke(_text);
            if (c is >= 32 and <= 125 && _text.Length < _max) _text = _text.Insert(_curPos++, $"{(char) c}");
            (c, cc) = (GetCharPressed(), GetKeyPressed_());
        }
    }

    public bool IsControl() => IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) || IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL);
    public void Clear() => (_text, _curPos) = (string.Empty, 0);
    public void SetText(string text) => (_text, _curPos) = (text, text.Length);
    public string GetText() => _text;

    public class Style : IStyle<Style>
    {
        public Label.Style labelStyle = new();
        public char startChar = '>';
        public char cursorChar = '|';

        public Style Copy()
        {
            return new Style
            {
                labelStyle = labelStyle.Copy(), startChar = startChar, cursorChar = cursorChar
            };
        }
    }
}