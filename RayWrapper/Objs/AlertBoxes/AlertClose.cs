using System.Numerics;

namespace RayWrapper.Objs
{
    public class AlertClose : AlertBase
    {
        private Button _close;

        public AlertClose(string title, string message) : base(title, message)
        {
        }

        public override void Init()
        {
            _close = new Button(Vector2.Zero, "Close");
            _close.Clicked += Hide;
        }

        public override Vector2 AddedBackSize() => _close.Size;
        public override void UpdateAdded() => _close.Update();

        public override void RenderAdded(Vector2 startPos, Vector2 size)
        {
            _close.Position = startPos + new Vector2(size.X/2 - _close.Size.X/2, 0);
            _close.Render();
        }

        public override string ResultMessage() => "closed";
    }
}