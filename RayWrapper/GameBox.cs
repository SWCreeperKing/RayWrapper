using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Raylib_cs;
using RayWrapper.Animation;
using RayWrapper.CollisionSystem;
using RayWrapper.Discord;
using RayWrapper.GameConsole;
using RayWrapper.Objs;
using RayWrapper.Objs.Slot;
using RayWrapper.Vars;
using static Raylib_cs.Raylib;
using static RayWrapper.GameConsole.GameConsole;
using static RayWrapper.RectWrapper;
using static RayWrapper.Vars.Logger.Level;

namespace RayWrapper
{
    public class GameBox
    {
        public static readonly string CoreDir =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        #region temp collsion performance vars

        public static readonly Random Random = new();
        public static readonly List<(string, string)> CollisionLayerTags = new();
        public static readonly long[] CollisionTime = new long[100];
        public static double timeAverage;
        public static long collisionHigh;
        public static long currentCollision;
        public static int timeKeeper;

        #endregion

        public static string DeveloperName { get; private set; }
        public static string AppName { get; private set; } = "Unknown App";
        public static bool SaveInit { get; private set; }
        public static Vector2 WindowSize { get; private set; }
        public static GameLoop Scene { get; set; }

        public static string Title { get; private set; }

        // public static GameBox Gb => _instance;
        public static bool IsMouseOccupied => mouseOccupier != null;
        public static bool enableConsole = true;
        public static float scale;
        public static Vector2 mousePos;
        public static string discordAppId = "";

        public static AlertBox alertBox = null;

        public static ColorModule backgroundColor = new(40);
        public static ColorModule letterboxColor = new(20);
        public static bool isDebugTool;
        public static List<SlotBase> dragCollision = new();
        public static GameObject mouseOccupier;
        public static ScreenGrid screenGrid;
        public static TextureFilter fontTextureFilter = TextureFilter.TEXTURE_FILTER_BILINEAR;
        public static TextureFilter targetTextureFilter = TextureFilter.TEXTURE_FILTER_BILINEAR;
        public static bool f11Fullscreen = true;
        public static bool isCollisionSystem;
        public static GameObject debugContext = null;
        public static List<string> tooltip = new();
        public static ColorModule baseTooltipColor = new Color(170, 170, 255, 220);
        public static ColorModule baseTooltipBackColor = new Color(0, 0, 0, 200);

        private static readonly List<ISave> SaveList = new();
        private static Font _font;
        private static List<Scheduler> _schedulers = new();
        private static bool _isDrawing;
        private static bool _isConsole;

        private static RenderTexture2D _target;

        // private static GameBox _instance;
        private static bool _initDiscord;
        private static bool _initCollision;
        private static bool _hasInit;

        public static int FPS
        {
            get => GetFPS();
            set => SetTargetFPS(value);
        }

        public static Font Font
        {
            get => _font;
            set
            {
                _font = value;
                SetTextureFilter(_font.texture, fontTextureFilter);
            }
        }

        public GameBox(GameLoop scene, Vector2 windowSize, string title = "Untitled Window", int fps = 60,
            string iconPath = "")
        {
            if (_hasInit) throw new ApplicationException("Only 1 instance of GameBox can be created");
            SetTraceLogCallback(Logger.RayLog);
            _hasInit = true;
            SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
            (Scene, WindowSize) = (scene, windowSize);
            screenGrid = new ScreenGrid();
            InitWindow((int)WindowSize.X, (int)WindowSize.Y, Title = title);
            if (iconPath != "") SetWindowIcon(LoadImage(iconPath));
            _target = LoadRenderTexture((int)windowSize.X, (int)windowSize.Y);
            _font = GetFontDefault();
            if (singleConsole is null)
            {
                singleConsole = new GameConsole.GameConsole();
                CommandRegister.RegisterCommand<DefaultCommands>();
            }

            SetTargetFPS(FPS = fps);
            SetWindowSize((int)windowSize.X, (int)windowSize.Y);
            Start();
        }

        private static void Start()
        {
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
                        Logger.Log(Error, e.ToString());
                    }

