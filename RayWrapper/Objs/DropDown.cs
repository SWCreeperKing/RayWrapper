using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    public class DropDown : GameObject
    {
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                UpdateChanges();
            }
        }

        public char arrowDown = '↓';
        public char arrowUp = '↑';
        public int fontSize = 24;
        public bool isListVisible;
        public Action<string, int> onChange = null; // op, val
        public ListView optionDisplay;
        public List<string> options;
        public Label text;

        private Vector2 _size;
        private int _value;

        public DropDown(Vector2 pos, params string[] options) : base(pos)
        {
            this.options = options.ToList();
            UpdateChanges();
        }

        public void UpdateChanges()
        {
            var longest = options.OrderByDescending(s => s.Length).First();
            _size = GameBox.Font.MeasureText($"^|||y{longest}", fontSize);
            var back = RectWrapper.AssembleRectFromVec(Position, _size).Grow(4);
            text = new Label(back, options[Value]) {fontSize = fontSize, outline = new Actionable<bool>(true)};

            optionDisplay = new(new Vector2(back.x, back.y + back.height + 2),
                (int) back.width,
                i => options[i],
                () => options.Count, 4, padding:0);

            optionDisplay.IndividualClick = i =>
            {
                Value = i;
                onChange?.Invoke(options[i], i);
                isListVisible = false;
            };

            optionDisplay.outsideClick = () =>
            {
                if (isListVisible && !text.isMouseIn) isListVisible = false;
            };

            text.clicked = () => isListVisible = !isListVisible;
        }

        public override void Update()
        {
            text.Update();
            if (isListVisible) optionDisplay.Update();
        }

        protected override void RenderCall()
        {
            if (isListVisible) optionDisplay.Render();
            text.text = $"{(isListVisible ? arrowUp : arrowDown)}| {options[Value]}";
            text.Render();
        }

        public override void PositionChange(Vector2 v2)
        {
            text.MoveTo(v2);
            optionDisplay.MoveTo(v2);
            optionDisplay.MoveBy(new Vector2(0, text.back.height + 2));
        }

        public override Vector2 Size() => optionDisplay.Size() + text.Size();
    }
}