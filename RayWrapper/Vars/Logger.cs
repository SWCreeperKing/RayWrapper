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

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static unsafe void RayLog(int logLevel, sbyte* text, sbyte* args) =>
            Log(logLevel switch
            {
                (int)LOG_ALL => Other,
                (int)LOG_TRACE or (int)LOG_DEBUG => Debug,
                (int)LOG_INFO or (int)LOG_NONE => Info,
                (int)LOG_WARNING => Warning,
                (int)LOG_ERROR or (int)LOG_FATAL => Error,
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
            }, $"from raylib: {Logging.GetLogMessage(new IntPtr(text), new IntPtr(args))}");

        public static void Log(string text) => Log(Debug, text);
        public static void Log(object text) => Log(Debug, text.ToString());

        public static T LogReturn<T>(T t)
        {
            Log(t.ToString());
            return t;
        }

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

        public static void Log(Level level, object text) => Log(level, text.ToString());

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