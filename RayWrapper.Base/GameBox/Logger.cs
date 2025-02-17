﻿using System.Runtime.InteropServices;
using Raylib_CsLo;
using static Raylib_CsLo.TraceLogLevel;
using static RayWrapper.Base.GameBox.Logger.Level;
using static RayWrapper.Base.GameBox.Logger.LoggingLogger.Native;

namespace RayWrapper.Base.GameBox;

public static class Logger
{
    public enum Level
    {
        Info,
        Debug,
        Special,
        Warning,
        SoftError,
        Error,
        Other
    }

    public static readonly string CrashSave = $"{Directory.GetCurrentDirectory().Replace('\\', '/')}/CrashLogs";
    public static readonly string Guid = System.Guid.NewGuid().ToString();
    public static readonly string StatusSave = $"{Directory.GetCurrentDirectory().Replace('\\', '/')}/CrashLogs";

    public static bool showDebugLogs = true;

    private static List<Action<Level, string>> _listeners = new();
    private static IList<string> _log = new List<string>();
    private static bool _hasError;

    public static event Action<Level, string> LoggerListeners
    {
        add => _listeners.Add(value);
        remove => _listeners.Remove(value);
    }

    public static unsafe void Init() => Raylib.SetTraceLogCallback(&RayLog);

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    public static unsafe void RayLog(int logLevel, sbyte* text, sbyte* args)
    {
        Log(logLevel switch
        {
            (int) LOG_ALL => Other,
            (int) LOG_TRACE or (int) LOG_DEBUG => Debug,
            (int) LOG_INFO or (int) LOG_NONE => Info,
            (int) LOG_WARNING => Warning,
            (int) LOG_ERROR or (int) LOG_FATAL => Error,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        }, $"from raylib: {LoggingLogger.Logging.GetLogMessage(new nint(text), new nint(args))} ");
    }

    public static void Log(string text) => Log(Debug, text);
    public static void Log(object text) => Log(Debug, text.ToString());

    public static void Log(Exception e, string furtherInfo = "")
    {
        if (furtherInfo is not "") Log(Warning, $"Error Info: {furtherInfo}");
        Log(Error, $"{e.Message}\n{e.StackTrace}");
    }
    
    public static T LogReturn<T>(T t)
    {
        Log(t.ToString());
        return t;
    }

    public static void Log(Level level, string text)
    {
        if (!showDebugLogs && level is Debug) return;
        if (level == Error) _hasError = true;
        var time = $"{DateTime.Now:G}";

        Console.ForegroundColor = level switch
        {
            Info => ConsoleColor.DarkGreen,
            Debug => ConsoleColor.DarkCyan,
            Warning => ConsoleColor.Yellow,
            Error or SoftError => ConsoleColor.Red,
            Other => ConsoleColor.Blue,
            Special => ConsoleColor.Cyan,
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };
        Console.WriteLine($"[{time}]: [{text}]");
        Console.ForegroundColor = ConsoleColor.White;
        _log.Add($"[{level}] [{time}] [\"{text}\"]");
        _listeners.ForEach(a => a(level, text));
    }

    public static void Log(Level level, object text) => Log(level, text.ToString());
    
    public static void WriteLog(bool isCrash = true)
    {
        _hasError = false;
        var dir = isCrash ? "CrashLogs" : "StatusLogs";
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        var file = isCrash
            ? $"CrashLogs/Crash {DateTime.Now:u}.log".Replace(' ', '_').Replace(':', '-')
            : $"StatusLogs/Status {Guid}.log";
        using var sw = File.CreateText(file);
        sw.Write(string.Join("\n", _log));
        sw.Close();
        Console.WriteLine(
            $"SAVED {(isCrash ? "CRASH" : "STATUS")} LOG AT: {Directory.GetCurrentDirectory().Replace('\\', '/')}/{file}");
    }

    public static void CheckWrite()
    {
        if (_hasError) WriteLog();
    }

