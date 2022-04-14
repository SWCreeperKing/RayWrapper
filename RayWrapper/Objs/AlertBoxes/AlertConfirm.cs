using System;
using System.Numerics;

namespace RayWrapper.Objs;

public class AlertConfirm : AlertBase
{
    private Button _yes;
    private Button _no;
    private string _message;

    public AlertConfirm(string title, string message) : base(title, message)
    {
    }

    public override void Init()
    {
        _yes = new Button(Vector2.Zero, "Yes");
        _yes.Clicked += () =>
        {
            _message = "yes";
            Hide();
        };

        _no = new Button(Vector2.Zero, "No");
        _no.Clicked += () =>
        {
            _message = "no";
            Hide();
        };
    }

    public override Vector2 AddedBackSize()
    {
        return new Vector2(_no.Size.X + _yes.Size.X + 8, Math.Max(_no.Size.Y, _yes.Size.Y));
    }

    public override void UpdateAdded()
    {
        _yes.Update();
        _no.Update();
    }

    public override void RenderAdded(Vector2 startPos, Vector2 size)
    {
        startPos += new Vector2(size.X / 2, 0);

        _yes.Position = startPos - new Vector2(_yes.Size.X + 4, 0);
        _no.Position = startPos + new Vector2(4, 0);

        _yes.Render();
        _no.Render();
    }

    public override string ResultMessage() => _message;
}