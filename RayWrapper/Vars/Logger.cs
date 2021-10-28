using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Raylib_cs;
using static Raylib_cs.TraceLogLevel;
using static RayWrapper.Vars.Logger.Level;

namespace RayWrapper.Vars
{
    public static class Logger
    {
        public enum Level
        {
            Info,
            Debug,
            Warning,
            Error,
            Other
        }

        public const string regStr = @"(%(s|i|d|[\d.]+f))";

        public static readonly string guid = Guid.NewGuid().ToString();
        public static readonly string crashSave = $"{Directory.GetCurrentDirectory().Replace('\\', '/')}/CrashLogs";
        public static readonly string statusSave = $"{Directory.GetCurrentDirectory().Replace('\\', '/')}/CrashLogs";
        private static List<string> _log = new();
        private static bool hasError;

        private static string RayForm(string text, IntPtr rawArgs)
        {
            if (rawArgs == IntPtr.Zero) return text;
            var formatSize = Regex.Matches(text, regStr).Count;
            var arr = new IntPtr[formatSize];
            Marshal.Copy(rawArgs, arr, 0, formatSize);

            var formatString = text;
            var count = 0;
            while (Regex.IsMatch(formatString, regStr))
            {
                var what = Regex.Match(formatString, regStr).Groups[1].Value;
                var where = formatString.IndexOf(what, StringComparison.Ordinal);
                formatString = formatString.Remove(where, what.Length);
                switch (what)
                {
                    case "%s":
                        formatString = formatString.Insert(where, Marshal.PtrToStringAnsi(arr[count])!);
                        break;
                    case "%i":
                        formatString = formatString.Insert(where, arr[count]!.ToString());
                        break;
                    default:
                        if (!what.Contains('f') && !what.Contains('d')) break;
                        formatString = formatString.Insert(where, "[unknown float/double]");
                        break;
                }

                count++;
            }

            return formatString;
        }

        public static void RayLog(TraceLogLevel logLevel, string text, IntPtr args) =>
            Log(logLevel switch
            {
                LOG_ALL => Other,
                LOG_TRACE or LOG_DEBUG => Debug,
                LOG_INFO or LOG_NONE => Info,
                LOG_WARNING => Warning,
                LOG_ERROR or LOG_FATAL => Error,
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
            }, RayForm($"from raylib: {text}", args));

        public static void Log(Level level, string text)
        {
            if (level == Error) hasError = true;
            var time = $"{DateTime.Now:G}";
            Console.ForegroundColor = level switch
            {
                Info => ConsoleColor.DarkGreen,
                Debug => ConsoleColor.DarkCyan,
                Warning => ConsoleColor.Yellow,
                Error => ConsoleColor.Red,
                Other => ConsoleColor.Blue
            };
            Console.WriteLine($"[{time}]: [{text}]");
            Console.ForegroundColor = ConsoleColor.White;
            _log.Add($"[{level}] [{time}] [\"{text}\"]");
        }

        public static void WriteLog(bool isCrash = true)
        {
            hasError = false;
            var dir = isCrash ? "CrashLogs" : "StatusLogs";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var file = isCrash
                ? $"CrashLogs/Crash {DateTime.Now:u}.log".Replace(' ', '_').Replace(':', '-')
                : $"StatusLogs/Status {guid}.log";
            using var sw = File.CreateText(file);
            sw.Write(string.Join("\n", _log));
            sw.Close();
            Console.WriteLine(
                $"SAVED {(isCrash ? "CRASH" : "STATUS")} LOG AT: {Directory.GetCurrentDirectory().Replace('\\', '/')}/{file}");
        }

        public static void CheckWrite()
        {
            if (hasError) WriteLog();
        }
    }
}