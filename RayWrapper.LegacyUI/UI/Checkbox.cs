using System.Numerics;
using RayWrapper.Base.GameBox;
using RayWrapper.Base.GameObject;
using RayWrapper.Base.Primitives;
using static Raylib_CsLo.MouseCursor;
using static Raylib_CsLo.Raylib;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.LegacyUI.UI;

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
    
    public Style style = defaultStyle.Copy();
    public Action<bool> checkChange;
    public bool isChecked;

    private readonly Vector2 _posOff = new(5);
    private readonly Vector2 _size = new(20);
    private readonly Vector2 _textOff = new(35, 5);

    private Vector2 _textPos;
    private Vector2 _textBox;
    private string _text;
    private float _textLength;

    public Checkbox(Vector2 pos, string text = "Untitled Checkbox")
    {
        this.pos = pos;
        _text = text;
        _textPos = pos + _textOff;
        Text = text;
    }

    protected override void UpdateCall(float dt)
    {
        var rect = new Rectangle(Position, _textBox); 
        if (!rect.IsMouseIn() || !IsMouseButtonPressed(MOUSE_LEFT_BUTTON)) return;
        isChecked = !isChecked;
        checkChange?.Invoke(isChecked);
    }

    protected override void RenderCall()
    {
        var rect = new Rectangle(Position + _posOff, _size);
        var rect2 = new Rectangle(Position, _textBox);
        var mouseIsIn = rect2.IsMouseIn() && !Input.IsMouseOccupied;

        style.Draw(_text, _textPos, isChecked, mouseIsIn, rect.Grow(3), rect.Shrink(3));
        if (rect2.IsMouseIn()) GameBox.SetMouseCursor(MOUSE_CURSOR_POINTING_HAND);
    }
    
    protected override Vector2 GetSize() => _textBox;

    protected override void UpdatePosition(Vector2 newPos)
    {
        base.UpdatePosition(newPos);
        _textPos = newPos + _textOff;
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