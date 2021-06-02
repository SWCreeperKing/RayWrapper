using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Objs;
using RayWrapper.Vars;
using static Raylib_cs.Raylib;

namespace RayWrapper
{
    public class GameBox
    {
        public static readonly string CoreDir =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        // public static List<(object iRef, ISave s)> SaveList = new();
        private static List<ISave> SaveList = new();
        public static string DeveloperName { get; private set; }
        public static string AppName { get; private set; }
        public static bool SaveInit { get; private set; }

        public static Font font;
        public static long frameTicker = 0;
        public static Vector2 WindowSize { get; private set; }

        public GameLoop Scene { get; private set; }
        public string Title { get; private set; }
        public int FPS { get; private set; }

        public Color backgroundColor = new(32, 32, 32, 255);

        private List<Action> _onDispose = new();
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

        public void Start()
        {
            Scene.Init();
            while (!WindowShouldClose())
            {
                frameTicker++;
                frameTicker %= FPS;

                Update();
                Render();
            }

            CloseWindow();
            _onDispose.ForEach(a => a.Invoke());
        }

        public void Update()
        {
            if (IsKeyPressed(KeyboardKey.KEY_GRAVE)) _isConsole = !_isConsole;
            if (_isConsole) return;
            Scene.Update();
        }

        public void Render()
        {
            BeginDrawing();
            ClearBackground(backgroundColor);
            Scene.Render();
            if (_isConsole) new Rectangle(0, 0, WindowSize.X, WindowSize.Y).Draw(new Color(0, 0, 0, 150));
            EndDrawing();
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
            SaveList.ForEach(t =>
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
            SaveList.ForEach(t =>
            {
                var file = $"{path}/{t.FileName()}.RaySaveWrap";
                if (!File.Exists(file)) return;
                using var sr = new StreamReader(file);
                t.LoadString(sr.ReadToEnd());
                sr.Close();
            });
        }

        public void DeleteFile(ISave iSave)
        {
            ISave.IsSaveInitCheck();
            var path = GetSavePath;
            if (!Directory.Exists(path)) return;
            var file = $"{path}/{iSave.FileName()}.RaySaveWrap";
            if (!File.Exists(file)) return;
            File.Delete(file);
        }

        public void DeleteAll()
        {
            ISave.IsSaveInitCheck();
            var path = GetSavePath;
            if (!Directory.Exists(path)) return;
            SaveList.ForEach(t =>
            {
                var file = $"{path}/{t.FileName()}.RaySaveWrap";
                if (!File.Exists(file)) return;
                File.Delete(file);
            });
        }

        public void RegisterSaveItem<T>(T obj, string fileName) where T : ISetable =>
            SaveList.Add(new SaveItem<T>(obj, fileName));

        public void DeRegisterSaveItem<T>(T obj, string fileName) where T : ISetable =>
            SaveList.RemoveAll(m => m.FileName() == fileName);

        public void ReRegisterSaveItem<T>(T ogObj, T newObj, string fileName) where T : ISetable
        {
            DeRegisterSaveItem(ogObj, fileName);
            RegisterSaveItem(newObj, fileName);
        }
    }
}