    /// <summary>
    /// Stolen from Raylib-cs since Raylib-cslo didn't have it
    /// </summary>
    internal static class LoggingLogger
    {
        internal readonly struct Native
        {
            private const string Msvcrt = "msvcrt";
            private const string Libc = "libc";
            private const string LibSystem = "libSystem";

            [DllImport(LibSystem, EntryPoint = "vasprintf", CallingConvention = CallingConvention.Cdecl)]
            public static extern int vasprintf_apple(ref nint buffer, nint format, nint args);

            [DllImport(Libc, EntryPoint = "vsprintf", CallingConvention = CallingConvention.Cdecl)]
            public static extern int vsprintf_linux(nint buffer, nint format, nint args);

            [DllImport(Msvcrt, EntryPoint = "vsprintf", CallingConvention = CallingConvention.Cdecl)]
            public static extern int vsprintf_windows(nint buffer, nint format, nint args);

            [DllImport(Libc, EntryPoint = "vsnprintf", CallingConvention = CallingConvention.Cdecl)]
            public static extern int vsnprintf_linux(nint buffer, nuint size, nint format, nint args);

            [DllImport(Msvcrt, EntryPoint = "vsnprintf", CallingConvention = CallingConvention.Cdecl)]
            public static extern int vsnprintf_windows(nint buffer, nuint size, nint format, nint args);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct VaListLinuxX64
        {
            uint gpOffset;
            uint fpOffset;
            nint overflowArgArea;
            nint regSaveArea;
        }

        /// <summary>
        /// Logging workaround for formatting strings from native code
        ///
        /// and maybe reformatted a bit :)
        /// </summary>
        public static unsafe class Logging
        {
            public static string GetLogMessage(nint format, nint args)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return AppleLogCallback(format, args);
                }

                // Special marshalling is needed on Linux desktop 64 bits.
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && nint.Size == 8)
                {
                    return LinuxX64LogCallback(format, args);
                }

                var byteLength = Vsnprintf(nint.Zero, nuint.Zero, format, args) + 1;
                if (byteLength <= 1)
                {
                    return string.Empty;
                }

                var buffer = Marshal.AllocHGlobal(byteLength);
                Vsprintf(buffer, format, args);

                var result = Marshal.PtrToStringUTF8(buffer);
                Marshal.FreeHGlobal(buffer);

                return result;
            }

            private static string AppleLogCallback(nint format, nint args)
            {
                var buffer = nint.Zero;
                try
                {
                    var count = vasprintf_apple(ref buffer, format, args);
                    if (count == -1)
                    {
                        return string.Empty;
                    }

                    return Marshal.PtrToStringUTF8(buffer) ?? string.Empty;
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }

            private static string LinuxX64LogCallback(nint format, nint args)
            {
                // The args pointer cannot be reused between two calls. We need to make a copy of the underlying structure.
                var listStructure = Marshal.PtrToStructure<VaListLinuxX64>(args);
                var listPointer = nint.Zero;
                int byteLength;
                string result;

                // Get length of args
                listPointer = Marshal.AllocHGlobal(Marshal.SizeOf(listStructure));
                Marshal.StructureToPtr(listStructure, listPointer, false);
                byteLength = vsnprintf_linux(nint.Zero, nuint.Zero, format, listPointer) + 1;

                // Allocate buffer for result
                Marshal.StructureToPtr(listStructure, listPointer, false);

                var utf8Buffer = nint.Zero;
                utf8Buffer = Marshal.AllocHGlobal(byteLength);

                // Print result into buffer
#pragma warning disable CA1806
                vsprintf_linux(utf8Buffer, format, listPointer);
#pragma warning restore CA1806
                result = Marshal.PtrToStringUTF8(utf8Buffer);

                Marshal.FreeHGlobal(listPointer);
                Marshal.FreeHGlobal(utf8Buffer);

                return result;
            }

            // https://github.com/dotnet/runtime/issues/51052
            static int Vsnprintf(nint buffer, nuint size, nint format, nint args)
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? vsnprintf_windows(buffer, size, format, args)
                    : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                        ? vsnprintf_linux(buffer, size, format, args)
                        : RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID"))
                            ? vsprintf_linux(buffer, format, args)
                            : -1;
            }

            // https://github.com/dotnet/runtime/issues/51052
            static int Vsprintf(nint buffer, nint format, nint args)
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? vsprintf_windows(buffer, format, args)
                    : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                        ? vsprintf_linux(buffer, format, args)
                        : RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID"))
                            ? vsprintf_linux(buffer, format, args)
                            : -1;
            }
        }
    }
}