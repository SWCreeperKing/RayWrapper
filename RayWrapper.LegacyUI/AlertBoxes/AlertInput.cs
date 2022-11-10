using System.Numerics;
using RayWrapper.LegacyUI.UI;

namespace RayWrapper.LegacyUI.AlertBoxes;

public class AlertInput : AlertBase
{
    public int maxCharacters = 40;
    public bool password;
    public bool numbersOnly;

    private InputBox _input;

    public AlertInput(string title, string message) : base(title,
        message)
    {
    }

    public override void Init()
    {
        _input = new InputBox(maxCharacters)
        {
            onEnter = _ => Hide(), password = password, numbersOnly = numbersOnly
        };
        _input.Update(0);
    }

    public override Vector2 AddedBackSize() => _input.Size + new Vector2(48, 0);
    public override void UpdateAdded() => _input.Update(0);

    public override void RenderAdded(Vector2 startPos, Vector2 size)
    {
        _input.Position = startPos + new Vector2(size.X / 2 - (_input.Size.X - 10) / 2, 0);
        _input.Size = size with { X = size.X - 50 };
        _input.Render();
    }

    public override string ResultMessage() => _input.Text;
}