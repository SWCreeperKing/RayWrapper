using System;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;

namespace RayWrapper.Objs;

public class KeyButton : GameObject
{
    public static Button.Style defaultStyle = new();

    public Button.Style Style => core.style;

    public Button core;
    public KeyboardKey key;
    public Action<KeyboardKey> keyChange;

    private bool _acceptingChange;

    public KeyButton(Vector2 pos, KeyboardKey key)
    {
        this.key = key;
        core = new Button(RectWrapper.AssembleRectFromVec(pos, new Vector2(0, 0)), $" {this.key.GetString()} ",
            Label.Style.DrawMode.SizeToText)
        {
            Text = new Actionable<string>(string.Empty,
                () => !_acceptingChange ? $" {this.key.GetString()} " : " _ ")
        };
        core.SetStyle(defaultStyle.Copy());
        core.Clicked += () => _acceptingChange = !_acceptingChange;
    }

    public override Vector2 Position
    {
        get => core.Position;
        set => core.Position = value;
    }

    public override Vector2 Size => core.Size;

    protected override void UpdateCall()
    {
        core.Update();
        if (!_acceptingChange) return;
        var pressed = Raylib.GetKeyPressed();
        if (pressed == 0) return;
        key = (KeyboardKey) pressed;
        keyChange.Invoke(key);
        _acceptingChange = false;
    }

    protected override void RenderCall() => core.Render();
}