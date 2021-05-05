using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static Raylib_cs.Raylib;

namespace RayWrapper.Objs
{
    public class GameBox
    {
        public static readonly string CoreDir =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public static List<ISave> SaveList = new();
        public static string DeveloperName { get; private set; }
        public static string AppName { get; private set; }
        public static bool SaveInit { get; private set; }

        public static Font font = GetFontDefault();
        public static long frameTicker = 0;

        public GameLoop Scene { get; private set; }
        public Vector2 WindowSize { get; private set; }
        public string Title { get; private set; }
        public int FPS { get; private set; }

        public Color backgroundColor = new(32, 32, 32, 255);

        private List<Action> _onDispose = new();

        public event Action OnDispose
        {
            add => _onDispose.Add(value);
            remove => _onDispose.Remove(value);
        }

        public GameBox(GameLoop scene, Vector2 windowSize, string title = "Untitled Window", int fps = 60)
        {
            (Scene, WindowSize) = (scene, windowSize);
            InitWindow((int) WindowSize.X, (int) WindowSize.Y, Title = title);
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
            Scene.Update();
        }

        public void Render()
        {
            BeginDrawing();
            ClearBackground(backgroundColor);
            Scene.Render();
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
            if (!SaveInit) throw new Exception("GameBox.InitSaveSystem() Not called, Save System Not Initialized");
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
            if (!SaveInit) throw new Exception("GameBox.InitSaveSystem() Not called, Save System Not Initialized");
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
            if (!SaveInit) throw new Exception("GameBox.InitSaveSystem() Not called, Save System Not Initialized");
            var path = GetSavePath;
            if (!Directory.Exists(path)) return;
            var file = $"{path}/{iSave.FileName()}.RaySaveWrap";
            if (!File.Exists(file)) return;
            File.Delete(file);
        }

        public void DeleteAll()
        {
            
            if (!SaveInit) throw new Exception("GameBox.InitSaveSystem() Not called, Save System Not Initialized");
            var path = GetSavePath;
            if (!Directory.Exists(path)) return;
            SaveList.ForEach(t =>
            {
                var file = $"{path}/{t.FileName()}.RaySaveWrap";
                if (!File.Exists(file)) return;
                File.Delete(file);
            });
        }
    }
}