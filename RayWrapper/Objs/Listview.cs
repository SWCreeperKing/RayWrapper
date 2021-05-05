using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using static RayWrapper.RayWrapper;

namespace RayWrapper.Objs
{
    public class Listview : IGameObject
    {
        public Func<int, string> itemProcessing;
        public Func<int> arrayLength;
        public Color backColor = new(50, 50, 50, 255);
        public Color fontColor = new(192, 192, 198, 255);

        private Rectangle _bounds;
        private Scrollbar _bar;
        private List<Label> _labels = new();
        private int _labelHeight;
        private int _padding = 5;
        private int _itemsToShow;

        public Listview(Vector2 pos, int width, Func<int, string> itemProcessing, Func<int> arrayLength,
            int itemsToShow = 10, int labelHeight = 40)
        {
            this.itemProcessing = itemProcessing;
            this.arrayLength = arrayLength;
            _itemsToShow = itemsToShow;
            var height = itemsToShow * labelHeight + (itemsToShow - 1) * _padding;
            _bar = new(new(pos.X, pos.Y, 18, height));
            _bounds = new(pos.X + 20, pos.Y, width - 20, height);
            for (var i = 0; i < itemsToShow + 1; i++)
                _labels.Add(new(new(0, 0, _bounds.width, labelHeight)));
            _bar.OnMoveEvent += UpdateLabels;
            _labelHeight = labelHeight;
            UpdateLabels(0);
        }

        public void UpdateLabels(float value)
        {
            _bar.amount = arrayLength.Invoke() + 1 - _itemsToShow;
            var strictVal = (int) value;
            var carry = value - strictVal; // hopefully b/t 1 and 0
            var y = _bounds.Pos().Y - (_labelHeight + _padding) * carry;
            _labels.ForEach(l => l.backColor = l.fontColor = Transparent);
            for (var i = 0; i < _labels.Count && i <= _bar.amount + 1; i++)
            {
                var l = _labels[i];
                l.text = this[strictVal + i];
                l.NewPos(new(_bounds.x, y + (_labelHeight + _padding) * i));
                l.backColor = backColor;
                l.fontColor = fontColor;
            }
        }

        public void Update()
        {
            _bar.Update();
        }

        public void Render()
        {
            _bar.Render();
            _bounds.MaskDraw(() => { _labels.ForEach(l => l.Render()); });
        }

        public string this[int idx] => itemProcessing.Invoke(idx);
    }
}