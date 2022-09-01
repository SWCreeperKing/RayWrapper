using System;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Var_Interfaces;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;

namespace RayWrapper.Objs.ListView;

public class ListView : GameObject
{
    public static Style defaultStyle = new();

    public override Vector2 Position
    {
        get => _bounds.Pos();
        set
        {
            _bar.Position = value + new Vector2(-20, 0);
            _bounds.MoveTo(value);
        }
    }

    public override Vector2 Size => _bounds.Size();

    public Style style = defaultStyle.Copy();
    public Sound clickSound;
    public IListItem itemTemplate;
    public bool randomPitch = true;
    public Action click;
    public Action outsideClick;
    public Tooltip tooltip;

    private readonly Vector2 _size;
    private readonly Scrollbar _bar;
    private readonly float _padding = 5;
    private readonly int _itemsToShow;
    private Rectangle _bounds;

    public ListView(Vector2 pos, IListItem template, int itemsToShow = 10, float padding = 5f)
    {
        (_size, itemTemplate, _padding, _itemsToShow) = (template.ItemSize(), template, padding, itemsToShow);
        var height = CalcHeight();
        _bar = new Scrollbar(new Rectangle(pos.X, pos.Y, 18, height))
        {
            amountInvoke = () => itemTemplate.ListSize() + 1 - _itemsToShow
        };
        _bounds = new Rectangle(pos.X + 20, pos.Y, _size.X - 20, height);
    }

    public float Offset
    {
        get => _bar.GetOffset;
        set => _bar.MoveBar(value);
    }

    public float Value => _bar.Value;

    private void DrawTemplate()
    {
        var value = _bar.Value;
        var strictVal = (int) value;
        var labelPadding = _size.Y + _padding;
        var y = _bounds.Pos().Y - labelPadding * (value - strictVal);
        for (var i = 0; i < Math.Min(_itemsToShow + 1, itemTemplate.ListSize() - strictVal); i++)
        {
            var notI = i;
            var place = strictVal + notI;

            itemTemplate.Render(new Vector2(_bounds.x, y + labelPadding * i), place);
        }
    }

    protected override void UpdateCall()
    {
        _bar.Update();

        if (_bounds.IsMouseIn())
        {
            var scroll = GetMouseWheelMove();
            if (scroll != 0)
            {
                var a = itemTemplate.ListSize();
                // smooth scrolling, help from chocobogamer#4214
                _bar.MoveBar(scroll * (((_size.Y + _padding) * ((float) a / _itemsToShow) - _padding) /
                                       (((_size.Y + _padding) * a - _padding) / _bar.container.height)));
                _bar.Update();
            }

            if (!IsMouseButtonPressed(MOUSE_LEFT_BUTTON)) return;
            click?.Invoke();
            if (randomPitch) SetSoundPitch(clickSound, GameBox.Random.Next(.75f, 1.25f));
            PlaySound(clickSound);
            return;
        }

        if (!IsMouseButtonPressed(MOUSE_LEFT_BUTTON) ||
            _bounds.ExtendPos(new Vector2(20, 0)).IsMouseIn()) return;
        outsideClick?.Invoke();
    }

    protected override void RenderCall()
    {
        _bounds.MaskDraw(DrawTemplate);

        if (itemTemplate.ListSize() > _itemsToShow) _bar.Render();
        tooltip?.Draw(_bounds.ExtendPos(new Vector2(20, 0)));
    }

    public float CalcHeight() => (_size.Y + _padding) * _itemsToShow - _padding;

    public class Style : IStyle<Style>
    {
        public Scrollbar.Style scrollStyle = new();
        public Label.Style labelStyle = new();
        public ColorModule backColor = new(50);
        public ColorModule fontColor = new(192);
        public ColorModule selectColor = new(60, 60, 100);

        public Style Copy()
        {
            return new Style
            {
                scrollStyle = scrollStyle.Copy(), labelStyle = labelStyle.Copy(), backColor = backColor.Copy(),
                fontColor = fontColor.Copy(), selectColor = selectColor.Copy()
            };
        }
    }
}