using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Var_Interfaces;
using RayWrapper.Vars;
using ZimonIsHimUtils.ExtensionMethods;
using static Raylib_CsLo.Raylib;

namespace RayWrapper.Objs
{
    public class TabView : GameObject
    {
        public static Style defaultStyle = new();

        public override Vector2 Position
        {
            get => _rect.Pos();
            set
            {
                _rect.MoveTo(value);
                _bar.Position = value + new Vector2(0, 40);
                Refresh();
            }
        }

        public override Vector2 Size => _rect.Size();

        public Style style = defaultStyle.Copy();
        public bool drawIfLowTabs = false;
        public bool outline = true;
        public Action<string, int> tabChanged = null;

        private readonly Scrollbar _bar;
        private readonly IDictionary<string, Scene> _tabContents = new Dictionary<string, Scene>();
        private readonly IList<Label> _tabs = new List<Label>();
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
            _bar.Update();


            try
            {
                if (!GameBox.IsMouseOccupied)
                {
                    _tabs.Each(t => t.Update());
                }
            }
            catch (InvalidOperationException)
            {
                // catches Collection was modified error
            }

            if (_currentTab is null || !_tabContents.ContainsKey(_currentTab)) return;
            _tabContents[_currentTab].Update();
        }

        protected override void RenderCall()
        {
            if (!(!drawIfLowTabs && _tabs.Count < 2))
            {
                _rect.MaskDraw(() => { _tabs.Each(t => t.Render()); });

                if (outline) _rect.DrawHallowRect(BLACK);
                if (_bar.Amount() > 1) _bar.Render();
            }

            if (_currentTab is null || !_tabContents.ContainsKey(_currentTab)) return;
            _tabContents[_currentTab].Render();
        }

        public void Refresh() => ReCalculate();

        public void ReCalculate()
        {
            var startX = _rect.x - _offset;
            var hOff = outline ? 2 : 0;
            foreach (var l in _tabs)
            {
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
                        if (name == _currentTab) return c.MakeDarker();
                        return b ? c.MakeLighter() : c;
                    }
                }
            };

            if (insert != -1)
            {
                _tabs.Insert(insert, l);
                _tabOrder.Insert(insert, name);
            }
            else
            {
                _tabs.Add(l);
                _tabOrder.Add(name);
            }
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

        public float GetTabLength() => _tabs.Sum(t => t.Size.X) + (_tabs.Count - 1) * _padding;

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
}