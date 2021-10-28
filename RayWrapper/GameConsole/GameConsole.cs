using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using Raylib_cs;
using RayWrapper.Objs;
using RayWrapper.Vars;
using static RayWrapper.GameBox;
using static RayWrapper.GameConsole.CommandLineColor;

namespace RayWrapper.GameConsole
{
    public class GameConsole : GameObject
    {
        public static GameConsole singleConsole;

        private static string regString = @"^(\d{1,3}),(\d{1,3}),(\d{1,3})\|";
        private static List<string> _lines = new();
        private static Dictionary<int, Color> _colors = new();

        public override Vector2 Position { get; set; }
        public override Vector2 Size { get; }

        public ListView history;
        public InputBox ib;

        public GameConsole()
        {
            if (singleConsole is not null)
                throw new ApplicationException("Only 1 instance of GameConsole can be created");

            singleConsole = this;

            ib = new InputBox(new Vector2(12, 10), 78, 200);
            ib.onEnter = s =>
            {
                if (s.Length < 1) return;
                Logger.Log(Logger.Level.Info, $"from user: {s}");
                var split = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var write = split.Length == 1
                    ? CommandRegister.ExecuteCommand(s)
                    : CommandRegister.ExecuteCommand(split[0], split[1..]);
                ib.Clear();
                if (!write.Any() || write.Length == 1 && write[0] == "") return;
                WriteToConsole(write);
            };

            history = new ListView(new Vector2(12, 50), (int)(WindowSize.X - 24),
                i => _lines[Math.Abs(_lines.Count - 1 - i)], () => _lines.Count,
                (int)Math.Floor((WindowSize.Y - 50) / 45))
            {
                fontColors = i =>
                {
                    var key = Math.Abs(_lines.Count - 1 - i);
                    return _colors.ContainsKey(key) ? _colors[key] : Color.DARKGREEN;
                }
            };
        }

        protected override void UpdateCall()
        {
            history.Update();
            ib.Update();
        }

        protected override void RenderCall()
        {
            new Rectangle(0, 0, WindowSize.X, WindowSize.Y).Draw(new Color(0, 0, 0, 150));
            history.Render();
            ib.Render();
        }

        public void WriteToConsole(params string[] texts)
        {
            Logger.Log(Logger.Level.Info,
                $"from GameConsole: {string.Join("\n\t>", texts.Select(s => Regex.IsMatch(s, regString) ? s[(s.IndexOf('|') + 1)..] : s))}");
            int ToColor(string text) => Math.Clamp(int.Parse(text), 0, 255);
            if (!texts.Any())
            {
                WriteToConsole($"{YELLOW}|An Attempt to write to the console was made");
                return;
            }

            foreach (var text in texts)
            {
                if (Regex.IsMatch(text, regString))
                {
                    var match = Regex.Match(text, regString).Groups;
                    _colors.Add(_lines.Count,
                        new Color(ToColor(match[1].Value), ToColor(match[2].Value), ToColor(match[3].Value), 255));
                    _lines.Add(text[(text.IndexOf('|') + 1)..]);
                }
                else _lines.Add(text);
            }
        }

        public int ClearOutput()
        {
            var lineAmt = _lines.Count;
            _colors.Clear();
            _lines.Clear();
            return lineAmt;
        }
    }
}