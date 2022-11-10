using System.Numerics;
using ImGuiNET;
using Raylib_CsLo;
using RayWrapper.Imgui.Widgets;

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
    public string included;
    public string excluded;

    private string _prevLoadedDir;
    private string _selected;
    private string _selectedWarn;
    private string[] _folders;
    private string[] _files;
    private string[] _names;

    /// <summary>
    /// include/exclude:
    /// ^ start with
    /// * end with
    /// , or
    /// & and
    /// </summary>
    public FileExplorer(string startingPath = "C:/", string included = "", string excluded = "") : base("File Explorer",
        ImGuiWindowFlags.None)
    {
        path = startingPath;
        this.included = included;
        this.excluded = excluded;
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
        ImGui.Button("Ok");
        ImGui.SameLine();
        ImGui.Button("Close");
        ImGui.EndGroup();
    }

    public void LoadPath()
    {
        try
        {
            _folders = Directory.GetDirectories(path);
            _files = Directory.GetFiles(path);

            // if (included != "")
            // {
            //     _files = _files.Where()
            // }

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

    // public bool FollowsPattern(string s, string pattern)
    // {
    //     
    // }
}