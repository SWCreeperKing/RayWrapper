using System;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Base;
using RayWrapper.Base.Gameobject;

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
        RegisterGameObj(core);
    }

    protected override void UpdateCall()
    {
        if (!_acceptingChange) return;
        var pressed = Raylib.GetKeyPressed();
        if (pressed == 0) return;
        key = (KeyboardKey) pressed;
        keyChange.Invoke(key);
        _acceptingChange = false;
    }

    protected override Vector2 GetPosition() => core.Position;
    protected override Vector2 GetSize() => core.Size;
    protected override void UpdatePosition(Vector2 newPos) => core.Position = newPos;
    protected override void UpdateSize(Vector2 newSize) => core.Size = newSize;
}