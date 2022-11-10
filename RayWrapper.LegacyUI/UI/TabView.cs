using System.Numerics;
using RayWrapper.Base.GameBox;
using RayWrapper.Base.GameObject;
using RayWrapper.Base.Primitives;
using ZimonIsHimUtils.ExtensionMethods;
using static Raylib_CsLo.MouseCursor;
using static Raylib_CsLo.Raylib;
using static RayWrapper.Base.GameBox.Input;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.LegacyUI.UI;

public class TabView : GameObject
{
    public static Style defaultStyle = new();

    public Style style = defaultStyle.Copy();
    public bool drawIfLowTabs = false;
    public bool outline = true;
    public Action<string, int> tabChanged = null;

    private readonly Scrollbar _bar;
    private readonly IDictionary<string, GameObject> _tabContents = new Dictionary<string, GameObject>();

    private readonly IDictionary<string, Label> _tabs = new Dictionary<string, Label>();

    private readonly IList<string> _tabOrder = new List<string>();
    private readonly int _padding = 7;

    private string _currentTab;
    private float _offset;
    private Rectangle _rect;
    private List<(string name, int insert)> _newQueue = new();

    public TabView(Vector2 pos, float width)
    {
        _rect = new Rectangle(pos.X, pos.Y, width, 35);
        _bar = new Scrollbar(new Rectangle(pos.X, pos.Y + 35, width, 18), false)
            { amountInvoke = () => GetTabLength() - _rect.W, style = style.scrollStyle };
        _bar.OnMoveEvent += f =>
        {
            _offset = f;
            Refresh();
        };
        Refresh();
    }

    protected override void UpdateCall(float dt)
    {
        try
        {
            if (_newQueue.Any())
            {
                _newQueue.Each(n => MakeNewTab(n.name, n.insert));
                _newQueue.Clear();
                Refresh();
            }

            if (!IsMouseOccupied) _tabs.Values.Each(t => t.Update(dt));
        }
        catch (InvalidOperationException)
        {
            // catches Collection was modified error
        }

        if (_bar.Amount() > 1) _bar.Update(dt);
        if (_currentTab is null || !_tabContents.ContainsKey(_currentTab)) return;
        _tabContents[_currentTab].Update(dt);
    }

    protected override void RenderCall()
    {
        if (!(!drawIfLowTabs && _tabs.Count < 2))
        {
            _rect.MaskDraw(() => { _tabs.Values.Each(t => t.Render()); });

            if (outline) _rect.DrawLines(color: BLACK);
            if (_bar.Amount() > 1) _bar.Render();
        }

        if (_currentTab is null || !_tabContents.ContainsKey(_currentTab)) return;
        _tabContents[_currentTab].Render();
    }

    protected override Vector2 GetPosition() => _rect.Pos;
    protected override Vector2 GetSize() => _rect.Size;

    protected override void UpdatePosition(Vector2 newPos)
    {
        _rect.Pos = newPos;
        _bar.Position = newPos + new Vector2(0, 40);
        Refresh();
    }

    protected override void UpdateSize(Vector2 newSize)
    {
        _rect.Size = newSize;
        Refresh();
    }

    public void Refresh() => ReCalculate();

    public void ReCalculate()
    {
        var startX = _rect.X - _offset;
        var hOff = outline ? 2 : 0;

        foreach (var t in _tabOrder)
        {
            var l = _tabs[t];
            var newPos = new Vector2(startX, _rect.Y + hOff);
            l.Position = newPos;
            startX += l.Size.X + _padding;
        }
    }

    private void MakeNewTab(string name, int insert)
    {
        Label l = new(new Vector2(0, _rect.Y), name)
        {
            clicked = () =>
            {
                _currentTab = name;
                tabChanged?.Invoke(name, _tabOrder.IndexOf(name));
                Refresh();
            },
            style =
            {
                drawHover = new Actionable<bool>(name != _currentTab),
                drawMode = Label.Style.DrawMode.AlignCenter,
                drawColor = (c, b) =>
                {
                    if (name == _currentTab)
                    {
                        if (b) GameBox.SetMouseCursor(MOUSE_CURSOR_NOT_ALLOWED);
                        return c.MakeDarker();
                    }

                    if (b) GameBox.SetMouseCursor(MOUSE_CURSOR_POINTING_HAND);
                    return b ? c.MakeLighter() : c;
                }
            }
        };
        _tabs.Add(name, l);

        if (insert != -1) _tabOrder.Insert(insert, name);
        else _tabOrder.Add(name);
    }

    private void NewTab(string name, int insert = -1) => _newQueue.Add((name, insert));

    public void AddTab(string tabName, params IGameObject[] gobjs)
    {
        if (_tabContents.ContainsKey(tabName)) return;
        _currentTab ??= tabName;
        var s = new Scene();
        s.RegisterGameObj(gobjs);
        _tabContents.Add(tabName, s);
        NewTab(tabName);
        Refresh();
    }

    public void InsertTab(string tabName, int index, params IGameObject[] gobjs)
    {
        if (_tabContents.ContainsKey(tabName)) return;
        _currentTab ??= tabName;
        var s = new Scene();
        s.RegisterGameObj(gobjs);
        _tabContents.Add(tabName, s);
        NewTab(tabName, index);
        Refresh();
    }

    public void RemoveTab(string tabName)
    {
        if (!_tabContents.ContainsKey(tabName)) return;
        _tabContents.Remove(tabName);
        _tabOrder.Remove(tabName);
        _tabs.Remove(tabName);
        if (_currentTab == tabName) _currentTab = _tabContents.Any() ? _tabContents.Keys.First() : null;
        Refresh();
        _bar.Update(0);
        _bar.MoveBar(0);
        Refresh();
    }

    public void AddToTab(string tabName, params IGameObject[] gobjs)
    {
        if (!_tabContents.ContainsKey(tabName)) return;
        _tabContents[tabName].RegisterGameObj(gobjs);
    }

    public float GetTabLength() => _tabs.Values.Sum(t => t.Size.X) + (_tabs.Count - 1) * _padding;

    public string GetCurrentTab() => _currentTab;
    public bool ContainsTab(string tabName) => _tabContents.ContainsKey(tabName);

    public IGameObject[] GetTabContents(string tabName)
    {
        return _tabContents.ContainsKey(tabName) ? _tabContents[tabName].GetRegistry() : null;
    }

    public class Style : IStyle<Style>
    {
        public Scrollbar.Style scrollStyle = new();

        public Style Copy()
        {
            return new Style { scrollStyle = scrollStyle.Copy() };
        }
    }

    private class Scene : GameObject
    {
    }
}