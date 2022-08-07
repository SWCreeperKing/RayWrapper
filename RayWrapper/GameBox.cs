using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Raylib_CsLo;
using RayWrapper.Animation;
using RayWrapper.GameConsole;
using RayWrapper.Objs;
using RayWrapper.Objs.Slot;
using RayWrapper.Var_Interfaces;
using RayWrapper.Vars;
using ZimonIsHimUtils.ExtensionMethods;
using static Raylib_CsLo.MouseCursor;
using static Raylib_CsLo.Raylib;
using static RayWrapper.GameConsole.GameConsole;
using static RayWrapper.RectWrapper;
using static RayWrapper.Vars.Logger.Level;

namespace RayWrapper;

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

    public static string DeveloperName { get; private set; }
    public static string AppName { get; private set; } = "Unknown App";
    public static string Title { get; private set; }
    public static long DeltaTime => thisTick - lastTick;
    public static bool SaveInit { get; private set; }
    public static Vector2 WindowSize { get; private set; }

    public static float scale;
    public static long gameObjects = 0;
    public static long lastTick;
    public static long thisTick;
    public static bool IsMouseOccupied => mouseOccupier != null;
    public static bool conserveCpu;
    public static bool enableConsole = true;
    public static bool f11Fullscreen = true;
    public static bool isDebugTool;
    public static bool showFps = false;
    public static TextureFilter targetTextureFilter = TextureFilter.TEXTURE_FILTER_POINT;
    public static GameObject debugContext = null;
    public static IGameObject mouseOccupier;
    public static Vector2 fpsPos = Vector2.One;
    public static Vector2 mousePos;
    public static Stack<AlertBase> alertQueue = new();
    public static ColorModule backgroundColor = new(40);
    public static ColorModule baseTooltipBackColor = new Color(0, 0, 0, 200);
    public static ColorModule baseTooltipColor = new Color(170, 170, 255, 220);
    public static ColorModule letterboxColor = new(20);
    public static List<SlotBase> dragCollision = new();
    public static int tooltipLayers = 1;
    public static List<Tooltip> tooltips = new();

    private static readonly List<ISave> SaveList = new();
    private static bool _hasInit;
    private static bool _initCollision;
    private static bool _isConsole;
    private static bool _isDrawing;
    private static bool _isEnding;
    private static string _currentScene = "main";
    private static MouseCursor _currentMouse;
    private static Task _collisionLoop;
    private static RenderTexture _target;
    private static List<Scheduler> _schedulers = new();
    private static List<Scheduler> _schedulerQueue = new();
    private static DefaultTooltip _debugTooltip;
    private static List<MouseCursor> _mouseCursors = new();
    private static List<Action> _staticInit = new();
    private static List<Action> _staticUpdate = new();
    private static List<Action> _staticRender = new();
    private static List<Action> _staticDispose = new();

    public static event Action StaticInit
    {
        add => _staticInit.Add(value);
        remove => _staticInit.Remove(value);
    }

    public static event Action StaticUpdate
    {
        add => _staticUpdate.Add(value);
        remove => _staticUpdate.Remove(value);
    }

    public static event Action StaticRender
    {
        add => _staticRender.Add(value);
        remove => _staticRender.Remove(value);
    }

    public static event Action StaticDispose
    {
        add => _staticDispose.Add(value);
        remove => _staticDispose.Remove(value);
    }

    public static int FPS
    {
        get => GetFPS();
        set => SetTargetFPS(value);
    }

    public GameBox(GameLoop loop, Vector2 windowSize, string title = "Untitled Window", int fps = 60,
        string iconPath = "", bool resizable = true)
    {
        if (_hasInit) throw new ApplicationException("Only 1 instance of GameBox can be created");
        unsafe
        {
            SetTraceLogCallback(&Logger.RayLog);
        }

        _hasInit = true;
        if (resizable) SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
        WindowSize = windowSize;
        SceneManager.AddScene("main", loop);
        InitWindow((int) WindowSize.X, (int) WindowSize.Y, Title = title);
        RayGui.GuiLoadStyleDefault();
        if (iconPath != string.Empty) SetWindowIcon(LoadImage(iconPath));
        _target = LoadRenderTexture((int) windowSize.X, (int) windowSize.Y);
        if (singleConsole is null)
        {
            singleConsole = new GameConsole.GameConsole();
            CommandRegister.RegisterCommand<DefaultCommands>();
        }

        _debugTooltip = new DefaultTooltip(new Actionable<string>(() => $@"({mousePos.X},{mousePos.Y}){
            (IsMouseOccupied ? $"\nocc: {mouseOccupier}" : string.Empty)
        }{
            (debugContext is not null ? $"\nP: {debugContext.Position}\nS: {debugContext.Size}" : string.Empty)
        }{(debugContext?.debugString is not null ? $"{debugContext.debugString}" : "")}"));

        SetTargetFPS(FPS = fps);
        SetWindowSize((int) windowSize.X, (int) windowSize.Y);

        Text.Style.SetDefaultFont(LoadDefaultFont());

        Start();
    }

    public static void SwitchScene(string id)
    {
        SceneManager.CheckScene(id);
        if (!SceneManager.HasInit[id]) SceneManager.InitScene(id);
        _currentScene = id;
    }

    public unsafe static Font LoadDefaultFont(int fontSize = 32, int toCodePoint = 1000)
    {
        try
        {
            var asm = Assembly.GetExecutingAssembly();
            var stream = asm.GetManifestResourceStream("RayWrapper.Resources.AddedAssets.Font.CascadiaMono.ttf");

            var byteArr = new byte[stream!.Length];
            stream.Read(byteArr, 0, (int) stream.Length);
            stream.Close();

            fixed (byte* bytes = byteArr)
            {
                return LoadFontFromMemory(".ttf", bytes, byteArr.Length, fontSize, null, toCodePoint);
            }
        }
        catch (Exception e)
        {
            Logger.Log(Warning, $"Could not load Default Font: CascadiaCodeMono.ttf: {e.Message}");
        }

        return GetFontDefault();
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

        _staticInit.ForEach(a => a.Invoke());
        SceneManager.InitScene("main");
        try
        {
            lastTick = GetTimeMs();
            GC.Collect();
            while (!WindowShouldClose())
            {
                thisTick = GetTimeMs();
                CalcMousePos();
                if (IsKeyPressed(KeyboardKey.KEY_GRAVE) && enableConsole) _isConsole = !_isConsole;
                else
                    switch (alertQueue.Count)
                    {
                        case < 1 when !_isConsole:
                            Update();
                            break;
                        case > 0 when !_isConsole:
                            alertQueue.Peek().Update();
                            break;
                        default:
                        {
                            if (_isConsole) singleConsole.Update();
                            break;
                        }
                    }

                if (!enableConsole && _isConsole) _isConsole = false;

                Render();

                if (mouseOccupier is not null)
                {
                    SetMouseCursorRay(mouseOccupier.GetOccupiedCursor());
                }
                else if (_mouseCursors.Any())
                {
                    SetMouseCursorRay(_mouseCursors[^1]);
                    _mouseCursors.Clear();
                }
                else if (_currentMouse != MOUSE_CURSOR_DEFAULT) SetMouseCursorRay(MOUSE_CURSOR_DEFAULT);

                lastTick = thisTick;
            }
        }
        catch (Exception e)
        {
            Logger.Log(Error, e.ToString());
        }

        _isEnding = true;
        Logger.Log(Other, "Waiting for schedulers to end");
        while (!schedulers.IsCompleted) Task.Delay(10).GetAwaiter().GetResult();
        if (_initCollision)
        {
            Logger.Log(Special, "Waiting for collision to end");
            while (!_collisionLoop.IsCompleted) Task.Delay(10).GetAwaiter().GetResult();
        }

        Logger.Log(Special, "All Tasks ended successfully");
        Dispose();
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

        _staticUpdate.ForEach(a => a.Invoke());
        SceneManager.UpdateScene(_currentScene);
    }

    public static void RenderRenderTexture(RenderTexture Texture, Vector2 pos, Action update, Action draw)
    {
        if (!_isDrawing) return;
        var before = new Vector2(mousePos.X, mousePos.Y);
        mousePos.X -= pos.X;
        mousePos.Y -= pos.Y;
        update.Invoke();
        BeginTextureMode(Texture);
        draw.Invoke();
        BeginTextureMode(_target);
        DrawTexturePro(Texture.texture, new Rectangle(0, 0, Texture.texture.width, -Texture.texture.height),
            new Rectangle(pos.X, pos.Y, Texture.texture.width, Texture.texture.height),
            Vector2.Zero, 0, WHITE);
        mousePos = before;
    }

    private static void Render()
    {
        BeginTextureMode(_target);
        _isDrawing = true;
        ClearBackground(backgroundColor);

        _staticRender.ForEach(a => a.Invoke());
        SceneManager.RenderScene(_currentScene);

        if (_isConsole) singleConsole.Render();
        else
        {
            Animator.Render();
            if (alertQueue.Count > 0)
            {
                new Rectangle(0, 0, WindowSize.X, WindowSize.Y).Draw(new Color(0, 0, 0, 75));
                alertQueue.Peek().Render();
            }
        }

        if (isDebugTool) _debugTooltip.Draw();

        if (tooltips.Any())
        {
            var quad = mousePos.X > WindowSize.X / 2 ? 1 : 2;
            if (mousePos.Y > WindowSize.Y / 2) quad += 2;

            foreach (var tt in tooltips.GetRange(tooltips.Count - tooltipLayers, tooltipLayers))
            {
                tt.RenderTooltip((Tooltip.ScreenQuadrant) quad);
            }

            tooltips.Clear();
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
            Vector2.Zero, 0, WHITE);
        EndDrawing();
        _isDrawing = false;
    }

    public static void Dispose()
    {
        if (_isDrawing) EndDrawing();
        _staticDispose.ForEach(a => a.Invoke());
        SceneManager.DisposeAllScenes();
        CloseWindow();
        _schedulers.Clear();
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

    /// <summary>
    /// Toggles the ingame console
    /// </summary>
    public static void ToggleConsole() => _isConsole.Flip();

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

    public static void CalcMousePos()
    {
        var mouse = GetMousePosition();
        scale = Math.Min(GetScreenWidth() / WindowSize.X, GetScreenHeight() / WindowSize.Y);
        float Calc(float m, int s, float w) => (m - (s - w * scale) * 0.5f) / scale;
        mousePos.X = Calc(mouse.X, GetScreenWidth(), WindowSize.X);
        mousePos.Y = Calc(mouse.Y, GetScreenHeight(), WindowSize.Y);
    }

    public static void SetMouseCursor(MouseCursor cursor) => _mouseCursors.Add(cursor);

    private static void SetMouseCursorRay(MouseCursor cursor)
    {
        Raylib.SetMouseCursor(cursor);
        _currentMouse = cursor;
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
    /// <returns>the yoinked data</returns>
    public static async Task<dynamic> LoadJsonFromWeb(string site)
    {
        try
        {
            var client = new HttpClient();
            using var response = await client.GetAsync(site);
            using var content = response.Content;
            return JsonConvert.DeserializeObject(await content.ReadAsStringAsync());
        }
        catch (Exception e)
        {
            Logger.Log(Warning, $"Cannot load json from web:\n{e.Message}\n{e.Source}");
        }

        return null;
    }

    public class DefaultTooltip : Tooltip
    {
        public DefaultTooltip(Actionable<string> data) : base(data)
        {
        }

        protected override void RenderTooltip(ScreenQuadrant screenQuad, string data)
        {
            var text = string.Join("\n", data);
            var defFont = Text.Style.DefaultFont ?? GetFontDefault();
            var textSize = defFont.MeasureText(text);
            Vector2 pos = new(mousePos.X - ((int) screenQuad % 2 != 0 ? textSize.X : 0),
                mousePos.Y - ((int) screenQuad > 2 ? textSize.Y : -33));
            var rect = AssembleRectFromVec(pos, textSize).Grow(4);
            rect.Draw(baseTooltipBackColor);
            rect.DrawHallowRect(((Color) baseTooltipColor).MakeDarker());
            defFont.DrawText(text, pos, baseTooltipColor);
        }
    }
}