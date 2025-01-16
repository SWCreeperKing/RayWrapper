namespace RayWrapper.Base.GameBox;

public static class SceneManager
{
    public static Dictionary<string, GameLoop> Scenes = new();
    public static Dictionary<string, bool> HasInit = new();

    public static void AddScene(string id, GameLoop loop)
    {
        if (Scenes.ContainsKey(id)) throw new ArgumentException($"You can not add scene with duplicate ID [{id}]");
        Scenes.Add(id, loop);
        HasInit.Add(id, false);
    }

    public static void RemoveScene(string id)
    {
        Scenes.Remove(id);
        HasInit.Remove(id);
    }

    public static void CheckScene(string id)
    {
        if (!Scenes.ContainsKey(id)) throw new ArgumentException($"There is no Scene matching the ID [{id}]");
    }

    public static void InitScene(string id)
    {
        CheckScene(id);
        if (HasInit[id]) throw new ArgumentException($"The scene with the ID [{id}] has been already Initialized");
        Scenes[id].Init();
        HasInit[id] = true;
    }

    public static void UpdateScene(string id, float dt)
    {
        CheckScene(id);
        Scenes[id].Update(dt);
    }

    public static void RenderScene(string id)
    {
        CheckScene(id);
        Scenes[id].Render();
    }

    public static void DisposeScene(string id)
    {
        CheckScene(id);
        Scenes[id].Dispose();
    }

    public static void DisposeAllScenes()
    {
        HasInit.Where(kv => kv.Value).Select(kv => kv.Key).ToList().ForEach(DisposeScene);
    }
}