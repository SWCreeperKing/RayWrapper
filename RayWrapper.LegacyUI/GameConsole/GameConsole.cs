using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using Raylib_CsLo;
using RayWrapper.Base.CommandRegister;
using RayWrapper.Base.GameBox;
using RayWrapper.Base.GameObject;
using RayWrapper.LegacyUI.AlertBoxes;
using RayWrapper.LegacyUI.ListView;
using RayWrapper.LegacyUI.UI;
using static Raylib_CsLo.Raylib;
using static RayWrapper.Base.GameBox.AttributeManager;
using static RayWrapper.Base.GameBox.GameBox;
using static RayWrapper.Base.GameBox.Logger.Level;
using static RayWrapper.LegacyUI.GameConsole.ConsoleController;
using static RayWrapper.LegacyUI.GameConsole.GameConsole;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.LegacyUI.GameConsole;

public class GameConsole : GameObject
{
    public static GameConsole singleConsole;

    private static readonly Regex regString = new(@"^(\d{1,3}),(\d{1,3}),(\d{1,3})\|", RegexOptions.Compiled);
    private static IList<string> _lines = new List<string>();
    private static Dictionary<int, Color> _colors = new();

    public ListView.ListView history;
    public InputBox ib;

    private static readonly Color BackColor = new(0, 0, 0, 150);
    private readonly Rectangle back;

    public GameConsole()
    {
        if (singleConsole is not null)
            throw new ApplicationException("Only 1 instance of GameConsole can be created");

        singleConsole = this;

        ib = new InputBox(new Vector2(12, 10), WindowSize.X - 20);
        ib.onEnter = s =>
        {
            if (s.Length < 1) return;
            Logger.Log(Info, $"from user: {s}");
            var split = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var write = split.Length == 1
                ? CommandRegister.ExecuteCommand(s)
                : CommandRegister.ExecuteCommand(split[0], split[1..]);
            ib.Clear();
            if (!write.Any() || write.Length == 1 && write[0].Item2 == string.Empty) return;
            if (write.Length == 1) WriteToConsole($"{ColorDecipher(write[0].Item1)}{write[0].Item2}");
            else WriteToConsole(write.Select(ls => $"{ColorDecipher(ls.Item1)}{ls.Item2}").ToArray());
        };

        DefaultListItem defItem = new((int) (WindowSize.X - 24), () => _lines.Count,
            i => _lines[Math.Abs(_lines.Count - 1 - i)])
        {
            fontColorLookup = i =>
            {
                var key = Math.Abs(_lines.Count - 1 - i);
                return _colors.ContainsKey(key) ? _colors[key] : DARKGREEN;
            },
            labelStyle =
            {
                drawHover = false
            }
        };

        history = new ListView.ListView(new Vector2(12, 50), defItem, (int) Math.Floor((WindowSize.Y - 50) / 45));

        back = new Rectangle(Vector2.Zero, WindowSize);
        RegisterGameObj(history, ib);
    }
    
    protected override void RenderCall() => back.Draw(BackColor);

    public static void WriteToConsole(params string[] texts)
    {
        int ToColor(string text) => Math.Clamp(int.Parse(text, CultureInfo.InvariantCulture), 0, 255);
        if (!texts.Any())
        {
            WriteToConsole($"{CommandLineColor.YELLOW}An Attempt to write to the console was made");
            return;
        }

        foreach (var text in texts)
        {
            if (regString.IsMatch(text))
            {
                var match = regString.Match(text).Groups;
                _colors.Add(_lines.Count,
                    new Color(ToColor(match[1].Value), ToColor(match[2].Value), ToColor(match[3].Value), 255));
                _lines.Add(text[(text.IndexOf('|') + 1)..]);
            }
            else _lines.Add(text);
        }
    }

    public static int ClearOutput()
    {
        var lineAmt = _lines.Count;
        _colors.Clear();
        _lines.Clear();
        return lineAmt;
    }
}

public static class ConsoleController
{
    public static bool enableConsole = true;

    private static bool _isConsole;

    [GameBoxWedge(PlacerType.AfterInit)]
    public static void GameConsoleInit()
    {
        singleConsole = new GameConsole();
        CommandRegister.RegisterCommand<DefaultUiCommands>();
        Logger.LoggerListeners += (level, s) => WriteToConsole($"{ColorDecipher(level)}{s}");
    }

    public static string ColorDecipher(Logger.Level level)
    {
        return level switch
        {
            Info => CommandLineColor.GREEN,
            Debug => CommandLineColor.CYAN,
            Warning => CommandLineColor.YELLOW,
            Error or SoftError => CommandLineColor.RED,
            Other => CommandLineColor.BLUE,
            Special => CommandLineColor.SKYBLUE
        };
    }

    [GameBoxWedge(PlacerType.BeforeUpdate)]
    public static void UpdateConsole(float dt)
    {
        if (IsKeyPressed(KeyboardKey.KEY_GRAVE) && enableConsole) _isConsole = !_isConsole;
        if (!AlertBase.AlertController.alertQueue.Any() && _isConsole) singleConsole.Update(dt);
        if (!enableConsole && _isConsole) _isConsole = false;
    }

    [GameBoxWedge(PlacerType.AfterRender, 99)]
    public static void RenderConsole()
    {
        if (!AlertBase.AlertController.alertQueue.Any() && _isConsole) singleConsole.Render();
    }
}