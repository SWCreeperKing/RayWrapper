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
        public Action<string> onEnter;

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
            {KeyboardKey.KEY_HOME, (_, _, _, s) => (0, s)},
            {KeyboardKey.KEY_END, (_, _, _, s) => (s.Length, s)},
            {
                KeyboardKey.KEY_V, (c, p, m, s) =>
                {
                    if (!c || s.Length >= m) return (p, s);
                    var txt = Raylib.GetClipboardText();
                    var txtLeng = Math.Min(m - s.Length, txt!.Length);
                    s = s.Insert(p, txt[..txtLeng] ?? "");
                    return (p + txtLeng, s);
                }
            },
            {
                KeyboardKey.KEY_C, (c, p, _, s) =>
                {
                    if (c) Raylib.SetClipboardText(s);
                    return (p, s);
                }
            },
            {KeyboardKey.KEY_SCROLL_LOCK, (_, _, _, _) => (0, "")}
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
            var fps = Raylib.GetFPS();
            _frameTime++;
            _frameTime %= fps;

            Input();

                foreach (var (_, v) in _actions.Where(key => (Raylib.IsKeyDown(key.Key) && (_frameTime + 1) % (fps/6) == 0) || Raylib.IsKeyPressed(key.Key)))
                    (_curPos, _text) =
                        v.Invoke(
                            Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) ||
                            Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL), _curPos, _max, _text);

            var flash = _frameTime % (fps * .33) > fps * .18;
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

        protected override void RenderCall()
        {
            _label.Render();
        }

        public override void PositionChange(Vector2 v2) => _label.MoveTo(v2);

        public void Input()
        {
            var (c, cc) = (Raylib.GetCharPressed(), Raylib.GetKeyPressed());
            while (c > 0 || cc > 0)
            {
                if ((KeyboardKey) cc == KeyboardKey.KEY_ENTER) onEnter.Invoke(_text);

                if (c is >= 32 and <= 125 && _text.Length < _max)
                    _text = _text.Insert(_curPos++, $"{(char) c}");

                (c, cc) = (Raylib.GetCharPressed(), Raylib.GetKeyPressed());
            }
        }

        public void Clear()
        {
            _text = "";
            _curPos = 0;
        }
    }
}