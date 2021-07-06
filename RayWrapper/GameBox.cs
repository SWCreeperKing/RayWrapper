using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Raylib_cs;
using RayWrapper.Animation;
using RayWrapper.Objs;
using RayWrapper.Vars;
using static Raylib_cs.Raylib;

namespace RayWrapper
{
    public class GameBox
    {
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

        public GameLoop Scene { get; private set; }
        public string Title { get; private set; }
        public int FPS { get; private set; }

        public Color backgroundColor = new(40, 40, 40, 255);

        private List<Action> _onDispose = new();
        private List<Scheduler> _schedulers = new();
        private bool isDrawing;

        // private bool _isConsole;

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

        public void Start()
        {
            Scene.Init();

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

            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        animator.Update();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"err: {e.Message}\n{e.StackTrace}");
                    }

                    Task.Delay(1).GetAwaiter().GetResult();
                }
            });

            while (!WindowShouldClose())
            {
                if (alertBox is null) Update();
                else alertBox.Update();
                Render();
                // _schedulers.ForEach(scheduler => scheduler.TestTime(GetTimeMs()));
            }

            CloseWindow();
            _onDispose.ForEach(a => a.Invoke());
        }

        public void Update()
        {
            // if (IsKeyPressed(KeyboardKey.KEY_GRAVE)) MouseOccupied = _isConsole = !_isConsole;
            // if (_isConsole) return;
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
                // if (_isConsole) new Rectangle(0, 0, WindowSize.X, WindowSize.Y).Draw(new Color(0, 0, 0, 150));
                animator.Render();
                alertBox?.Render();
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
            _onDispose.ForEach(a => a.Invoke());
            Environment.Exit(0);
        }

        public void InitSaveSystem(string developerName, string appName)
        {
            DeveloperName = developerName;
            AppName = appName;
            SaveInit = true;
        }

        public string GetSavePath => $"{CoreDir}/{DeveloperName}/{AppName}";

        public void SaveItems()
        {
            ISave.IsSaveInitCheck();
            var path = GetSavePath;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            _saveList.ForEach(t =>
            {
                using var sw = File.CreateText($"{path}/{t.FileName()}.RaySaveWrap");
                sw.Write(t.SaveString());
                sw.Close();
            });
        }

        public void LoadItems()
        {
            ISave.IsSaveInitCheck();
            var path = GetSavePath;
            if (!Directory.Exists(path)) return;
            _saveList.ForEach(t =>
            {
                var file = $"{path}/{t.FileName()}.RaySaveWrap";
                if (!File.Exists(file)) return;
                using var sr = new StreamReader(file);
                t.LoadString(sr.ReadToEnd());
                sr.Close();
            });
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
            _saveList.ForEach(t =>
            {
                var file = $"{path}/{t.FileName()}.RaySaveWrap";
                if (!File.Exists(file)) return;
                File.Delete(file);
            });
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

        public void AddScheduler(Scheduler schedule)
        {
            _schedulers.Add(schedule);
        }

        public void ChangeFps(int fps) => SetTargetFPS(FPS = fps);

        public static long GetTimeMs() => DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}