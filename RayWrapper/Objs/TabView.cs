using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RayWrapper.Base;
using RayWrapper.Base.GameObject;
using RayWrapper.Var_Interfaces;
using RayWrapper.Vars;
using ZimonIsHimUtils.ExtensionMethods;
using static Raylib_CsLo.MouseCursor;
using static Raylib_CsLo.Raylib;
using Rectangle = Raylib_CsLo.Rectangle;

namespace RayWrapper.Objs;

public class TabView : GameObject
{
    public static Style defaultStyle = new();

    public Style style = defaultStyle.Copy();
    public bool drawIfLowTabs = false;
    public bool outline = true;
    public Action<string, int> tabChanged = null;

    private readonly Scrollbar _bar;
    private readonly IDictionary<string, Scene> _tabContents = new Dictionary<string, Scene>();

    private readonly IDictionary<string, Label> _tabs = new Dictionary<string, Label>();

    // private readonly IList<Label> _tabs = new List<Label>();
    private readonly IList<string> _tabOrder = new List<string>();
    private readonly int _padding = 7;
    private string _currentTab;
    private float _offset;
    private Rectangle _rect;

    public TabView(Vector2 pos, float width)
    {
        _rect = new Rectangle(pos.X, pos.Y, width, 35);
        _bar = new Scrollbar(new Rectangle(pos.X, pos.Y + 35, width, 18), false)
            { amountInvoke = () => GetTabLength() - _rect.width, style = style.scrollStyle };
        _bar.OnMoveEvent += f =>
        {
            _offset = f;
            Refresh();
        };
    }

    protected override void UpdateCall()
    {
        try
        {
            if (!GameBox.IsMouseOccupied)
            {
                _tabs.Values.Each(t => t.Update());
            }
        }
        catch (InvalidOperationException)
        {
            // catches Collection was modified error
        }

        if (_bar.Amount() > 1) _bar.Update();
        if (_currentTab is null || !_tabContents.ContainsKey(_currentTab)) return;
        _tabContents[_currentTab].Update();
    }

    protected override void RenderCall()
    {
        if (!(!drawIfLowTabs && _tabs.Count < 2))
        {
            _rect.MaskDraw(() => { _tabs.Values.Each(t => t.Render()); });

            if (outline) _rect.DrawHallowRect(BLACK);
            if (_bar.Amount() > 1) _bar.Render();
                
            // _tabs.Values.Each(t =>
            // {
            //     if (t.Rect.IsMouseIn()) GameBox.SetMouseCursor(MOUSE_CURSOR_POINTING_HAND);
            // });
        }

        if (_currentTab is null || !_tabContents.ContainsKey(_currentTab)) return;
        _tabContents[_currentTab].Render();
    }
    
    protected override Vector2 GetPosition() => _rect.Pos;
    protected override Vector2 GetSize() => _rect.Size;
    protected override void UpdatePosition(Vector2 newPos)
    {
        _rect.MoveTo(newPos);
        _bar.Position = newPos + new Vector2(0, 40);
        Refresh();
    }

    protected override void UpdateSize(Vector2 newSize)
    {
        _rect.SetSize(newSize);
        Refresh();
    }

    public void Refresh() => ReCalculate();

    public void ReCalculate()
    {
        var startX = _rect.x - _offset;
        var hOff = outline ? 2 : 0;
        foreach (var t in _tabOrder)
        {
            var l = _tabs[t];
            var newPos = new Vector2(startX, _rect.y + hOff);
            l.Position = newPos;
            startX += l.Size.X + _padding;
        }
    }

    private void NewTab(string name, int insert = -1)
    {
        Label l = new(new Vector2(0, _rect.y), name)
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
        _bar.Update();
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
}