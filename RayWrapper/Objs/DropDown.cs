using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Objs.ListView;
using RayWrapper.Vars;
using static Raylib_CsLo.MouseCursor;

namespace RayWrapper.Objs;

public class DropDown : GameObject
{
    public override Vector2 Position
    {
        get => _pos;
        set
        {
            _pos = text.Position = value;
            optionDisplay.Position = value + new Vector2(0, text.Rect.height + 2);
            UpdateChanges();
        }
    }

    public override Vector2 Size => text.Size + new Vector2(0, optionDisplay.Size.Y);

    public char arrowDown = 'v';
    public char arrowUp = '^';
    public int fontSize = 24;
    public bool isListVisible;
    public Action<string, int> onChange = null; // op, val
    public ListView.ListView optionDisplay;
    public List<string> options;
    public Label text;

    private Vector2 _size;
    private Vector2 _pos;
    private int _value;

    public DropDown(Vector2 pos, params string[] options)
    {
        (_pos, this.options) = (pos, options.ToList());
        UpdateChanges();
    }

    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            UpdateChanges();
        }
    }

    public void UpdateChanges()
    {
        var longest = options.OrderByDescending(s => s.Length).First();
        _size = (Text.Style.DefaultFont ?? Raylib.GetFontDefault()).MeasureText($"^|||y{longest}", fontSize);
        var back = RectWrapper.AssembleRectFromVec(Position, _size).Grow(4);
        text = new Label(back, options[Value])
        {
            clicked = () => isListVisible = !isListVisible,
            style =
            {
                backColor = new ColorModule(50),
                fontColor = new ColorModule(192),
                drawHover = true
            }
        };

        var defItem = new DefaultListItem((int) back.width, () => options.Count, i => options[i])
        {
            onClick = (i, _) =>
            {
                Value = i;
                onChange?.Invoke(options[i], i);
                isListVisible = false;
            }
        };
        optionDisplay = new ListView.ListView(new Vector2(back.x, back.y + back.height + 2), defItem, 4, padding: 2)
        {
            outsideClick = () =>
            {
                if (isListVisible && !text.Rect.IsMouseIn()) isListVisible = false;
            }
        };
    }

    protected override void UpdateCall()
    {
        text.Update();
        if (isListVisible) optionDisplay.Update();
    }

    protected override void RenderCall()
    {
        if (isListVisible) optionDisplay.Render();
        if (text.Rect.IsMouseIn()) GameBox.SetMouseCursor(MOUSE_CURSOR_POINTING_HAND);
        text.text = $"{(isListVisible ? arrowUp : arrowDown)}| {options[Value]}";
        text.Render();
    }
}