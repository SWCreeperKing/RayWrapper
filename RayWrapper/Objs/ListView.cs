﻿using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static Raylib_cs.Raylib;
using static RayWrapper.GeneralWrapper;

namespace RayWrapper.Objs
{
    public class ListView : GameObject
    {
        public override Vector2 Position
        {
            get => _bounds.Pos();
            set
            {
                _bar.Position = value + new Vector2(-20, 0);
                _bounds = _bounds.MoveTo(value);
                UpdateLabels(_bar.Value);
            }
        }

        public override Vector2 Size => _bounds.Size();

        public Actionable<string> tooltip = new("");
        public ColorModule backColor = new(50);
        public ColorModule fontColor = new(192);
        public ColorModule selectColor = new(60, 60, 100);
        public Sound clickSound;
        public Func<int> arrayLength;
        public Func<int, Color> backColors;
        public Func<int, Color> fontColors;
        public Func<int, string> itemProcessing;
        public Func<int, string> indivTooltip;
        public Action click;
        public Action outsideClick;
        public bool randomPitch = true;
        public bool showTooltip = true;
        public bool rememberLast = true;
        public bool useSelection = true;
        public bool selectedToggle;

        private readonly Label[] _labels;
        private readonly Scrollbar _bar;
        private readonly float _padding = 5;
        private readonly int _itemsToShow;
        private readonly int _labelHeight;
        private Rectangle _bounds;
        private Action<int> _individualClick;
        private float _lastValue;
        private int _lastLength;
        private int _lastSelect = -1;

        public ListView(Vector2 pos, int width, Func<int, string> itemProcessing, Func<int> arrayLength,
            int itemsToShow = 10, int labelHeight = 40, float padding = 5f)
        {
            (this.itemProcessing, this.arrayLength, _padding, _itemsToShow, _labelHeight) =
                (itemProcessing, arrayLength, padding, itemsToShow, labelHeight);
            var height = CalcHeight();
            _bar = new Scrollbar(new Rectangle(pos.X, pos.Y, 18, height))
            {
                amountInvoke = () => this.arrayLength.Invoke() + 1 - _itemsToShow
            };
            _bounds = new Rectangle(pos.X + 20, pos.Y, width - 20, height);

            _labels = new Label[itemsToShow + 1];
            for (var i = 0; i < _labels.Length; i++)
                _labels[i] = new Label(new Rectangle(0, 0, _bounds.width, labelHeight))
                {
                    useBaseHover = new Actionable<bool>(() => _individualClick is not null), updateReturnIfNonVis = true
                };
            _bar.OnMoveEvent += UpdateLabels;
            UpdateLabels(0);
        }

        public Action<int> IndividualClick
        {
            get => _individualClick;
            set
            {
                _individualClick = value;
                if (_bar is null) return;
                UpdateLabels(_bar.Value);
            }
        }

        public float Offset
        {
            get => _bar.GetOffset;
            set => _bar.MoveBar(value);
        }

        public float Value => _bar.Value;
        public string this[int idx] => itemProcessing.Invoke(idx);

        private void UpdateLabels(float value)
        {
            var strictVal = (int)value;
            var labelPadding = _labelHeight + _padding;
            var y = _bounds.Pos().Y - labelPadding * (value - strictVal);
            foreach (var l in _labels) l.isVisible = false;

            for (var i = 0; i < Math.Min(_labels.Length, arrayLength.Invoke() - strictVal); i++)
            {
                var notI = i;
                var place = strictVal + notI;
                var l = _labels[i];
                l.isVisible = true;
                l.text = new Actionable<string>(() =>this[place]);
                if (_individualClick is not null)
                    l.clicked = () =>
                    {
                        _individualClick(place);
                        if (!useSelection)  return;
                        if (_lastSelect == place && selectedToggle) _lastSelect = -1;
                        else _lastSelect = place;
                    };
                l.Position = new Vector2(_bounds.x, y + labelPadding * i);
                l.backColor =
                    new ColorModule(() =>
                        place == _lastSelect && _individualClick is not null
                            ? (Color)selectColor
                            : backColors?.Invoke(place) ?? (Color)backColor);
                l.fontColor =
                    new ColorModule(fontColors?.Invoke(place) ?? (Color)fontColor);
                if (indivTooltip is null) continue;
                l.tooltip = indivTooltip.Invoke(place);
            }
        }

        private void UpdateText()
        {
            var value = _bar.Value;
            for (var i = 0; i < Math.Min(_labels.Length, arrayLength.Invoke() - (int)value); i++)
            {
                _labels[i].text = this[(int)value + i];
                _labels[i].fontColor = new ColorModule(fontColors?.Invoke((int)value + i) ?? (Color)fontColor);
            }
        }

        protected override void UpdateCall()
        {
            _bar.Update();
            if (_lastLength != arrayLength.Invoke() || _lastValue != _bar.Value)
            {
                (_lastLength, _lastValue) = (arrayLength.Invoke(), _bar.Value);
                UpdateLabels(Value);
            }

            if (_bounds.IsMouseIn())
            {
                var scroll = GetMouseWheelMove();
                if (scroll != 0)
                {
                    var a = arrayLength.Invoke();
                    // smooth scrolling, help from chocobogamer#4214
                    _bar.MoveBar(scroll * (((_labelHeight + _padding) * ((float)a / _itemsToShow) - _padding) /
                                           (((_labelHeight + _padding) * a - _padding) / _bar.container.height)));
                    _bar.Update();
                    UpdateLabels(_bar.Value);
                }

                if (!IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON)) return;
                click?.Invoke();
                if (randomPitch) SetSoundPitch(clickSound, GameBox.Random.Next(.75f, 1.25f));
                PlaySound(clickSound);
                foreach (var l in _labels) l?.Update();
                return;
            }

            if (!IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON) ||
                _bounds.ExtendPos(new Vector2(20, 0)).IsMouseIn()) return;
            outsideClick?.Invoke();
        }

        protected override void RenderCall()
        {
            UpdateText();
            _bounds.MaskDraw(() =>
            {
                foreach (var l in _labels) l.Render();
            });
            if (arrayLength.Invoke() > _itemsToShow) _bar.Render();
            if (showTooltip && tooltip != "") _bounds.ExtendPos(new Vector2(20, 0)).DrawTooltip(tooltip);
        }

        public float CalcHeight() => (_labelHeight + _padding) * _itemsToShow - _padding;
        public void Deselect() => _lastSelect = -1;

        public void Select(int select, bool doOnClick = false)
        {
            _lastSelect = select;
            if (!doOnClick) return;
            click?.Invoke();
            IndividualClick?.Invoke(select);
        }
    }
}