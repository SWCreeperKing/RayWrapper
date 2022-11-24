using System.Numerics;
using ImGuiNET;
using Raylib_CsLo;
using RayWrapper.Base.GameBox;
using RayWrapper.Imgui.Widgets;
using static RayWrapper.Base.GameBox.Logger.Level;

namespace RayWrapper.Imgui.Extras;

public class FileExplorer : WindowBase
{
    public static readonly uint WarningTextColor = Raylib.RED.ToUint();

    public enum SelectType
    {
        File,
        Folder
    }

    public string path;
    public SelectType typeSelect;

    private Pattern included;
    private Pattern excluded;
    private string _prevLoadedDir;
    private string _selected;
    private string _selectedWarn;
    private string[] _folders;
    private string[] _files;
    private string[] _names;
    private Action<string> _ok;
    private Action _cancle;

    /// <summary>
    /// include/exclude:
    /// ^ start with
    /// * end with
    /// , or
    /// & and
    /// </summary>
    public FileExplorer(Action<string> ok, Action cancle = null, string startingPath = "C:/",
        SelectType typeSelect = SelectType.File, string included = "",
        string excluded = "") : base("File Explorer", ImGuiWindowFlags.None)
    {
        path = startingPath;
        this.typeSelect = typeSelect;
        this.included = included == "" ? null : new Pattern(included);
        this.excluded = excluded == "" ? null : new Pattern(excluded);
        _ok = ok;
        _cancle = cancle;
    }

    protected override void WindowRender()
    {
        var wSize = ImGui.GetWindowContentRegionMax();

        var b = true;
        if (ImGui.BeginPopupModal("Warning.UnableToAccessFile", ref b, ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.PushStyleColor(ImGuiCol.Text, WarningTextColor);
            ImGui.Text($"WARNING THE Folder/File [{_selectedWarn}]\nCan not be accessed\nTry running in Administrator");
            ImGui.PopStyleColor();
            var mSize = ImGui.GetWindowContentRegionMax();
            ImGui.Separator();
            ImGui.SameLine(mSize.X / 2f);
            if (ImGui.Button("Ok")) ImGui.CloseCurrentPopup();
            ImGui.EndPopup();
        }

        if (path.Count(c => c == '/') > 1 && ImGui.Button("Back"))
        {
            if (path.EndsWith('/')) path = path.Remove(path.Length - 1);
            path = path[..(path.LastIndexOf('/') + 1)];
        }

        ImGui.SameLine();
        ImGui.Spacing();
        ImGui.SameLine();
        ImGui.InputTextWithHint("Path", "Path Here", ref path, 999);
        if (Directory.Exists(path))
        {
            if (path != _prevLoadedDir) LoadPath();
            ImGui.BeginListBox("", new Vector2(-1, Math.Max(0, wSize.Y - 75)));

            foreach (var name in _names)
            {
                if (!ImGui.Selectable(name, name == _selected)) continue;
                if (name != _selected) _selected = name;
                else
                {
                    if (!name.StartsWith("> ")) continue;
                    if (!path.EndsWith('/')) path += '/';
                    path += name[2..] + '/';
                }
            }

            ImGui.EndListBox();
        }
        else ImGui.Text($"No Such Directory '{path}'");

        ImGui.Separator();
        ImGui.SameLine(wSize.X - 100);
        ImGui.BeginGroup();
        if (ImGui.Button("Ok"))
        {
            var file = $"{path}/{_selected}";
            if (typeSelect is not SelectType.File || File.Exists(file)) _ok(file);
        }

        if (_cancle is not null)
        {
            ImGui.SameLine();
            if (ImGui.Button("Close")) _cancle();
        }

        ImGui.EndGroup();
    }

    public void LoadPath()
    {
        try
        {
            _folders = Directory.GetDirectories(path);
            _files = typeSelect is SelectType.File ? Directory.GetFiles(path) : Array.Empty<string>();

            if (included is not null) _files = _files.Where(f => included.contains(f)).ToArray();
            if (excluded is not null) _files = _files.Where(f => !excluded.contains(f)).ToArray();

            _names = _folders.Select(s => "> " + s.Replace("\\", "/").Split("/")[^1])
                .Union(_files.Select(s => s.Replace("\\", "/").Split("/")[^1])).ToArray();
        }
        catch (UnauthorizedAccessException)
        {
            _selectedWarn = _selected.StartsWith("> ") ? _selected[2..] : _selected;
            ImGui.OpenPopup("Warning.UnableToAccessFile");
            path = _prevLoadedDir;
            LoadPath();
            return;
        }

        _selected = "";
        _prevLoadedDir = path;
    }

    /// <summary>
    /// include/exclude:
    /// ^ start with
    /// * end with
    /// , or
    /// & and
    /// </summary>
    public class Pattern
    {
        public readonly (string starting, string ending)[] patterns;

        public Pattern(string pattern)
        {
            List<(string, string)> patterns = new();
            foreach (var andPattern in pattern.Split(","))
            {
                var andSplit = andPattern.Split("&").ToList();
                if (andPattern.Length > 2 || andPattern.Count(c => c == '^') > 1 ||
                    andPattern.Count(c => c == '*') > 1)
                {
                    Logger.Log(Warning, $"Pattern Error: [{andPattern}] does not match pattern minimum requirements");
                    continue;
                }

                var startingIndex = andSplit.IndexOf(andSplit.First(s => s.StartsWith('^')));
                var endingIndex = andSplit.IndexOf(andSplit.First(s => s.StartsWith('*')));

                patterns.Add((startingIndex == -1 ? "" : andSplit[startingIndex],
                    endingIndex == -1 ? "" : andSplit[endingIndex]));
            }

            this.patterns = patterns.ToArray();
        }

        public bool contains(string s)
        {
            return patterns.Any(p =>
            {
                var start = p.starting == "" || s.StartsWith(p.starting);
                var end = p.ending == "" || s.EndsWith(p.ending);
                return start || end;
            });
        }
    }
}