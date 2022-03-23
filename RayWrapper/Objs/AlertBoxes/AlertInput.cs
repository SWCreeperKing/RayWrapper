using System.Numerics;

namespace RayWrapper.Objs
{
    public class AlertInput : AlertBase
    {
        private InputBox _input;
        private int maxCharacterShow = 20;
        private int maxCharacters = 40;

        public AlertInput(string title, string message) : base(title,
            message)
        {
        }

        public override void Init()
        {
            _input = new InputBox(Vector2.Zero, maxCharacterShow, maxCharacters)
            {
                onEnter = _ => Hide()
            };
        }

        public override Vector2 AddedBackSize() => _input.Size;
        public override void UpdateAdded() => _input.Update();

        public override void RenderAdded(Vector2 startPos, Vector2 size)
        {
            _input.Position = startPos + new Vector2(size.X / 2 - _input.Size.X / 2, 0);
            _input.Render();
        }

        public override string ResultMessage() => _input.GetText();
    }
}