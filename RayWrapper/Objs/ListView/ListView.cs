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

    public Style style = defaultStyle.Copy();
    public Sound clickSound;
    public IListItem itemTemplate;
    public bool randomPitch = true;
    public Action click;
    public Action outsideClick;
    public Tooltip tooltip;

    private readonly Vector2 _itemSize;
    private readonly Scrollbar _bar;
    private readonly float _padding = 5;
    private readonly int _itemsToShow;
    private Rectangle _bounds;

    public ListView(Vector2 pos, IListItem template, int itemsToShow = 10, float padding = 5f)
    {
        (_itemSize, itemTemplate, _padding, _itemsToShow) = (template.ItemSize(), template, padding, itemsToShow);
        var height = GetHeight();
        _bar = new Scrollbar(new Rectangle(pos.X, pos.Y, 18, height))
        {
            amountInvoke = () => itemTemplate.ListSize() + 1 - _itemsToShow
        };
        _bounds = new Rectangle(pos.X + 20, pos.Y, _itemSize.X - 20, height);
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
        var labelPadding = _itemSize.Y + _padding;
        var y = _bounds.Pos().Y - labelPadding * (value - strictVal);
        var mouseActive = _bounds.IsMouseIn();

        for (var i = 0; i < Math.Min(_itemsToShow + 1, itemTemplate.ListSize() - strictVal); i++)
        {
            itemTemplate.Render(new Vector2(_bounds.x, y + labelPadding * i), strictVal + i, mouseActive);
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
                _bar.MoveBar(scroll * (((_itemSize.Y + _padding) * ((float) a / _itemsToShow) - _padding) /
                                       (((_itemSize.Y + _padding) * a - _padding) / _bar.Size.X)));
                
                _bar.Update();
            }

            if (!IsMouseButtonPressed(MOUSE_LEFT_BUTTON)) return;
            click?.Invoke();
            if (randomPitch) SetSoundPitch(clickSound, GameBox.Random.Next(.75f, 1.25f));
            PlaySound(clickSound);
            return;
        }

        if (!IsMouseButtonPressed(MOUSE_LEFT_BUTTON) || _bounds.ExtendPos(new Vector2(20, 0)).IsMouseIn()) return;
        outsideClick?.Invoke();
    }

    protected override void RenderCall()
    {
        _bounds.MaskDraw(DrawTemplate);

        if (itemTemplate.ListSize() > _itemsToShow) _bar.Render();
        tooltip?.Draw(_bounds.ExtendPos(new Vector2(20, 0)));
    }

    protected override void UpdatePosition(Vector2 newPos)
    {
        _bar.Position = newPos;
        _bounds = new Rectangle(newPos.X + 20, newPos.Y, _bounds.width, _bounds.height);
    }

    protected override void UpdatedSize(Vector2 newSize)
    {
        var height = GetHeightWithPadding(GetItemShowFromHeight(newSize.Y), _padding);

        _bar.Size = new Vector2(20, height);
        _bounds = new Rectangle(_bounds.X, _bounds.Y, newSize.X - 20, height);
    }

    public int GetItemShowFromHeight(float height)
    {
        var cutH = height - _itemSize.Y;
        var paddingAndH = _padding + _itemSize.Y;

        if (cutH < paddingAndH) return 1;

        var rawItemShow = (int) Math.Truncate(cutH / paddingAndH);
        return rawItemShow + 1;
    }

    public float GetHeight() => GetHeightWithPadding(_itemsToShow, _padding);
    public float GetHeightWithPadding(int itemShow, float padding) => (_itemSize.Y + padding) * itemShow - padding;

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