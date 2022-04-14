using System;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Var_Interfaces;
using RayWrapper.Vars;
using static RayWrapper.GameBox;

namespace RayWrapper.Objs;

public abstract class AlertBase : GameObject
{
    public static Style defaultStyle = new();

    public override Vector2 Position { get; set; }
    public override Vector2 Size { get; }

    public Style style = defaultStyle.Copy();
    public string title;
    public string message;
    public Action<string> onResult;

    private Vector2 _titleSize;
    private Vector2 _messageSize;
    private Rectangle _rect;

    protected AlertBase(string title, string message)
    {
        this.title = title;
        this.message = message;
        style.title.drawMode = Text.Style.DrawMode.Center;
        style.message.drawMode = Text.Style.DrawMode.Center;

        _titleSize = style.title.MeasureText(title);
        _messageSize = style.message.MeasureText(message);
        var baseBackSize = new Vector2(Math.Max(_titleSize.X, _messageSize.X), _titleSize.Y + _messageSize.Y + 14);

        Init();
        var afterBackSize = baseBackSize + AddedBackSize();
        var (midX, midY) = (WindowSize / 2).Deconstruct();
        var (halfX, halfY) = (afterBackSize / 2).Deconstruct();
        _rect = new Rectangle(midX - halfX, midY - halfY, halfX * 2, halfY * 2).Grow(4);
    }

    /// <summary>
    /// stuff needs to init rather than constructor since base constructor runs first
    /// </summary>
    public abstract void Init();
    public abstract Vector2 AddedBackSize();
    public abstract void UpdateAdded();
    public abstract void RenderAdded(Vector2 startPos, Vector2 size);
    public abstract string ResultMessage();

    protected override void UpdateCall() => UpdateAdded();

    protected override void RenderCall()
    {
        var shrink = _rect.Shrink(4);
        var rectPos = shrink.Pos();

        style.back.Draw(_rect);
        style.title.Draw(title, new Rectangle(rectPos.X, rectPos.Y, shrink.width, _titleSize.Y).Center());

        rectPos.Y += _titleSize.Y + 7;
        style.message.Draw(message, new Rectangle(rectPos.X, rectPos.Y, shrink.width, _messageSize.Y).Center());

        rectPos.Y += _messageSize.Y + 9;
        RenderAdded(rectPos, shrink.Size());
    }

    public void Show() => alertQueue.Push(this);

    public void Hide()
    {
        alertQueue.Pop();
        onResult?.Invoke(ResultMessage());
    }

    public class Style : IStyle<Style>
    {
        public Text.Style title = new() { fontSize = 30, color = new ColorModule(70, 170, 70) };
        public Text.Style message = new() { color = new ColorModule(70, 140, 140) };
        public RectStyle back = new() { color = new ColorModule(60, 60, 60) };

        public Style Copy()
        {
            return new Style
            {
                title = title.Copy(), message = message.Copy(), back = back.Copy()
            };
        }
    }
}