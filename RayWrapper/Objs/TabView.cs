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

        public bool outline = true;

        private Scrollbar _bar;
        private bool _closable;
        private List<Label> _closing = new();
        private string _currentTab;
        private float offset;
        private int _padding = 7;
        private Rectangle _rect;
        private Dictionary<string, GameObject[]> _tabContents = new();
        private Dictionary<string, float> _tabLengths = new();
        private List<Label> _tabs = new();

        public TabView(Vector2 pos, float width) : base(pos)
        {
            _rect = new Rectangle(pos.X, pos.Y, width, 40);
            _bar = new Scrollbar(new Rectangle(pos.X, pos.Y + 40, width, 18), false);
            _bar.OnMoveEvent += f =>
            {
                offset = f;
                // offset = _bar.Value * barScale;
                Refresh();
            };
        }

        public override void Update()
        {
            _bar.Update();
            try
            {
                if (!GeneralWrapper.MouseOccupied) _tabs.ForEach(t => t.Update());
            }
            catch (InvalidOperationException)
            {
            }

            if (_closable) _closing.ForEach(t => t.Update());
            if (_currentTab is null || !_tabContents.ContainsKey(_currentTab)) return;
            foreach (var go in _tabContents[_currentTab]) go.Update();
        }

        public override void Render()
        {
            if (_bar.amount > 1) _bar.Render();
            _rect.MaskDraw(() => _tabs.ForEach(t => t.Render()));
            if (_closable) _closing.ForEach(t => t.Render());
            if (outline) _rect.DrawHallowRect(Color.BLACK);
            if (_currentTab is null || !_tabContents.ContainsKey(_currentTab)) return;
            foreach (var go in _tabContents[_currentTab]) go.Render();
        }

        public override void PositionChange(Vector2 v2)
        {
            _rect = _rect.MoveTo(v2);
            _bar.MoveTo(v2 + new Vector2(0, 40));
            Refresh();
        }

        public void Refresh()
        {
            _bar.amount = GetTabLength() - _rect.width;
            RefreshTabs();
        }

        public void RefreshTabs()
        {
            _tabs.Clear();
            var startX = _rect.x - offset;
            foreach (var (name, _) in _tabContents)
            {
                if (startX > _rect.x || startX + _tabLengths[name] > _rect.x)
                {
                    Label l = new(new Rectangle(startX, _rect.y, _tabLengths[name], 40), name, Label.TextMode
                        .AlignCenter);
                    if (name == _currentTab) l.backColor = new Color(70, 70, 70, 255);
                    l.clicked = () =>
                    {
                        _currentTab = name;
                        Refresh();
                    };
                    _tabs.Add(l);
                }

                startX += _tabLengths[name];

                if (_closable)
                {
                    if (startX > _rect.x || startX + 25 > _rect.x)
                    {
                        Label l = new(new Rectangle(startX, _rect.y, 25, 40), "x", Label.TextMode.AlignCenter);
                        l.backColor = Color.RED;
                        l.clicked = () => RemoveTab(name);
                        _tabs.Add(l);
                    }

                    startX += 25 + _padding;
                }
                else startX += _padding;
            }
        }

        public void AddTab(string tabName, params GameObject[] gobjs)
        {
            if (_tabContents.ContainsKey(tabName)) return;
            _currentTab ??= tabName;
            _tabContents.Add(tabName, gobjs);
            _tabLengths.Add(tabName, GameBox.font.MeasureText($" {tabName} ").X);
            Refresh();
        }

        public void RemoveTab(string tabName)
        {
            if (!_tabContents.ContainsKey(tabName)) return;
            _tabContents.Remove(tabName);
            _tabLengths.Remove(tabName);
            if (_currentTab == tabName) _currentTab = _tabContents.Any() ? _tabContents.Keys.First() : null;
            Refresh();
            _bar.Update();
            _bar.MoveBar(0);
        }

        public float GetTabLength() =>
            _tabLengths.Values.Sum() + (_closable ? 25 * _tabLengths.Values.Count : 0) +
            (_tabLengths.Count - 1) * _padding;

        public string GetCurrentTab() => _currentTab;
    }
}