using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    public class TabView : GameObject
    {
        public override Vector2 Position
        {
            get => _rect.Pos();
            set
            {
                _rect = _rect.MoveTo(value);
                _bar.Position = value + new Vector2(0, 40);
                Refresh();
            }
        }

        public override Vector2 Size => _rect.Size();

        public bool drawIfLowTabs = false;
        public bool outline = true;
        public Action<string, int> tabChanged = null;

        private readonly Scrollbar _bar;
        private readonly Color _baseColor = new(95, 95, 95, 255);
        private bool _closable;
        private readonly List<Label> _closing = new();
        private string _currentTab;
        private readonly Color _hoverColor = new(65, 65, 65, 255);
        private float _offset;
        private readonly int _padding = 7;
        private Rectangle _rect;
        private readonly Dictionary<string, Scene> _tabContents = new();
        private readonly Dictionary<string, float> _tabLengths = new();
        private readonly List<string> _tabOrder = new();
        private readonly List<Label> _tabs = new();

        public TabView(Vector2 pos, float width)
        {
            _rect = new Rectangle(pos.X, pos.Y, width, 40);
            _bar = new Scrollbar(new Rectangle(pos.X, pos.Y + 40, width, 18), false)
                { amountInvoke = () => GetTabLength() - _rect.width };
            _bar.OnMoveEvent += f =>
            {
                _offset = f;
                Refresh();
            };
        }

        public bool Closable
        {
            get => _closable;
            set
            {
                _closable = value;
                Refresh();
                _bar.Update();
                _bar.MoveBar(0);
            }
        }

        protected override void UpdateCall()
        {
            _bar.Update();
            try
            {
                if (!GameBox.IsMouseOccupied)
                    foreach (var t in _tabs)
                        t.Update();
            }
            catch (InvalidOperationException)
            {
            }

            if (_closable)
                foreach (var t in _closing)
                    t.Update();
            if (_currentTab is null || !_tabContents.ContainsKey(_currentTab)) return;
          _tabContents[_currentTab].Update();
        }

        protected override void RenderCall()
        {
            if (!(!drawIfLowTabs && _tabs.Count < 2))
            {
                _rect.MaskDraw(() =>
                {
                    foreach (var t in _tabs) t.Render();
                });
                if (_closable)
                    foreach (var t in _closing)
                        t.Render();
                if (outline) _rect.DrawHallowRect(Color.BLACK);
                if (_bar.Amount() > 1) _bar.Render();
            }

            if (_currentTab is null || !_tabContents.ContainsKey(_currentTab)) return;
            _tabContents[_currentTab].Render();
        }

        public void Refresh() => RefreshTabs();

        public void RefreshTabs()
        {
            _tabs.Clear();
            var startX = _rect.x - _offset;
            var heightOff = outline ? 3 : 0;
            foreach (var name in _tabOrder)
            {
                if (startX + _tabLengths[name] + 25 <= _rect.x)
                {
                    startX += _tabLengths[name] + _padding + (_closable ? 25 : 0);
                    continue;
                }

                if (startX + _tabLengths[name] > _rect.x)
                {
                    Label l = new(new Rectangle(startX, _rect.y + heightOff, _tabLengths[name], 40 - heightOff * 2),
                        name, Label.TextMode.AlignCenter)
                    {
                        clicked = () =>
                        {
                            _currentTab = name;
                            tabChanged?.Invoke(name, _tabOrder.IndexOf(name));
                            Refresh();
                        },
                        useBaseHover = new Actionable<bool>(name != _currentTab),
                        outline = true,
                        backColor = new ColorModule(() => name == _currentTab ? _baseColor : _hoverColor)
                    };
                    _tabs.Add(l);
                }

                startX += _tabLengths[name];

                if (_closable && startX + 25 > _rect.x)
                {
                    Label l = new(new Rectangle(startX, _rect.y + heightOff, 25, 40 - heightOff * 2), "x",
                        Label.TextMode.AlignCenter)
                    {
                        backColor = Color.RED, clicked = () => RemoveTab(name), outline = true, useBaseHover = true
                    };
                    _tabs.Add(l);
                    startX += 25 + _padding;
                }
                else startX += _padding + (_closable ? 25 : 0);
            }
        }

        public void AddTab(string tabName, params GameObject[] gobjs)
        {
            if (_tabContents.ContainsKey(tabName)) return;
            _tabOrder.Add(tabName);
            _currentTab ??= tabName;
            var s = new Scene();
            s.RegisterGameObj(gobjs);
            _tabContents.Add(tabName, s);
            _tabLengths.Add(tabName, GameBox.Font.MeasureText($" {tabName} ").X);
            Refresh();
        }

        public void InsertTab(string tabName, int index, params GameObject[] gobjs)
        {
            if (_tabContents.ContainsKey(tabName)) return;
            _tabOrder.Insert(index, tabName);
            _currentTab ??= tabName;
            var s = new Scene();
            s.RegisterGameObj(gobjs);
            _tabContents.Add(tabName, s);
            _tabLengths.Add(tabName, GameBox.Font.MeasureText($" {tabName} ").X);
            Refresh();
        }

        public void RemoveTab(string tabName)
        {
            if (!_tabContents.ContainsKey(tabName)) return;
            _tabOrder.Remove(tabName);
            _tabContents.Remove(tabName);
            _tabLengths.Remove(tabName);
            if (_currentTab == tabName) _currentTab = _tabContents.Any() ? _tabContents.Keys.First() : null;
            Refresh();
            _bar.Update();
            _bar.MoveBar(0);
        }

        public void AddToTab(string tabName, params GameObject[] gobjs)
        {
            if (!_tabContents.ContainsKey(tabName)) return;
            _tabContents[tabName].RegisterGameObj(gobjs);
        }
        
        public float GetTabLength() =>
            _tabLengths.Values.Sum() + (_closable ? 25 * _tabLengths.Values.Count : 0) +
            (_tabLengths.Count - 1) * _padding;

        public string GetCurrentTab() => _currentTab;
        public bool ContainsTab(string tabName) => _tabOrder.Contains(tabName);

        public GameObject[] GetTabContents(string tabName) =>
            _tabContents.ContainsKey(tabName) ? _tabContents[tabName].GetRegistry() : null;
    }
}