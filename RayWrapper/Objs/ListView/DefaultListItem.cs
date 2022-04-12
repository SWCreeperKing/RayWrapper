using System;
using System.Numerics;
using Raylib_CsLo;

namespace RayWrapper.Objs;

public class DefaultListItem : IListItem
{
    public Color backColor = new(70, 70, 70, 255);
    public Color fontColor = new(192, 192, 192, 255);

    public int itemNum;
    public int height = 40;
    public Action<int, string> onClick;
    public Func<int> arraySize;
    public Func<int, string> item;
    public Func<int, Color> backColorLookup;
    public Func<int, Color> fontColorLookup;

    public Label.Style labelStyle = new() { drawHover = true };

    private Vector2 _itemSize;
    private Vector2 _itemSizeReal;

    public DefaultListItem(int width, Func<int> arraySize, Func<int, string> item)
    {
        this.arraySize = arraySize;
        this.item = item;
        _itemSizeReal = new Vector2(width, height);
        _itemSize = new Vector2(width - 20, height); // account for scrollbar 
    }

    public int ListSize() => arraySize.Invoke();
    public Vector2 ItemSize() => _itemSizeReal;

    public void Render(Vector2 offset, int item)
    {
        labelStyle.fontColor = fontColorLookup?.Invoke(item) ?? fontColor;
        labelStyle.backColor = backColorLookup?.Invoke(item) ?? backColor;

        itemNum = item;
        var s = this.item.Invoke(item);
        var rect = RectWrapper.AssembleRectFromVec(offset, _itemSize);
        labelStyle.Draw(s, rect);
        if (rect.IsMouseIn() && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) onClick?.Invoke(item, s);
    }
}