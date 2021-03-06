using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using RayWrapper.Vars;
using ZimonIsHimUtils.ExtensionMethods;
using static Raylib_CsLo.Raylib;
using static RayWrapper.GameBox;
using static RayWrapper.GameConsole.CommandLineColor;
using static RayWrapper.GameConsole.CommandRegister;
using static RayWrapper.GameConsole.GameConsole;

namespace RayWrapper.GameConsole;

public class DefaultCommands : ICommandModule
{
    [Command("setfps"), Help("setfps [n > 0]\nsets fps to n")]
    public static string SetFps(string[] args)
    {
        if (args.Length < 1) return $"{CommandLineColor.SKYBLUE}To set your fps do setfps [number]";
        if (int.TryParse(args[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var fpsset) && fpsset > 0)
        {
            return $"{CommandLineColor.BLUE}Fps set to [{FPS = fpsset}]";
        }

        return $"{DARKRED}[{args[0]}] <- IS NOT A VALID NUMBER";
    }

    [Command("clear"), Help("clear (true/false)\nclears screen, true will not display message")]
    public static string Clear(string[] args)
    {
        var c = ClearOutput();
        if (args.Length <= 0 || args[0].ToLower()[0] is not 't' or '1')
            return $"{CommandLineColor.BLUE}Cleared {c} lines";
        return string.Empty;
    }

    [Command("opensavedir"), Help("opens save directory")]
    public static string OpenSaveDirectory(string[] args)
    {
        if (!SaveInit) return "Save system is not initialized";
        Process.Start("explorer.exe", $@"{GetSavePath}".Replace("/", "\\"));
        return $"Opening to [{GetSavePath}]";
    }

    [Command("resetres"), Help("resets resolution to base resolution")]
    public static string ResetResolution(string[] args)
    {
        if (IsWindowFullscreen()) return "Game can not be in fullscreen";
        SetWindowSize((int) WindowSize.X, (int) WindowSize.Y);
        return $"Reset Resolution to {WindowSize}";
    }

    [Command("setres"), Help("setres [width] [height]\nsets resolution to a specific screen size")]
    public static string SetResolution(string[] args)
    {
        if (args.Length < 2) return "To set your resolution do setres [width] [height]";
        if (!int.TryParse(args[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var w) ||
            !int.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var h))
        {
            return "Could not parse one of the variables";
        }

        SetWindowSize(w, h);
        return $"Set Resolution to <{w}, {h}>";
    }

    [Command("curres"), Help("displays current resolution")]
    public static string CurrentResolution(string[] args) =>
        $"The current window resolution is <{GetScreenWidth()}, {GetScreenHeight()}>";

    [Command("help"), Help("displays this message")]
    public static string[] Help(string[] args)
    {
        List<string> help = new();
        foreach (var key in Definitions.Keys.ToArray())
        {
            help.Add($"> {key}");
            help.AddRange(Definitions[key].Split('\n'));
        }

        help.Reverse();
        return help.Select(s => $"{CYAN}{s}").ToArray();
    }

    [Command("toggledebug"), Help("toggle debug tools")]
    public static string ToggleDebug(string[] args) => $"Toggled debug tools ({isDebugTool = !isDebugTool})";

    [Command("setscale"), Help("sets the window size based on scale")]
    public static string SetScale(string[] args)
    {
        if (args.Length < 1) return "To set your window size do setscale [scale]";
        if (!float.TryParse(args[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var f))
            return "Could not parse scale";
        SetWindowSize((int) (WindowSize.X * f), (int) (WindowSize.Y * f));
        return $"Set Resolution to a scale of <{f}>";
    }

    [Command("printstat"), Help("Creates a status file from the logger")]
    public static string PrintStatus(string[] args)
    {
        Logger.WriteLog(false);
        return $"{CYAN}Created status file at: {Logger.StatusSave}";
    }

    [Command("collectGarb"), Help("Calls garbage collection")]
    public static string CallGC(string[] args)
    {
        GC.Collect();
        return "Garbage Collected";
    }

    [Command("objs"), Help("Displays how many Gameobjects there are")]
    public static string Objects(string[] args) => $"{CYAN}There are [{gameObjects}] GameObjects";

    [Command("toggleFps"), Help("Toggles the fps counter visibility")]
    public static string ToggleFps(string[] args)
    {
        showFps.Flip();
        return $"{CYAN}{(showFps ? "Showing" : "Hiding")} Fps";
    }

    [Command("fpsPos"), Help("Sets Fps Position")]
    public static string FpsPos(string[] args)
    {
        if (args.Length < 2) return "to set the pos do fpspos [x] [y]";
        var x = int.TryParse(args[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var rX) ? rX : 0;
        var y = int.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var rY) ? rY : 0;
        fpsPos = new Vector2(x, y);
        return $"{CYAN}Set fps pos to ({x},{y})";
    }
}