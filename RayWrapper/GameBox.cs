using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Raylib_cs;
using RayWrapper.Animation;
using RayWrapper.CollisionSystem;
using RayWrapper.Objs;
using RayWrapper.Vars;
using static Raylib_cs.Raylib;

namespace RayWrapper
{
    public class GameBox
    {
        //temp performance vars
        public static readonly Dictionary<string, Func<string, string[], string>> ConsoleCommands = new();
        public static readonly List<(string, string)> CollisionLayerTags = new();
        public static readonly long[] CollisionTime = new long[100];
        public static double TimeAverage;
        public static long CollisionHigh;
        public static long CurrentCollision;
        public static int TimeKeeper;

        public static readonly string CoreDir =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public static string DeveloperName { get; private set; }
        public static string AppName { get; private set; }
        public static bool SaveInit { get; private set; }
        public static Vector2 WindowSize { get; private set; }

        public static AlertBox alertBox = null;
        public static Animator animator = new();
        public static Font font;

        private static readonly List<ISave> _saveList = new();
        private static readonly List<Collider> _colliders = new();
        private static readonly List<Collider> _collidersAddQueue = new();
        private static readonly List<Collider> _collidersRemoveQueue = new();
        private static long _lastTime;

        // private static readonly List<(long, long)> CollisionMem = new();

        public GameLoop Scene { get; private set; }
        public string Title { get; private set; }
        public int FPS { get; private set; }

        public Color backgroundColor = new(40, 40, 40, 255);

        private InputBox _ib;
        private List<string> _consoleOut = new();
        private string[] _consoleWriteOut = new string[20];
        private List<Action> _onDispose = new();
        private List<Scheduler> _schedulers = new();
        private bool isDrawing;
        private bool _isConsole;

        public event Action OnDispose
        {
            add => _onDispose.Add(value);
            remove => _onDispose.Remove(value);
        }

        public GameBox(GameLoop scene, Vector2 windowSize, string title = "Untitled Window", int fps = 60)
        {
            (Scene, WindowSize) = (scene, windowSize);
            InitWindow((int) WindowSize.X, (int) WindowSize.Y, Title = title);
            font = GetFontDefault();
            SetTargetFPS(FPS = fps);
        }

        public void Start(bool initCollision)
        {
            Scene.Init();
            _lastTime = GetTimeMs();

            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        foreach (var scheduler in _schedulers) scheduler.TestTime(GetTimeMs());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"err: {e.Message}\n{e.StackTrace}");
                    }

