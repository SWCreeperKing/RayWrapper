using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Raylib_cs;
using RayWrapper.Animation;
using RayWrapper.CollisionSystem;
using RayWrapper.GameConsole;
using RayWrapper.Objs;
using RayWrapper.Objs.Slot;
using RayWrapper.Vars;
using static Raylib_cs.Raylib;
using static RayWrapper.GameConsole.GameConsole;

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
        public static GameBox Gb => _instance;
        public static bool IsMouseOccupied => mouseOccupier != null;
        public static bool enableConsole = true;
        public static float scale;
        public static Vector2 mousePos;
        public static string discordAppId = "";
        public static AlertBox alertBox = null;
        public static Animator animator = new();
        public static ColorModule backgroundColor = new(40);
        public static ColorModule letterboxColor = new(20);
        public static bool isDebugTool;
        public static List<Slot> dragCollision = new();
        public static GameObject mouseOccupier;

        private static readonly List<ISave> SaveList = new();
        private static readonly List<Collider> Colliders = new();
        private static readonly List<Collider> CollidersAddQueue = new();
        private static readonly List<Collider> CollidersRemoveQueue = new();
        private static long _lastTime;
        private static Font _font;
        private static List<Action> _onDispose = new();
        private static List<Scheduler> _schedulers = new();
        private static bool _isDrawing;
        private static bool _isConsole;
        private static RenderTexture2D _target;
        private static GameBox _instance;

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
                SetTextureFilter(_font.texture, TextureFilter.TEXTURE_FILTER_TRILINEAR);
            }
        }

        public static event Action OnDispose
        {
            add => _onDispose.Add(value);
            remove => _onDispose.Remove(value);
        }

        public GameBox(GameLoop scene, Vector2 windowSize, string title = "Untitled Window", int fps = 60)
        {
            if (_instance is not null) throw new ApplicationException("Only 1 instance of GameBox can be created");
            _instance = this;
            DiscordIntegration.Init();
            SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
            (Scene, WindowSize) = (scene, windowSize);
            InitWindow((int)WindowSize.X, (int)WindowSize.Y, Title = title);
            _target = LoadRenderTexture((int)windowSize.X, (int)windowSize.Y);
            _font = GetFontDefault();
            if (singleConsole is null)
            {
                singleConsole = new GameConsole.GameConsole();
                CommandRegister.RegisterCommand<DefaultCommands>();
            }

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
                        DiscordIntegration.UpdateActivity();
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
                    var groups = Colliders.GroupBy(c => c.layer).ToDictionary(c => c.Key, c => c);
                    List<(long, long)> collisionMem = new();
                    while (true)
                    {
                        try
                        {
                            var startTime = GetTimeMs();
                            if (CollidersAddQueue.Count > 0 || CollidersRemoveQueue.Count > 0)
                            {
                                if (CollidersAddQueue.Count > 0)
                                {
                                    Colliders.AddRange(CollidersAddQueue);
                                    CollidersAddQueue.Clear();
                                }

                                if (CollidersRemoveQueue.Count > 0)
                                {
                                    Colliders.RemoveAll(c => CollidersRemoveQueue.Contains(c));
                                    CollidersRemoveQueue.Clear();
                                }

                                groups = Colliders.GroupBy(c => c.layer).ToDictionary(c => c.Key, c => c);
                            }

                            var time = GetTimeMs();
                            var deltaTime = time - _lastTime;
                            _lastTime = time;

                            foreach (var collider in Colliders.Where(c => c.velocity != Vector2.Zero))
                                collider.Position += collider.velocity * deltaTime;

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
                            Task.Delay(1).GetAwaiter().GetResult();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Err: {e.Message}\n{e.StackTrace}");
                        }
                    }
                });
            }

            if (discordAppId != "") DiscordIntegration.CheckDiscord(discordAppId);
            OnDispose += DiscordIntegration.Dispose;

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
                Console.WriteLine("AN ERROR HAS OCCURED");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ResetColor();
                Console.WriteLine("press any key to continue");
                Console.ReadKey();
                Dispose();
            }

            CloseWindow();
            foreach (var a in _onDispose) a.Invoke();
        }

        private void Update()
        {
            animator.Update();
            Scene.Update();
        }

        public void RenderRenderTexture(RenderTexture2D texture2D, Vector2 pos, Action update, Action draw)
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

        public void Render()
        {
            BeginTextureMode(_target);
            _isDrawing = true;
            try
            {
                ClearBackground(backgroundColor);

                Scene.Render();
                if (_isConsole) singleConsole.Render();
                else
                {
                    animator.Render();
                    alertBox?.Render();
                }

                if (isDebugTool)
                    RectWrapper.AssembleRectFromVec(Vector2.Zero, WindowSize)
                        .DrawTooltip($"({mousePos.X},{mousePos.Y}){(IsMouseOccupied ? $"\nocc: {mouseOccupier}" : "")}");
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

            EndTextureMode();

            BeginDrawing();
            ClearBackground(letterboxColor);
            DrawTexturePro(_target.texture, new Rectangle(0, 0, _target.texture.width, -_target.texture.height),
                new Rectangle((GetScreenWidth() - WindowSize.X * scale) * .5f,
                    (GetScreenHeight() - WindowSize.Y * scale) * 0.5f, WindowSize.X * scale, WindowSize.Y * scale),
                Vector2.Zero, 0, Color.WHITE);
            EndDrawing();
            _isDrawing = false;
        }

        public void Dispose()
        {
            if (_isDrawing) EndDrawing();
            _schedulers.Clear();
            CloseWindow();
            foreach (var a in _onDispose) a.Invoke();
            Environment.Exit(0);
        }

        public void AddScheduler(Scheduler schedule) => _schedulers.Add(schedule);
        public void ChangeFps(int fps) => SetTargetFPS(FPS = fps);
        public static long GetTimeMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        # region save system

        public void InitSaveSystem(string developerName, string appName) =>
            (DeveloperName, AppName, SaveInit) = (developerName, appName, true);

        public static string GetSavePath => $"{CoreDir}/{DeveloperName}/{AppName}";
        public void SaveItems() => SaveList.SaveItems(this);
        public void LoadItems() => SaveList.LoadItems(this);
        public void DeleteFile(string name) => SaveList.DeleteFile(name, this);
        public void DeleteAll() => SaveList.DeleteAll(this);
        public void RegisterSaveItem<T>(T obj, string fileName) => SaveList.Add(new SaveItem<T>(obj, fileName));
        public void DeRegisterSaveItem(string fileName) => SaveList.RemoveAll(m => m.FileName() == fileName);

        #endregion

        public static void RenderColliders()
        {
            try
            {
                foreach (var c in Colliders) c.Render();
            }
            catch
            {
                RenderColliders();
            }
        }

        public static void AddColliders(params Collider[] c) => CollidersAddQueue.AddRange(c);
        public static void RemoveColliders(params Collider[] c) => CollidersRemoveQueue.AddRange(c);

        public static void AddTime(long ms)
        {
            currentCollision = CollisionTime[timeKeeper++] = ms;
            timeKeeper %= CollisionTime.Length;
            collisionHigh = Math.Max(collisionHigh, ms);
            timeAverage = CollisionTime.Sum() / (double)CollisionTime.Length;
        }

        public void CalcMousePos()
        {
            var mouse = GetMousePosition();
            scale = Math.Min(GetScreenWidth() / WindowSize.X, GetScreenHeight() / WindowSize.Y);
            float Calc(float m, int s, float w) => (m - (s - w * scale) * 0.5f) / scale;
            mousePos.X = Calc(mouse.X, GetScreenWidth(), WindowSize.X);
            mousePos.Y = Calc(mouse.Y, GetScreenHeight(), WindowSize.Y);
        }

        public static void OpenLink(string url) => Process.Start("explorer.exe", url);
    }
}