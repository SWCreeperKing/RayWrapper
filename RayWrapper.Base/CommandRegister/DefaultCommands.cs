using System.Diagnostics;
using System.Globalization;
using RayWrapper.Base.GameBox;
using static Raylib_CsLo.Raylib;
using static RayWrapper.Base.GameBox.GameBox;
using static RayWrapper.Base.GameBox.Logger.Level;
using static RayWrapper.Base.GameObject.GameObject;
using static RayWrapper.Base.SaveSystem.SaveExt;
using static RayWrapper.Base.CommandRegister.CommandRegister;

namespace RayWrapper.Base.CommandRegister;

public class DefaultCommands : ICommandModule
{
    [Command("setfps"), Help("setfps [n > 0]\nsets fps to n")]
    public static (Logger.Level, string) SetFps(string[] args)
    {
        if (args.Length < 1) return (Special, "To set your fps do setfps [number]");
        if (int.TryParse(args[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var fpsset) && fpsset > 0)
        {
            return (Special, $"Fps set to [{Fps = fpsset}]");
        }

        return (SoftError, $"[{args[0]}] <- IS NOT A VALID NUMBER");
    }

    [Command("opensavedir"), Help("opens save directory")]
    public static (Logger.Level, string) OpenSaveDirectory(string[] args)
    {
        if (!SaveInit) return (Special, "Save system is not initialized");
        Process.Start("explorer.exe", $@"{GetSavePath}".Replace("/", "\\"));
        return (Special, $"Opening to [{GetSavePath}]");
    }

    [Command("resetres"), Help("resets resolution to base resolution")]
    public static (Logger.Level, string) ResetResolution(string[] args)
    {
        if (IsWindowFullscreen()) return (Warning, "Game can not be in fullscreen");
        SetWindowSize((int) WindowSize.X, (int) WindowSize.Y);
        return (Special, $"Reset Resolution to {WindowSize}");
    }

    [Command("setres"), Help("setres [width] [height]\nsets resolution to a specific screen size")]
    public static (Logger.Level, string) SetResolution(string[] args)
    {
        if (args.Length < 2) return (Special, "To set your resolution do setres [width] [height]");
        if (!int.TryParse(args[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var w) ||
            !int.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var h))
        {
            return (SoftError, "Could not parse one of the variables");
        }

        SetWindowSize(w, h);
        return (Special, $"Set Resolution to <{w}, {h}>");
    }

    [Command("curres"), Help("displays current resolution")]
    public static (Logger.Level, string) CurrentResolution(string[] args) =>
        (Special, $"The current window resolution is <{GetScreenWidth()}, {GetScreenHeight()}>");

    [Command("help"), Help("displays this message")]
    public static (Logger.Level, string)[] Help(string[] args)
    {
        List<string> help = new();
        foreach (var key in Definitions.Keys.ToArray())
        {
            help.Add($"> {key}");
            help.AddRange(Definitions[key].Split('\n'));
        }

        help.Reverse();
        return help.Select(s => (Special, $"{s}")).ToArray();
    }

    [Command("setscale"), Help("sets the window size based on scale")]
    public static (Logger.Level, string) SetScale(string[] args)
    {
        if (args.Length < 1) return (Special, "To set your window size do setscale [scale]");
        if (!float.TryParse(args[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var f))
            return (SoftError, "Could not parse scale");
        SetWindowSize((int) (WindowSize.X * f), (int) (WindowSize.Y * f));
        return (Special, $"Set Resolution to a scale of <{f}>");
    }

    [Command("printstat"), Help("Creates a status file from the logger")]
    public static (Logger.Level, string) PrintStatus(string[] args)
    {
        Logger.WriteLog(false);
        return (Special, $"Created status file at: {Logger.StatusSave}");
    }

    [Command("collectGarb"), Help("Calls garbage collection")]
    public static (Logger.Level, string) CallGC(string[] args)
    {
        GC.Collect();
        return (Special, "Garbage Collected");
    }

    [Command("objs"), Help("Displays how many Gameobjects there are")]
    public static (Logger.Level, string) Objects(string[] args) => (Special, $"There are [{gameObjects}] GameObjects");
}