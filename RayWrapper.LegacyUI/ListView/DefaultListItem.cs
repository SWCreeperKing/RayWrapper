using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base.GameBox;
using RayWrapper.Base.Primitives;
using RayWrapper.LegacyUI.UI;
using static Raylib_CsLo.MouseCursor;
using Rectangle = RayWrapper.Base.Primitives.Rectangle;

namespace RayWrapper.LegacyUI.ListView;

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
        
        labelStyle.drawColor = (c, b) =>
        {
            if (onClick is null) return c;
            if (!labelStyle.drawHover) return c;
            return b ? c.MakeLighter() : c;
        };
    }

    public int ListSize() => arraySize();
    public Vector2 ItemSize() => _itemSizeReal;

    public void Render(Vector2 offset, int item, bool isMouseActive)
    {
        labelStyle.fontColor = fontColorLookup?.Invoke(item) ?? fontColor;
        labelStyle.backColor = backColorLookup?.Invoke(item) ?? backColor;

        itemNum = item;
        var s = this.item(item);
        var rect = new Rectangle(offset, _itemSize);
        labelStyle.Draw(s, rect);
        
        if (!isMouseActive || !rect.IsMouseIn() || onClick is null) return; 
        GameBox.SetMouseCursor(MOUSE_CURSOR_POINTING_HAND);
        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) onClick?.Invoke(item, s);
    }
}