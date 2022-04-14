using System;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Var_Interfaces;
using RayWrapper.Vars;
using static Raylib_CsLo.MouseCursor;
using static Raylib_CsLo.Raylib;
using static RayWrapper.GameBox;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs;

public class Checkbox : GameObject
{
    public static Style defaultStyle = new();

    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            _textLength = style.textStyle.MeasureText(_text).X;
            _textBox = new Vector2(35 + _textLength, 40);
        }
    }

    public override Vector2 Position
    {
        get => _pos;
        set
        {
            _pos = value;
            _textPos = value + _textOff;
        }
    }

    public override Vector2 Size => _textBox;

    public Style style = defaultStyle.Copy();
    public Action<bool> checkChange;
    public bool isChecked;

    private readonly Vector2 _posOff = new(5);
    private readonly Vector2 _size = new(20);
    private readonly Vector2 _textOff = new(35, 5);

    private Vector2 _pos;
    private Vector2 _textPos;
    private Vector2 _textBox;
    private string _text;
    private float _textLength;

    public Checkbox(Vector2 pos, string text = "Untitled Checkbox")
    {
        _pos = pos;
        _text = text;
        _textPos = pos + _textOff;
        Text = text;
    }

    protected override void UpdateCall()
    {
        var rect = AssembleRectFromVec(Position, _textBox); 
        if (!rect.IsMouseIn() || !IsMouseButtonPressed(MOUSE_LEFT_BUTTON)) return;
        isChecked = !isChecked;
        checkChange?.Invoke(isChecked);
    }

    protected override void RenderCall()
    {
        var rect = AssembleRectFromVec(Position + _posOff, _size);
        var rect2 = AssembleRectFromVec(Position, _textBox);
        var mouseIsIn = rect2.IsMouseIn() && !IsMouseOccupied;

        style.Draw(_text, _textPos, isChecked, mouseIsIn, rect.Grow(3), rect.Shrink(3));
        if (rect2.IsMouseIn()) GameBox.SetMouseCursor(MOUSE_CURSOR_POINTING_HAND);
    }

    public class Style : IStyle<Style>
    {
        public RectStyle check = new() { color = new ColorModule(1, 193, 1) };
        public OutlineStyle outsideBase = new() { color = new ColorModule(170) };
        public OutlineStyle outsideHover = new() { color = new ColorModule(255) };
        public Text.Style textStyle = new() { color = new ColorModule(192) };

        public void Draw(string text, Vector2 textPos, bool isChecked, bool mouseIsIn, Rectangle outside,
            Rectangle inside)
        {
            if (isChecked) check.Draw(inside);
            if (mouseIsIn) outsideHover.Draw(outside);
            else outsideBase.Draw(outside);
            textStyle.Draw(text, textPos);
        }

        public Style Copy()
        {
            return new Style
            {
                check = check.Copy(), outsideBase = outsideBase.Copy(), outsideHover = outsideHover.Copy(),
                textStyle = textStyle.Copy()
            };
        }
    }
}