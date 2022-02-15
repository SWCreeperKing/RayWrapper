using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static Raylib_cs.Raylib;
using static RayWrapper.GameBox;
using static RayWrapper.GeneralWrapper;

namespace RayWrapper.Objs
{
    public class InputBox : GameObject
    {
        // func: isControl?, cursor pos, max leng, current text
        // func return: (cursorpos, text)
        private readonly Dictionary<KeyboardKey, Func<bool, int, int, string, (int cur, string txt)>> _actions = new()
        {
            { KeyboardKey.KEY_LEFT, (_, p, _, s) => (p > 0 ? p - 1 : p, s) },
            { KeyboardKey.KEY_RIGHT, (_, p, _, s) => (p < s.Length ? p + 1 : p, s) },
            {
                KeyboardKey.KEY_BACKSPACE, (c, p, _, s) =>
                {
                    if (!c)
                    {
                        if (p > 0) s = s.Remove(p-- - 1, 1);
                        return (p, s);
                    }

                    var lastSpace = Math.Max(0, s[..p].LastIndexOf(' '));
                    return (lastSpace, s.Remove(lastSpace, p - lastSpace));
                }
            },
            { KeyboardKey.KEY_DELETE, (_, p, _, s) => (p, p < s.Length ? s.Remove(p, 1) : s) },
            { KeyboardKey.KEY_HOME, (_, _, _, s) => (0, s) },
            { KeyboardKey.KEY_END, (_, _, _, s) => (s.Length, s) },
            {
                KeyboardKey.KEY_V, (c, p, m, s) =>
                {
                    if (!c || s.Length >= m) return (p, s);
                    var txt = FromClipboard().Replace("\r", "").Replace("\n", " ");
                    var txtLeng = Math.Min(m - s.Length, txt!.Length);
                    return (p + txtLeng, s.Insert(p, txt[..txtLeng] ?? ""));
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

        private int _curPos;
        private int _frameTime;
        private readonly Label _label;
        private long _lastTime;
        private readonly int _max;
        private bool _selected;
        private readonly int _show;
        private string _text = "";
        public Action<string> onEnter;
        public Action<string> onExit;

        public InputBox(Vector2 pos, int maxCharacterShow = 20, int maxCharacters = 40)
        {
            _show = maxCharacterShow;
            _max = maxCharacters;
            _label = new Label(
                    new Rectangle(pos.X, pos.Y, 16 * _show, FontManager.GetDefFont().MeasureText("!").Y),
                    string.Join(",", Enumerable.Repeat(" ", _show)))
                { outline = new Actionable<bool>(true) };
            _lastTime = GetTimeMs();
        }

        protected override void UpdateCall()
        {
            var isLeft = (bool)IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON);
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
                    (_curPos, _text) =
                        v.Invoke(
                            IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) ||
                            IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL), _curPos, _max, _text);
                    _lastTime = GetTimeMs();
                }
            }

            var fps = GetFPS();

            try
            {
                _frameTime %= fps;
                _frameTime++;
            }
            catch
            {
            }

            var flash = _frameTime % (fps * .33) > fps * .18;
            var start = 0;
            var end = Math.Min(_show, _text.Length);
            var curs = _curPos;

            if (curs >= _show / 2)
                (start, curs, end) = (curs - _show / 2, _show / 2, Math.Min(_text.Length, _curPos + _show / 2));
            if (start > _max - _show) (start, curs) = (_max - _show, _curPos - (_max - _show));

            _label.text = $"> {(_selected ? _text[start..end].Insert(curs, flash ? " " : "|") : _text[start..end])}";
            _label.Update();
        }

        protected override void RenderCall() => _label.Render();

        public void Input()
        {
            var (c, cc) = (GetCharPressed(), GetKeyPressed());
            while (c > 0 || cc > 0)
            {
                if ((KeyboardKey)cc == KeyboardKey.KEY_ENTER) onEnter?.Invoke(_text);
                if (c is >= 32 and <= 125 && _text.Length < _max) _text = _text.Insert(_curPos++, $"{(char)c}");
                (c, cc) = (GetCharPressed(), GetKeyPressed());
            }
        }

        public void Clear() => (_text, _curPos) = ("", 0);
        public void SetText(string text) => (_text, _curPos) = (text, text.Length);
    }
}