                    Task.Delay(10).GetAwaiter().GetResult();
                }
            });

            if (initCollision)
            {
                Task.Run(() =>
                {
                    var groups = _colliders.GroupBy(c => c.layer).ToDictionary(c => c.Key, c => c);
                    List<(long, long)> collisionMem = new();
                    while (true)
                    {
                        try
                        {
                            var startTime = GetTimeMs();
                            if (_collidersAddQueue.Count > 0 || _collidersRemoveQueue.Count > 0)
                            {
                                if (_collidersAddQueue.Count > 0)
                                {
                                    _colliders.AddRange(_collidersAddQueue);
                                    _collidersAddQueue.Clear();
                                }

                                if (_collidersRemoveQueue.Count > 0)
                                {
                                    _colliders.RemoveAll(c => _collidersRemoveQueue.Contains(c));
                                    _collidersRemoveQueue.Clear();
                                }

                                groups = _colliders.GroupBy(c => c.layer).ToDictionary(c => c.Key, c => c);
                            }

                            var time = GetTimeMs();
                            var deltaTime = time - _lastTime;
                            _lastTime = time;

                            foreach (var collider in _colliders.Where(c => c.velocity != Vector2.Zero))
                                collider.MoveBy(collider.velocity * deltaTime);

                            if (groups.Count > 1)
                                foreach (var (s1, s2) in CollisionLayerTags)
                                foreach (var ci in groups[s1])
                                foreach (var cj in groups[s2])
                                {
                                    var key1 = (ci.currentId, cj.currentId);
                                    var key2 = (cj.currentId, ci.currentId);
                                    if (ci.CheckCollision(cj))
                                    {
                                        if (collisionMem.Contains(key1)) ci.InCollision(cj);
                                        else
                                        {
                                            ci.FirstCollision(cj);
                                            collisionMem.Add(key1);
                                        }

                                        if (collisionMem.Contains(key2)) cj.InCollision(ci);
                                        else
                                        {
                                            cj.FirstCollision(ci);
                                            collisionMem.Add(key2);
                                        }
                                    }
                                    else
                                    {
                                        if (collisionMem.Contains(key1))
                                        {
                                            ci.ExitCollision(cj);
                                            collisionMem.Remove(key1);
                                        }

                                        if (!collisionMem.Contains(key2)) continue;
                                        cj.ExitCollision(ci);
                                        collisionMem.Remove(key2);
                                    }
                                }

                            Task.Run(() => AddTime(GetTimeMs() - startTime));
                            Task.Delay(5).GetAwaiter().GetResult();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Err: {e.Message}\n{e.StackTrace}");
                        }
                    }
                });
            }

            _ib = new InputBox(new Vector2(0, 10), 100, 200);
            _ib.onEnter = s =>
            {
                if (s.Length < 1) return;
                var split = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 1) ExecuteCommand(s);
                else ExecuteCommand(split[0], split[1..]);
                _ib.Clear();
            };

            while (!WindowShouldClose())
            {
                if (alertBox is null) Update();
                else alertBox.Update();
                Render();
            }

            CloseWindow();
            foreach (var a in _onDispose) a.Invoke();
        }

        public void Update()
        {
            if (IsKeyPressed(KeyboardKey.KEY_GRAVE)) _isConsole = !_isConsole;
            else if (_isConsole)
            {
                _ib.Update();
                return;
            }

            animator.Update();
            Scene.Update();
        }

        public void Render()
        {
            BeginDrawing();
            isDrawing = true;
            try
            {
                ClearBackground(backgroundColor);
                Scene.Render();
                if (_isConsole)
                {
                    new Rectangle(0, 0, WindowSize.X, WindowSize.Y).Draw(new Color(0, 0, 0, 150));
                    _ib.Render();
                    font.DrawText(string.Join("\n", _consoleWriteOut), new Vector2(0, 50), Color.GREEN);
                }
                else
                {
                    animator.Render();
                    alertBox?.Render();
                }
            }
            catch (Exception e)
            {
                var before = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"RENDERING ERROR: {e.Message}");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(e.StackTrace);
                Console.ForegroundColor = before;
            }

            EndDrawing();
            isDrawing = false;
        }

        public void Dispose()
        {
            if (isDrawing) EndDrawing();
            _schedulers.Clear();
            CloseWindow();
            foreach (var a in _onDispose) a.Invoke();
            Environment.Exit(0);
        }

        public void AddScheduler(Scheduler schedule) => _schedulers.Add(schedule);
        public void ChangeFps(int fps) => SetTargetFPS(FPS = fps);
        public static long GetTimeMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        # region save system

        public string GetSavePath => $"{CoreDir}/{DeveloperName}/{AppName}";

        public void InitSaveSystem(string developerName, string appName)
        {
            DeveloperName = developerName;
            AppName = appName;
            SaveInit = true;
        }

        public void SaveItems()
        {
            WriteToConsole($"Saving Start @ {DateTime.Now:G}");
            var start = GetTimeMs();
            ISave.IsSaveInitCheck();
            var path = GetSavePath;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            foreach (var t in _saveList)
            {
                using var sw = File.CreateText($"{path}/{t.FileName()}.RaySaveWrap");
                sw.Write(t.SaveString());
                sw.Close();
            }

            WriteToConsole($"Saved in {new TimeVar(GetTimeMs() - start)}");
        }

        public void LoadItems()
        {
            WriteToConsole($"Loading Start @ {DateTime.Now:G}");
            var start = GetTimeMs();
            ISave.IsSaveInitCheck();
            var path = GetSavePath;
            if (!Directory.Exists(path)) return;
            foreach (var t in _saveList)
            {
                var file = $"{path}/{t.FileName()}.RaySaveWrap";
                if (!File.Exists(file)) continue;
                using var sr = new StreamReader(file);
                t.LoadString(sr.ReadToEnd());
                sr.Close();
            }

            WriteToConsole($"Loaded in {new TimeVar(GetTimeMs() - start)}");
        }

        public void DeleteFile(string name)
        {
            ISave.IsSaveInitCheck();
            var path = GetSavePath;
            if (!Directory.Exists(path)) return;
            var file = $"{path}/{_saveList.First(s => s.FileName() == name).FileName()}.RaySaveWrap";
            Console.WriteLine($"> DELETED {file} <");
            if (!File.Exists(file)) return;
            File.Delete(file);
        }

        public void DeleteAll()
        {
            ISave.IsSaveInitCheck();
            var path = GetSavePath;
            if (!Directory.Exists(path)) return;
            foreach (var file in _saveList.Select(t => $"{path}/{t.FileName()}.RaySaveWrap")
                .Where(file => File.Exists(file)))
                File.Delete(file);
        }

        public void RegisterSaveItem<T>(T obj, string fileName) where T : Setable<T> =>
            _saveList.Add(new SaveItem<T>(obj, fileName));

        public void DeRegisterSaveItem<T>(T obj, string fileName) where T : Setable<T> =>
            _saveList.RemoveAll(m => m.FileName() == fileName);

        public void ReRegisterSaveItem<T>(T ogObj, T newObj, string fileName) where T : Setable<T>
        {
            DeRegisterSaveItem(ogObj, fileName);
            RegisterSaveItem(newObj, fileName);
        }

        #endregion

        public static void RenderColliders()
        {
            try
            {
                foreach (var c in _colliders) c.Render();
            }
            catch
            {
                RenderColliders();
            }
        }

        public static void AddColliders(params Collider[] c) => _collidersAddQueue.AddRange(c);
        public static void RemoveColliders(params Collider[] c) => _collidersRemoveQueue.AddRange(c);

        public static void AddTime(long ms)
        {
            CurrentCollision = ms;
            CollisionTime[TimeKeeper++] = ms;
            TimeKeeper %= CollisionTime.Length;
            CollisionHigh = Math.Max(CollisionHigh, ms);
            TimeAverage = CollisionTime.Sum() / (double) CollisionTime.Length;
        }

        private void ExecuteCommand(string command, params string[] args)
        {
            switch (command.ToLower())
            {
                case "setfps":
                    if (args.Length < 1) WriteToConsole("To set your fps do setfps [number]");
                    else
                    {
                        if (int.TryParse(args[0], out var fpsset) && fpsset is > 0 and < 500)
                        {
                            SetTargetFPS(fpsset);
                            FPS = fpsset;
                            WriteToConsole($"Fps set to [{fpsset}]");
                        }
                        else WriteToConsole($"[{args[0]}] <- IS NOT A VALID NUMBER");
                    }

                    break;
                case "clear":
                    var len = _consoleOut.Count;
                    _consoleWriteOut = new string[20];
                    _consoleOut.Clear();
                    var doubleClear = args.Length > 0 && args[0].ToLower()[0] == 't';
                    if (!doubleClear) WriteToConsole($"Cleared {len} lines");
                    break;
                default:
                    WriteToConsole(ConsoleCommands.ContainsKey(command)
                        ? ConsoleCommands[command].Invoke(command, args)
                        : $"INVALID COMMAND: [{command}]");
                    break;
            }
        }

        public void WriteToConsole(string text)
        {
            _consoleOut.Add(text);
            UpdateConsoleOut();
        }

        public void UpdateConsoleOut()
        {
            for (int i = _consoleOut.Count - 1, j = 0; i >= 0 && j < 20; i--, j++)
                _consoleWriteOut[j] = _consoleOut[i];
        }
    }
}