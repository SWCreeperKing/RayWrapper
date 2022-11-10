using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.GameObject;
using RayWrapper.Base.Primitives;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.LegacyUI.UI;

public class Hierarchy : GameObject
{
    public const int IndentPadding = 10;
    public string selectedPath;
    public Vector2 offset;

    public Text.Style folderStyle = new()
    {
        color = Raylib.GREEN, fontSize = 25
    };

    public Text.Style objStyle = new()
    {
        color = new(0, 255, 255), fontSize = 20
    };

    public Scrollbar leftBar;
    public Scrollbar bottomBar;
    public Action<string> objectClicked = null;

    private readonly Folder _folder;
    private readonly Vector2 _sidePadding = new(5, 0);
    private Rectangle _cachedRect;
    private float _objHeight;
    private float _folderHeight;
    private float _fullHeight;
    private float _fullWidth;

    public Hierarchy(Rectangle rect, Folder folder)
    {
        _folder = folder;
        _folder.expanded = true;
        FolderUpdate();

        Position = rect.Pos + new Vector2(20, 0);
        Size = rect.Size - new Vector2(20);

        leftBar = new Scrollbar(new Rectangle(Position with { X = Position.X - 20 }, Size with { X = 20 }))
        {
            amountInvoke = () => Math.Max(0, _fullHeight - Size.Y)
        };

        bottomBar = new Scrollbar(new Rectangle(new Vector2(Position.X, Position.Y + Size.Y), Size with { Y = 20 }),
            false)
        {
            amountInvoke = () => Math.Max(0, _fullWidth - Size.X)
        };

        RegisterGameObj(leftBar, bottomBar);
    }

    protected override void UpdateCall(float dt)
    {
        offset = -new Vector2(bottomBar.Value, leftBar.Value);
        _cachedRect = GetRect();
    }

    protected override void RenderCall()
    {
        float Draw(Folder folder, Vector2 offset, string path, int indent = 0)
        {
            void HoverAndClick(Rectangle r, string path)
            {
                if (path == _folder.Name) return;
                if (selectedPath == path) r.DrawLines(Raylib.GRAY);
                if (r.IsMouseIn())
                {
                    r.DrawLines(Raylib.RAYWHITE);
                    var clicked = Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);
                    if (clicked && selectedPath != path) selectedPath = path;
                    else if (clicked)
                    {
                        var pathing = path.Split("/");
                        var holder = _folder;
                        for (var i = 0; i < pathing.Length - 2; i++) holder = holder[pathing[i + 1]];

                        if (holder.Folders.ContainsKey(pathing[^1]))
                        {
                            var f = holder[pathing[^1]];
                            f.expanded = !f.expanded;
                            FolderUpdate();
                        }
                        else objectClicked?.Invoke(path);
                    }
                }
            }

            var rect = new Rectangle(offset, new Vector2(0, _folderHeight));
            var addedY = _folderHeight + 5;
            var pad = (indent + 1) * IndentPadding;

            rect.W = folderStyle.MeasureText(folder.ToString()).X;
            folderStyle.Draw(folder.ToString(), rect);
            HoverAndClick(rect, path);
            rect.Pos += new Vector2(pad, addedY);

            foreach (var f in folder.Folders.Values)
            {
                var off = _folderHeight;

                rect.W = folderStyle.MeasureText(f.ToString()).X;

                if (f.expanded) off = Draw(f, rect.Pos, $"{path}/{f.Name}", indent + 1);
                else
                {
                    folderStyle.Draw(f.ToString(), rect);
                    HoverAndClick(rect, $"{path}/{f.Name}");
                    off += 5;
                }

                rect.Y += off;
                addedY += off;
            }

            rect.H = _objHeight;
            foreach (var obj in folder.Objs)
            {
                rect.W = objStyle.MeasureText(obj).X;
                objStyle.Draw(obj, rect);
                HoverAndClick(rect, $"{path}/{obj}");
                rect.Y += _objHeight + 5;
                addedY += _objHeight + 5;
            }

            return addedY;
        }

        _cachedRect.DrawLines(Raylib.GRAY);
        _cachedRect.MaskDraw(() => { Draw(_folder, Position + offset + _sidePadding, _folder.Name); });
    }

    public void AddToFolder(string newPath)
    {
        _folder.AddToList(newPath);
        FolderUpdate();
    }

    public void FolderUpdate()
    {
        Vector2 FolderMeasure(Folder f, out float folderHeight, int indent = 0)
        {
            var pad = (indent + 1) * IndentPadding;
            var size = folderStyle.MeasureText(f.ToString());

            if (!f.expanded)
            {
                folderHeight = size.Y;
                return size with { X = size.X + pad };
            }

            folderHeight = size.Y;
            size.Y = 0;

            size = f.Objs.Aggregate(size,
                (v2, name) =>
                    Vector2.Max(v2, objStyle.MeasureText(name)));

            foreach (var folder in f.Folders.Keys)
            {
                size = Vector2.Max(size, FolderMeasure(f[folder], out var tHeight, indent + 1));
                folderHeight = Math.Max(folderHeight, tHeight);
            }

            return size with { X = size.X + pad };
        }

        var tempSize = FolderMeasure(_folder, out _folderHeight);
        var (folders, objs) = _folder.CountCurrent();

        _fullWidth = tempSize.X + _sidePadding.X * 2 + 5;
        _objHeight = tempSize.Y;
        _fullHeight = (_folderHeight + 5) * folders + (_objHeight + 5) * objs;
    }
}