using RayWrapper.Base.GameBox;
using RayWrapper.Base.Primitives;
using static RayWrapper.Base.GameBox.GameBox;
using static RayWrapper.Base.GameBox.Logger.Level;

namespace RayWrapper.Base.SaveSystem;

public static class SaveExt
{
    public static readonly List<ISave> SaveList = new();

    public static bool SaveInit { get; private set; }
    public static string DeveloperName { get; private set; }
    public static string AppName { get; private set; } = "Unknown App";
    public static void SaveItems() => SaveList.SaveItems();
    public static void LoadItems() => SaveList.LoadItems();
    public static void DeleteFile(string name) => SaveList.DeleteFile(name);
    public static void DeleteAll() => SaveList.DeleteAll();

    public static string GetSavePath =>
        File.Exists("UseLocalPath") ? $"{AppName}" : $"{CoreDir}/{DeveloperName}/{AppName}";

    public static void InitSaveSystem(string developerName, string appName) =>
        (DeveloperName, AppName, SaveInit) = (developerName, appName, true);

    public static void SaveToFile(this ISave t, string path)
    {
        using var sw = File.CreateText($"{path}/{t.FileName()}.RaySaveWrap");
        sw.Write(t.SaveString());
        sw.Close();
    }

    public static void LoadToFile(this ISave t, string path)
    {
        var file = $"{path}/{t.FileName()}.RaySaveWrap";
        if (!File.Exists(file)) return;
        using var sr = new StreamReader(file);
        t.LoadString(sr.ReadToEnd(), file);
        sr.Close();
    }

    public static void SaveItems(this List<ISave> saveList)
    {
        Logger.Log(Info, $"Saving Start @ {DateTime.Now:G}");
        var start = GetTimeMs();
        ISave.IsSaveInitCheck();
        var path = GetSavePath;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        foreach (var t in saveList) t.SaveToFile(path);
        Logger.Log(Info, $"Saved in {new TimeVar(GetTimeMs() - start)}");
    }

    public static void LoadItems(this List<ISave> saveList)
    {
        Logger.Log(Info, $"Loading Start @ {DateTime.Now:G}");
        var start = GetTimeMs();
        ISave.IsSaveInitCheck();
        var path = GetSavePath;
        if (!Directory.Exists(path)) return;
        foreach (var t in saveList) t.LoadToFile(path);
        Logger.Log(Info, $"Loaded in {new TimeVar(GetTimeMs() - start)}");
    }

    public static void DeleteFile(this IEnumerable<ISave> saveList, string name)
    {
        ISave.IsSaveInitCheck();
        var path = GetSavePath;
        if (!Directory.Exists(path)) return;
        var file = $"{path}/{saveList.First(s => s.FileName() == name).FileName()}.RaySaveWrap";
        Logger.Log("Save System: Files deleted");
        if (!File.Exists(file)) return;
        File.Delete(file);
    }

    public static void DeleteAll(this IEnumerable<ISave> saveList)
    {
        ISave.IsSaveInitCheck();
        var path = GetSavePath;
        if (!Directory.Exists(path)) return;
        foreach (var file in saveList.Select(t => $"{path}/{t.FileName()}.RaySaveWrap").Where(File.Exists))
        {
            File.Delete(file);
        }
    }
}