using System.Numerics;
using System.Reflection;
using Raylib_CsLo;
using RayWrapper.Base.CommandRegister;
using RayWrapper.Base.GameObject;
using RayWrapper.Base.Primitives;
using RayWrapper.Base.SaveSystem;
using static System.Environment;
using static Raylib_CsLo.MouseCursor;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.TextureFilter;
using static RayWrapper.Base.GameBox.AttributeManager.PlacerType;
using static RayWrapper.Base.GameBox.Logger.Level;
using static RayWrapper.Base.SaveSystem.SaveExt;
using Rectangle = Raylib_CsLo.Rectangle;

namespace RayWrapper.Base.GameBox;

/// <summary>
/// The heart of the wrapper
///
/// <example>
/// This is how you should setup the beginning of a project
/// <code>
/// new GameBox(new Program(), new Vector2(window width, window height, "title"));
/// 
/// public partial class Program : GameLoop 
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
    public static readonly string CoreDir = GetFolderPath(SpecialFolder.LocalApplicationData);
    public static readonly Random Random = new();

    public static Vector2 WindowSize
    {
        get => _windowSize;
        set
        {
            Input.maxWindowSize = _windowSize = value;
            SetWindowSize((int) _windowSize.X, (int) _windowSize.Y);
        }
    }

    public static int Fps
    {
        get => _maxFps;
        set => SetTargetFPS(_maxFps = value);
    }

    public static string Title
    {
        get => _windowTitle;
        private set => SetWindowTitle(_windowTitle = value);
    }

    public static bool pauseMainUpdateLoop;
    public static float scale;
    public static Font defaultFont;
    public static ColorModule backgroundColor = new(40);
    public static ColorModule letterboxColor = new(20);
    public static TextureFilter targetTextureFilter = TEXTURE_FILTER_POINT;

    private static bool _running = true;
    private static bool _hasInit;
    private static bool _isDrawing;
    private static int _maxFps;
    private static long _lastTick;
    private static string _windowTitle;
    private static string _currentScene = "main";
    private static Vector2 _windowSize;
    private static RenderTexture _target;
    private static List<MouseCursor> _mouseCursors = new();
    private static MouseCursor _currentMouse;

    public GameBox(GameLoop loop, Vector2 windowSize, string title = "Untitled Window", int fps = 60,
        string iconPath = "", bool resizable = true)
    {
        if (_hasInit) throw new ApplicationException("Only 1 instance of GameBox can be created");
        if (resizable) SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);

        SceneManager.AddScene("main", loop);
        WindowSize = windowSize;

        Init();

        Fps = fps;
        Title = title;
        defaultFont = LoadDefaultFont();
        if (iconPath != string.Empty) SetWindowIcon(LoadImage(iconPath));

        Start();
    }

    private void Init()
    {
        if (_hasInit) throw new ApplicationException("Only 1 instance of GameBox can be created");

        _hasInit = true;
        Logger.Init();
        AttributeManager.Init();
        Input.Init(WindowSize);
        InitWindow((int) WindowSize.X, (int) WindowSize.Y, "Loading...");
        Input.processPositions = v2 =>
        {
            float Calc(float m, int s, float w) => (m - (s - w * scale) * 0.5f) / scale;
            return new Vector2(Calc(v2.X, GetScreenWidth(), WindowSize.X), Calc(v2.Y, GetScreenHeight(), WindowSize.Y));
        };
        AttributeManager.ExtraRunner(BeforeInit);
        SceneManager.InitScene("main");
        AttributeManager.ExtraRunner(AfterInit);
        CommandRegister.CommandRegister.RegisterCommand<DefaultCommands>();

        _target = LoadRenderTexture((int) WindowSize.X, (int) WindowSize.Y);
    }

    private void Start()
    {
        try
        {
            _lastTick = GetTimeMs();

            while (!WindowShouldClose() && _running)
            {
                var currentTick = GetTimeMs();
                scale = Math.Min(GetScreenWidth() / WindowSize.X, GetScreenHeight() / WindowSize.Y);
                pauseMainUpdateLoop = false;
                Input.Update();

                Update((currentTick - _lastTick) / 1000f);
                Render();

                if (Input.MouseOccupier is not null && Input.MouseOccupier is IGameObject io)
                {
                    SetMouseCursorRay(io.GetOccupiedCursor());
                }
                else if (_mouseCursors.Any())
                {
                    SetMouseCursorRay(_mouseCursors[^1]);
                    _mouseCursors.Clear();
                }
                else if (_currentMouse != MOUSE_CURSOR_DEFAULT) SetMouseCursorRay(MOUSE_CURSOR_DEFAULT);

                _lastTick = currentTick;
            }
        }
        catch (Exception e)
        {
            Logger.Log(Error, e.ToString());
        }

        AttributeManager.ExtraRunner(BeforeDispose);
        Dispose();
    }

    private void Update(float dt)
    {
        AttributeManager.ExtraRunner(BeforeUpdate, dt);
        if (!pauseMainUpdateLoop) SceneManager.UpdateScene(_currentScene, dt);
        AttributeManager.ExtraRunner(AfterUpdate, dt);
    }

    private void Render()
    {
        BeginTextureMode(_target);
        _isDrawing = true;
        ClearBackground(backgroundColor);

        AttributeManager.ExtraRunner(BeforeRender);
        SceneManager.RenderScene(_currentScene);
        AttributeManager.ExtraRunner(AfterRender);
        
        var texture = _target.texture;
        EndTextureMode();
        SetTextureFilter(texture, targetTextureFilter);

        BeginDrawing();
        ClearBackground(letterboxColor);
        DrawTexturePro(texture, new Rectangle(0, 0, texture.width, -texture.height), new Rectangle(
            (GetScreenWidth() - WindowSize.X * scale) * .5f, (GetScreenHeight() - WindowSize.Y * scale) * 0.5f,
            WindowSize.X * scale, WindowSize.Y * scale), Vector2.Zero, 0, WHITE);
        EndDrawing();
        _isDrawing = false;
    }

    private void Dispose()
    {
        if (_isDrawing) EndDrawing();
        SceneManager.DisposeAllScenes();
        CloseWindow();
        AttributeManager.ExtraRunner(AfterDispose);
        Logger.CheckWrite();
        Exit(0);
    }

    public static void SetMouseCursor(MouseCursor cursor) => _mouseCursors.Add(cursor);

    private static void SetMouseCursorRay(MouseCursor cursor)
    {
        SetMouseCursor(cursor);
        _currentMouse = cursor;
    }

    public static unsafe Font LoadDefaultFont(int fontSize = 32, int toCodePoint = 1000)
    {
        try
        {
            var asm = Assembly.GetExecutingAssembly();
            var stream = asm.GetManifestResourceStream("RayWrapper.Base.Resources.AddedAssets.Font.CascadiaMono.ttf");

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

    public static void SwitchScene(string id)
    {
        SceneManager.CheckScene(id);
        if (!SceneManager.HasInit[id]) SceneManager.InitScene(id);
        _currentScene = id;
    }

    public static long GetTimeMs() => TimeVar.GetTimeMs();
    public static void Shutdown() => _running = false;
    public static void RegisterSaveItem<T>(T obj, string fileName) => SaveList.Add(new SaveItem<T>(obj, fileName));
    public static void DeRegisterSaveItem(string fileName) => SaveList.RemoveAll(m => m.FileName() == fileName);
}