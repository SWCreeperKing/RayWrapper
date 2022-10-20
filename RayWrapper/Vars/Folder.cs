using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RayWrapper.Vars;

public record Folder(string Name, Dictionary<string, Folder> Folders, List<string> Objs)
{
    public bool expanded;

    public (int folders, int objs) CountCurrent()
    {
        if (!expanded) return (1, 0);
        if (!Folders.Any()) return (1, expanded ? Objs.Count : 0);

        return Folders.Values.Select(f => f.CountCurrent()).Aggregate((1, expanded ? Objs.Count : 0),
            (tuple, valueTuple) => (tuple.Item1 + valueTuple.folders, tuple.Item2 + valueTuple.objs));
    }

    public static Folder FolderSetFromPath(string path)
    {
        List<string> GetStrings(string path)
        {
            List<string> strings = new();
            strings.AddRange(Directory.GetFiles(path).Select(s => string.Join(".", s.Split(".")[..^1])));
            foreach (var dir in Directory.GetDirectories(path)) strings.AddRange(GetStrings(dir));
            return strings;
        }

        var dir = path.Replace("\\", "/");
        if (!dir.EndsWith("/")) dir += '/';
        var files = GetStrings(path);
        var processed = files.Select(f => f[dir.Length..]).Select(s => s.Replace("\\", "/")).ToList();

        return GenFolderFromPathing(dir.Split("/")[^2], processed);
    }

    public static Folder GenFolderFromPathing(string name, List<string> objList)
    {
        var objs = objList.Where(f => !f.Contains('/'));
        var dict = new Dictionary<string, Folder>();

        var next = objList.Where(f => f.Contains('/')).GroupBy(f => f[..f.IndexOf('/')]);
        foreach (var dir in next)
        {
            if (!dir.Any()) continue;
            dict[dir.Key] = GenFolderFromPathing(dir.Key, dir.Select(f => f[(dir.Key.Length + 1)..]).ToList());
        }

        return new Folder(name, dict, objs.ToList());
    }

    public void AddToList(string path)
    {
        if (!path.Contains('/'))
        {
            Objs.Add(path);
            return;
        }

        var index = path.IndexOf('/');
        var nextPath = path[..index];
        var further = path[(index + 1)..];

        if (!Folders.ContainsKey(nextPath))
        {
            Folders.Add(nextPath, new Folder(nextPath, new Dictionary<string, Folder>(), new List<string>()));
        }

        this[nextPath].AddToList(further);
    }

    public Folder this[string key]
    {
        get => Folders[key];
        set => Folders[key] = value;
    }

    public Folder this[int i]
    {
        get => Folders[Folders.Keys.ElementAt(i)];
        set => Folders[Folders.Keys.ElementAt(i)] = value;
    }

    public override string ToString() => $"{(expanded ? '-' : '+')} {Name}";
}