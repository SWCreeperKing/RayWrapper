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
    /// <summary>
    /// The heart of the wrapper
    ///
    /// <example>
    /// This is how you should setup the beginning of a project
    /// <code>
    /// new GameBox(new Program(), new Vector2(window width, window height, "title"));
    /// 
    /// public class Program : GameLoop 
    /// {
    ///     public override void Init() 
    ///     {
    ///         // game object init
    ///         // if you have objects that just need to render and update
    ///         // then use RegisterGameObj()
    ///     }
    /// 
    ///     // tip: DO NOT init new objects everytime in Update/Render Loops
    ///     public override void UpdateLoop()
    ///     {
    ///         // put update loop stuff here
    ///     }
    /// 
    ///     public override void RenderLoop()
    ///     {
    ///         // put render loop stuff here
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public class GameBox
    {
        public static readonly string CoreDir =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static readonly Random Random = new();

        #region temp collision performance vars

        public static List<(string, string)> CollisionLayerTags { get; } = new();
        public static long[] CollisionTime { get; } = new long[100];
        public static double TimeAverage { get; private set; }
        public static long CollisionHigh { get; private set; }
        public static long CurrentCollision { get; private set; }
        public static int TimeKeeper { get; private set; }

        #endregion

        public static string DeveloperName { get; private set; }
        public static string AppName { get; private set; } = "Unknown App";
        public static string Title { get; private set; }
        public static bool SaveInit { get; private set; }
        public static Vector2 WindowSize { get; private set; }

        public static GameLoop scene;
        public static string discordAppId = string.Empty;
        public static float scale;
        public static long gameObjects = 0;
        public static bool IsMouseOccupied => mouseOccupier != null;
        public static bool conserveCpu;
        public static bool enableConsole = true;
        public static bool f11Fullscreen = true;
        public static bool isCollisionSystem;
        public static bool isDebugTool;
        public static bool showFps = false;
        public static TextureFilter targetTextureFilter = TextureFilter.TEXTURE_FILTER_POINT;
        public static IGameObject debugContext = null;
        public static IGameObject mouseOccupier;
        public static Vector2 fpsPos = Vector2.One;
        public static Vector2 mousePos;
        public static AlertBox alertBox = null;
        public static ScreenGrid screenGrid;
        public static ColorModule backgroundColor = new(40);
        public static ColorModule baseTooltipBackColor = new Color(0, 0, 0, 200);
        public static ColorModule baseTooltipColor = new Color(170, 170, 255, 220);
        public static ColorModule letterboxColor = new(20);
        public static List<SlotBase> dragCollision = new();
        public static List<string> tooltip = new();

        private static readonly List<ISave> SaveList = new();
        private static Task _collisionLoop;
        private static bool _hasInit;
        private static bool _initCollision;
        private static bool _initDiscord;
        private static bool _isConsole;
        private static bool _isDrawing;
        private static bool _isEnding;
        private static RenderTexture2D _target;
        private static List<Scheduler> _schedulers = new();
        private static List<Scheduler> _schedulerQueue = new();
        
        public static int FPS
        {
            get => GetFPS();
            set => SetTargetFPS(value);
        }

        public GameBox(GameLoop scene, Vector2 windowSize, string title = "Untitled Window", int fps = 60,
            string iconPath = "")
        {
            if (_hasInit) throw new ApplicationException("Only 1 instance of GameBox can be created");
            unsafe
            {
                SetTraceLogCallback(&Logger.RayLog);
            }

            _hasInit = true;
            SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
            (GameBox.scene, WindowSize) = (scene, windowSize);
            screenGrid = new ScreenGrid();
            InitWindow((int) WindowSize.X, (int) WindowSize.Y, Title = title);
            if (iconPath != string.Empty) SetWindowIcon(LoadImage(iconPath));
            _target = LoadRenderTexture((int) windowSize.X, (int) windowSize.Y);
            if (singleConsole is null)
            {
                singleConsole = new GameConsole.GameConsole();
                CommandRegister.RegisterCommand<DefaultCommands>();
            }

            SetTargetFPS(FPS = fps);
            SetWindowSize((int) windowSize.X, (int) windowSize.Y);
            Start();
        }

        /// <summary>
        /// Start Game loops
        /// </summary>
        private static void Start()
        {
            var schedulers = Task.Run(async () =>
            {
                while (!_isEnding)
                {
                    try
                    {
                        foreach (var scheduler in _schedulers) scheduler.TestTime(GetTimeMs());
                        _schedulers.AddRange(_schedulerQueue);
                        _schedulerQueue.RemoveAll(s => _schedulers.Contains(s));
                    }
                    catch (Exception e)
                    {
                        Logger.Log(Error, e.ToString());
                    }

                    if (conserveCpu) await Task.Delay(10);
                }
            });

            scene.Init();
            try
            {
                GC.Collect();
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

            _isEnding = true;
            Logger.Log("Waiting for schedulers to end");
            while (!schedulers.IsCompleted) Task.Delay(10).GetAwaiter().GetResult();
            if (_initCollision)
            {
                Logger.Log("Waiting for collision to end");
                while (!_collisionLoop.IsCompleted) Task.Delay(10).GetAwaiter().GetResult();
            }

            Logger.Log("All Tasks ended successfully");
            Dispose();
        }

        /// <summary>
        /// Initialize <see cref="DiscordIntegration"/>
        /// </summary>
        public static void InitDiscord()
        {
            if (_initDiscord) return;
            _initDiscord = true;
            DiscordIntegration.Init();
            if (discordAppId != string.Empty) DiscordIntegration.CheckDiscord(discordAppId);
            AddScheduler(new Scheduler(100, DiscordIntegration.UpdateActivity));
        }

        /// <summary>
        /// Initialize the Collision thread
        /// </summary>
        public static void InitCollision()
        {
            if (_initCollision) return;
            _initCollision = true;
            _collisionLoop = Task.Run(async () =>
            {
                while (!_isEnding)
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
                if (IsWindowFullscreen()) SetWindowSize((int) WindowSize.X, (int) WindowSize.Y);
                else SetWindowSize(GetMonitorWidth(mon), GetMonitorHeight(mon));
                ToggleFullscreen();
            }

            Animator.Update();

            if (IsKeyPressed(KeyboardKey.KEY_F3))
            {
                isDebugTool = !isDebugTool;
                WriteToConsole($"toggled debug via F3: {isDebugTool}");
            }

            scene.Update();
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
            scene.Render();
            if (_isConsole) singleConsole.Render();
            else
            {
                Animator.Render();
                alertBox?.Render();
            }

            if (isDebugTool)
            {
                tooltip.Add(
                    $"({mousePos.X},{mousePos.Y}){(IsMouseOccupied ? $"\nocc: {mouseOccupier}" : string.Empty)}{(debugContext is not null ? $"\nP: {debugContext.Position}\nS: {debugContext.Size}" : string.Empty)}");
            }

            if (tooltip.Any())
            {
                var text = string.Join("\n", tooltip);
                var quad = mousePos.X > WindowSize.X / 2 ? 1 : 2;
                if (mousePos.Y > WindowSize.Y / 2) quad += 2;
                var defFont = FontManager.GetDefFont();
                var textSize = defFont.MeasureText(text);
                Vector2 pos = new(mousePos.X - (quad % 2 != 0 ? textSize.X : 0),
                    mousePos.Y - (quad > 2 ? textSize.Y : -33));
                AssembleRectFromVec(pos, textSize).Grow(4).Draw(baseTooltipBackColor);
                defFont.DrawText(text, pos, baseTooltipColor);
                tooltip.Clear();
            }

            if (showFps || isDebugTool) DrawFPS((int) fpsPos.X, (int) fpsPos.Y);

            var texture = _target.texture;

            EndTextureMode();
            SetTextureFilter(texture, targetTextureFilter);
            BeginDrawing();
            ClearBackground(letterboxColor);
            DrawTexturePro(texture, new Rectangle(0, 0, texture.width, -texture.height),
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
            scene.Dispose();
            Logger.CheckWrite();
            Environment.Exit(0);
        }

        /// <summary>
        /// Adds a <see cref="Scheduler"/> to the scheduling list
        /// </summary>
        /// <param name="schedule">the <see cref="Scheduler"/> add</param>
        public static void AddScheduler(Scheduler schedule) => _schedulerQueue.Add(schedule);

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
            CurrentCollision = CollisionTime[TimeKeeper++] = ms;
            TimeKeeper %= CollisionTime.Length;
            CollisionHigh = Math.Max(CollisionHigh, ms);
            TimeAverage = CollisionTime.Sum() / (double) CollisionTime.Length;
        }

        public static void CalcMousePos()
        {
            var mouse = GetMousePosition();
            scale = Math.Min(GetScreenWidth() / WindowSize.X, GetScreenHeight() / WindowSize.Y);
            float Calc(float m, int s, float w) => (m - (s - w * scale) * 0.5f) / scale;
            mousePos.X = Calc(mouse.X, GetScreenWidth(), WindowSize.X);
            mousePos.Y = Calc(mouse.Y, GetScreenHeight(), WindowSize.Y);
        }

        /// <summary>
        /// open a web link with a given url
        /// </summary>
        /// <param name="url">url to open</param>
        /// <remarks>might not work on linux :p</remarks>
        public static void OpenLink(string url) => Process.Start("explorer.exe", url);

        /// <summary>
        /// yoink some json from the web
        /// </summary>
        /// <param name="site">website url</param>
        /// <param name="isSuccessful">if the yoink was successful</param>
        /// <returns>the yoinked data</returns>
        public static dynamic LoadJsonFromWeb(string site, out bool isSuccessful)
        {
            isSuccessful = true;
            try
            {
                return JsonConvert.DeserializeObject(new WebClient().DownloadString(site));
            }
            catch (Exception e)
            {
                Logger.Log(Warning, $"Cannot load json from web:\n{e.Message}\n{e.Source}");
            }

            isSuccessful = false;
            return null;
        }
    }
}