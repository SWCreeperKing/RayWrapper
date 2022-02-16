using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using RayWrapper.Discord;
using RayWrapper.Vars;
using static Raylib_cs.Raylib;
using static RayWrapper.GameBox;
using static RayWrapper.GameConsole.CommandLineColor;
using static RayWrapper.GameConsole.CommandRegister;
using static RayWrapper.GameConsole.GameConsole;

namespace RayWrapper.GameConsole
{
    public class DefaultCommands : ICommandModule
    {
        [Command("setfps"), Help("setfps [0 < n < 501]\nsets fps to n")]
        public static string SetFps(string[] args)
        {
            if (args.Length < 1) return $"{SKYBLUE}To set your fps do setfps [number]";
            if (int.TryParse(args[0], out var fpsset) && fpsset is > 0 and <= 500)
                return $"{BLUE}Fps set to [{FPS = fpsset}]";
            return $"{DARKRED}[{args[0]}] <- IS NOT A VALID NUMBER";
        }

        [Command("clear"), Help("clear (true/false)\nclears screen, true will not display message")]
        public static string Clear(string[] args)
        {
            var c = singleConsole.ClearOutput();
            if (args.Length <= 0 || args[0].ToLower()[0] is not 't' or '1') return $"{BLUE}Cleared {c} lines";
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
            SetWindowSize((int)WindowSize.X, (int)WindowSize.Y);
            return $"Reset Resolution to {WindowSize}";
        }

        [Command("setres"), Help("setres [width] [height]\nsets resolution to a specific screen size")]
        public static string SetResolution(string[] args)
        {
            if (args.Length < 2) return "To set your resolution do setres [width] [height]";
            if (!int.TryParse(args[0], out var w) || !int.TryParse(args[1], out var h))
                return "Could not parse one of the variables";
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
            if (!float.TryParse(args[0], out var f)) return "Could not parse scale";
            SetWindowSize((int)(WindowSize.X * f), (int)(WindowSize.Y * f));
            return $"Set Resolution to a scale of <{f}>";
        }

        [Command("discord"), Help("discord rich presence status/recheck")]
        public static string DiscordCheck(string[] args)
        {
            if (discordAppId == string.Empty) return $"Discord Integration is not set up for [{AppName}]";
            if (DiscordIntegration.discordAlive) return $"[{AppName}] is Connected to discord!";
            singleConsole.WriteToConsole("Disconnected from discord, attempting to connect");
            DiscordIntegration.CheckDiscord(discordAppId);
            return DiscordIntegration.discordAlive ? "Reconnected!" : "Failed, Try again later!";
        }

        [Command("printstat"), Help("Creates a status file from the logger")]
        public static string PrintStatus(string[] args)
        {
            Logger.WriteLog(false);
            return $"{CYAN}Created status file at: {Logger.statusSave}";
        }

        [Command("collectGarb"), Help("Calls garbage collection")]
        public static string CallGC(string[] args)
        {
            GC.Collect();
            return "Garbage Collected";
        }

        [Command("objs"), Help("Displays how many Gameobjects there are")]
        public static string Objects(string[] args) => $"{CYAN}There are [{GameObjects}] GameObjects";

        [Command("toggleFps"), Help("Toggles the fps counter visibility")]
        public static string ToggleFps(string[] args)
        {
            showFps = !showFps;
            return $"{CYAN}{(showFps ? "Showing" : "Hiding")} Fps";
        }

        [Command("fpsPos"), Help("Sets Fps Position")]
        public static string FpsPos(string[] args)
        {
            if (args.Length < 2) return "to set the pos do fpspos [x] [y]";
            var x = int.TryParse(args[0], out var rX) ? rX : 0;
            var y = int.TryParse(args[1], out var rY) ? rY : 0;
            fpsPos = new Vector2(x, y);
            return $"{CYAN}Set fps pos to ({x},{y})";
        }
    }
}