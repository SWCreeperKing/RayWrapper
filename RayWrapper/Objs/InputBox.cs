using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    public class InputBox : GameObject
    {
        private int _curPos = 0;
        private int _frameTime;
        private Label _label;
        private int _max;
        private bool _selected;
        private int _show;
        private string _text = "";

        private Dictionary<KeyboardKey, Func<bool, int, int, string, (int cur, string txt)>> _actions = new()
        {
            {
                KeyboardKey.KEY_LEFT, (_, p, _, s) =>
                {
                    if (p > 0) p--;
                    return (p, s);
                }
            },
            {
                KeyboardKey.KEY_RIGHT, (_, p, _, s) =>
                {
                    if (p < s.Length) p++;
                    return (p, s);
                }
            },
            {
                KeyboardKey.KEY_BACKSPACE, (_, p, _, s) =>
                {
                    if (p > 0) s = s.Remove(p-- - 1, 1);
                    return (p, s);
                }
            },
            {
                KeyboardKey.KEY_DELETE, (_, p, _, s) =>
                {
                    if (p < s.Length) s = s.Remove(p, 1);
                    return (p, s);
                }
            },
            {KeyboardKey.KEY_HOME, (_, p, _, s) => (0, s)},
            {KeyboardKey.KEY_END, (_, p, _, s) => (s.Length, s)},
            {
                KeyboardKey.KEY_V, (c, p, m, s) =>
                {
                    if (!c || s.Length >= m) return (p, s);
                    var txt = Raylib.GetClipboardText();
                    s = s.Insert(p, txt[..Math.Min(m - s.Length, txt!.Length)] ?? "");
                    return (p, s);
                }
            },
            {
                KeyboardKey.KEY_C, (c, p, _, s) =>
                {
                    if (c) Raylib.SetClipboardText(s);
                    return (p, s);
                }
            },
            {KeyboardKey.KEY_SCROLL_LOCK, (c, p, _, s) => (0, "")}
        };

        public InputBox(Vector2 pos, int maxCharacterShow = 20, int maxCharacters = 40) : base(pos)
        {
            _show = maxCharacterShow;
            _max = maxCharacters;
            _label = new Label(
                new Rectangle(pos.X, pos.Y, 16 * _show,
                    GameBox.font.MeasureText("!").Y), string.Join(",", Enumerable.Repeat(" ", _show)));
        }

        public override void Update()
        {
            _frameTime++;
            _frameTime %= 60;

            Input();

            var flash = _frameTime % 20 > 11;
            var start = 0;
            var end = Math.Min(_show, _text.Length);
            var curs = _curPos;

            if (curs >= _show / 2)
            {
                start = curs - _show / 2;
                curs = _show / 2;
                end = Math.Min(_text.Length, _curPos + _show / 2);
            }

            if (start > _max - _show)
            {
                start = _max - _show;
                curs = _curPos - (_max - _show);
            }

            var sub = _text[start..end];
            var newString = sub.Insert(curs, flash ? " " : "|");
            _label.text = $" {newString}";
            _label.Update();
        }

        public override void Render()
        {
            _label.Render();
        }

        public override void PositionChange(Vector2 v2) => _label.MoveTo(v2);

        public void Input()
        {
            if (_text.Length == _max) return;
            var (c, cc) = (Raylib.GetCharPressed(), Raylib.GetKeyPressed());
            while (c > 0 || cc > 0)
            {
                if (_actions.ContainsKey((KeyboardKey) cc))
                {
                    Console.WriteLine("has");
                    (_curPos, _text) = _actions[(KeyboardKey) cc]
                        .Invoke(
                            Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) ||
                            Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL), _curPos, _max, _text);
                }
                
                if (c is >= 32 and <= 125)
                    _text = _text.Insert(_curPos++, $"{(char) c}");

                (c, cc) = (Raylib.GetCharPressed(), Raylib.GetKeyPressed());
            }
        }
    }
}