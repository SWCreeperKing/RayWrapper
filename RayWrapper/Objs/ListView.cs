using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static Raylib_cs.Raylib;
using static RayWrapper.GeneralWrapper;

namespace RayWrapper.Objs
{
    public class ListView : GameObject
    {
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

        public Func<int> arrayLength;
        public Color backColor = new(50, 50, 50, 255);
        public Dictionary<int, Color> backColors = new();
        public Action click;
        public Color fontColor = new(192, 192, 198, 255);
        public Dictionary<int, Color> fontColors = new();
        public Func<int, string> itemProcessing;
        public Action outsideClick;
        public string tooltip = "";
        public bool showTooltip = true;
        public Sound clickSound;
        public bool randomPitch = true;

        private float _padding = 5;
        private Rectangle _bounds;
        private Scrollbar _bar;
        private List<Label> _labels = new();
        private int _labelHeight;
        private Action<int> _individualClick;
        private int _itemsToShow;
        private int _lastLength;
        private float _lastValue;

        public ListView(Vector2 pos, int width, Func<int, string> itemProcessing, Func<int> arrayLength,
            int itemsToShow = 10, int labelHeight = 40, float padding = 5f) : base(pos)
        {
            (this.itemProcessing, this.arrayLength, _padding, _itemsToShow, _labelHeight) =
                (itemProcessing, arrayLength, padding, itemsToShow, labelHeight);
            var height = CalcHeight();
            _bar = new Scrollbar(new Rectangle(pos.X, pos.Y, 18, height))
            {
                barScale = 2,
                amountInvoke = () => this.arrayLength.Invoke() + 1 - _itemsToShow
            };
            _bounds = new Rectangle(pos.X + 20, pos.Y, width - 20, height);
            for (var i = 0; i < itemsToShow + 1; i++)
                _labels.Add(new Label(new Rectangle(0, 0, _bounds.width, labelHeight)));
            _bar.OnMoveEvent += UpdateLabels;
            initPosition = _bounds.Pos();
            UpdateLabels(0);
        }

        private void UpdateLabels(float value)
        {
            var strictVal = (int)value;
            var labelPadding = _labelHeight + _padding;
            var y = _bounds.Pos().Y - labelPadding * (value - strictVal);
            // Console.WriteLine($"start y = {y} | _labels.Count , {leng} - {strictVal}");
            foreach (var l in _labels) l.backColor = l.fontColor = Transparent;

            for (var i = 0; i < Math.Min(_labels.Count, arrayLength.Invoke() - strictVal); i++)
            {
                var notI = i;
                var l = _labels[i];
                l.text = this[strictVal + i];
                if (_individualClick is not null) l.getId = () => strictVal + notI;
                l.NewPos(new Vector2(_bounds.x, y + labelPadding * i));
                l.backColor = backColors.ContainsKey(strictVal + i) ? backColors[strictVal + i] : backColor;
                l.fontColor = fontColors.ContainsKey(strictVal + i) ? fontColors[strictVal + i] : fontColor;
            }
        }

        private void UpdateText()
        {
            var value = _bar.Value;
            for (var i = 0; i < Math.Min(_labels.Count, arrayLength.Invoke() - (int)value); i++)
                _labels[i].text = this[(int)value + i];
        }

        public override void Update()
        {
            _bar.Update();
            if (_lastLength != arrayLength.Invoke() || _lastValue != _bar.Value)
            {
                (_lastLength, _lastValue) = (arrayLength.Invoke(), _bar.Value);
                UpdateLabels(Value);
            }

            if (_bounds.ExtendPos(new Vector2(20, 0)).IsMouseIn())
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
                if (!_labels.Any(l => l.isMouseIn)) return;
                _individualClick?.Invoke(_labels.Where(l => l.isMouseIn).First().getId.Invoke());
                return;
            }

            if (!IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON)) return;
            outsideClick?.Invoke();
        }

        protected override void RenderCall()
        {
            UpdateText();
            if (arrayLength.Invoke() > _itemsToShow) _bar.Render();
            _bounds.MaskDraw(() =>
            {
                foreach (var l in _labels) l.Render();
            });
            if (showTooltip && tooltip != "") _bounds.ExtendPos(new Vector2(20, 0)).DrawTooltip(tooltip);
        }

        public override void PositionChange(Vector2 v2)
        {
            _bar.MoveTo(v2);
            _bar.MoveBy(new Vector2(-20, 0));
            _bounds = _bounds.MoveTo(v2);
            UpdateLabels(_bar.Value);
        }

        public float CalcHeight() => (_labelHeight + _padding) * _itemsToShow - _padding;
        public string this[int idx] => itemProcessing.Invoke(idx);
    }
}