                    Task.Delay(10).GetAwaiter().GetResult();
                }
            });

            Scene.Init();
            try
            {
                while (!WindowShouldClose())
                {
                    CalcMousePos();
                    if (IsKeyPressed(KeyboardKey.KEY_GRAVE) && enableConsole) _isConsole = !_isConsole;
                    else if (alertBox is null && !_isConsole) Update();
                    else if (alertBox is not null && !_isConsole) alertBox.Update();
                    else if (_isConsole) singleConsole.Update();
                    if (!enableConsole && _isConsole) _isConsole = false;
                    Render();
                }
            }
            catch (Exception e)
            {
                Logger.Log(Error, e.ToString());
            }

            Dispose();
        }

        public static void InitDiscord()
        {
            if (_initDiscord) return;
            _initDiscord = true;
            DiscordIntegration.Init();
            if (discordAppId != "") DiscordIntegration.CheckDiscord(discordAppId);
            AddScheduler(new Scheduler(100, DiscordIntegration.UpdateActivity));
        }

        public static void InitCollision()
        {
            if (_initCollision) return;
            _initCollision = true;
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var startTime = GetTimeMs();
                        await screenGrid.Update();
                        await Task.Run(() => AddTime(GetTimeMs() - startTime));
                    }
                    catch (Exception e)
                    {
                        Logger.Log(Error, e.ToString());
                    }
                }
            });
        }

        private static void Update()
        {
            if (f11Fullscreen && IsKeyPressed(KeyboardKey.KEY_F11))
            {
                var mon = GetCurrentMonitor();
                if (IsWindowFullscreen()) SetWindowSize((int)WindowSize.X, (int)WindowSize.Y);
                else SetWindowSize(GetMonitorWidth(mon), GetMonitorHeight(mon));
                ToggleFullscreen();
            }

            Animator.Update();

            if (IsKeyPressed(KeyboardKey.KEY_F3))
            {
                isDebugTool = !isDebugTool;
                singleConsole.WriteToConsole($"toggled debug via F3: {isDebugTool}");
            }

            Scene.Update();
        }

        public static void RenderRenderTexture(RenderTexture2D texture2D, Vector2 pos, Action update, Action draw)
        {
            if (!_isDrawing) return;
            var before = new Vector2(mousePos.X, mousePos.Y);
            mousePos.X -= pos.X;
            mousePos.Y -= pos.Y;
            update.Invoke();
            BeginTextureMode(texture2D);
            draw.Invoke();
            BeginTextureMode(_target);
            DrawTexturePro(texture2D.texture, new Rectangle(0, 0, texture2D.texture.width, -texture2D.texture.height),
                new Rectangle(pos.X, pos.Y, texture2D.texture.width, texture2D.texture.height),
                Vector2.Zero, 0, Color.WHITE);
            mousePos = before;
        }

        private static void Render()
        {
            BeginTextureMode(_target);
            _isDrawing = true;
            ClearBackground(backgroundColor);

            screenGrid.Draw(isDebugTool);
            Scene.Render();
            if (_isConsole) singleConsole.Render();
            else
            {
                Animator.Render();
                alertBox?.Render();
            }

            if (isDebugTool)
                tooltip.Add(
                    $"({mousePos.X},{mousePos.Y}){(IsMouseOccupied ? $"\nocc: {mouseOccupier}" : "")}{(debugContext is not null ? $"\nP: {debugContext.Position}\nS: {debugContext.Size}" : "")}");

            if (tooltip.Any())
            {
                var text = string.Join("\n", tooltip);
                var quad = mousePos.X > WindowSize.X / 2 ? 1 : 2;
                if (mousePos.Y > WindowSize.Y / 2) quad += 2;
                var textSize = Font.MeasureText(text);
                Vector2 pos = new(mousePos.X - (quad % 2 != 0 ? textSize.X : 0),
                    mousePos.Y - (quad > 2 ? textSize.Y : -33));
                AssembleRectFromVec(pos, textSize).Grow(4).Draw(baseTooltipBackColor);
                Font.DrawText(text, pos, baseTooltipColor);
                tooltip.Clear();
            }

            EndTextureMode();
            SetTextureFilter(_target.texture, targetTextureFilter);
            BeginDrawing();
            ClearBackground(letterboxColor);
            DrawTexturePro(_target.texture, new Rectangle(0, 0, _target.texture.width, -_target.texture.height),
                new Rectangle((GetScreenWidth() - WindowSize.X * scale) * .5f,
                    (GetScreenHeight() - WindowSize.Y * scale) * 0.5f, WindowSize.X * scale, WindowSize.Y * scale),
                Vector2.Zero, 0, Color.WHITE);
            EndDrawing();
            _isDrawing = false;
        }

        public static void Dispose()
        {
            if (_isDrawing) EndDrawing();
            _schedulers.Clear();
            CloseWindow();
            Scene.Dispose();
            Logger.CheckWrite();
            Environment.Exit(0);
        }

        public static void AddScheduler(Scheduler schedule) => _schedulers.Add(schedule);
        public static void ChangeFps(int fps) => SetTargetFPS(FPS = fps);
        public static long GetTimeMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        # region save system

        public static void InitSaveSystem(string developerName, string appName) =>
            (DeveloperName, AppName, SaveInit) = (developerName, appName, true);

        public static string GetSavePath =>
            File.Exists("UseLocalPath") ? $"{AppName}" : $"{CoreDir}/{DeveloperName}/{AppName}";

        public static void SaveItems() => SaveList.SaveItems();
        public static void LoadItems() => SaveList.LoadItems();
        public static void DeleteFile(string name) => SaveList.DeleteFile(name);
        public static void DeleteAll() => SaveList.DeleteAll();
        public static void RegisterSaveItem<T>(T obj, string fileName) => SaveList.Add(new SaveItem<T>(obj, fileName));
        public static void DeRegisterSaveItem(string fileName) => SaveList.RemoveAll(m => m.FileName() == fileName);

        #endregion

        public static void AddTime(long ms)
        {
            currentCollision = CollisionTime[timeKeeper++] = ms;
            timeKeeper %= CollisionTime.Length;
            collisionHigh = Math.Max(collisionHigh, ms);
            timeAverage = CollisionTime.Sum() / (double)CollisionTime.Length;
        }

        public static void CalcMousePos()
        {
            var mouse = GetMousePosition();
            scale = Math.Min(GetScreenWidth() / WindowSize.X, GetScreenHeight() / WindowSize.Y);
            float Calc(float m, int s, float w) => (m - (s - w * scale) * 0.5f) / scale;
            mousePos.X = Calc(mouse.X, GetScreenWidth(), WindowSize.X);
            mousePos.Y = Calc(mouse.Y, GetScreenHeight(), WindowSize.Y);
        }

        public static void LoadFont(string font, int fontSize = 96, int toChar = 255) =>
            Font = LoadFontEx(font, fontSize, null, toChar);

        public static void OpenLink(string url) => Process.Start("explorer.exe", url);

        public static dynamic LoadJsonFromWeb(string site, out bool isSuccessful)
        {
            isSuccessful = true;
            try
            {
                return JsonConvert.DeserializeObject(new WebClient().DownloadString(site));
            }
            catch
            {
                // ignored
            }

            isSuccessful = false;
            return null;
        }
    }
}