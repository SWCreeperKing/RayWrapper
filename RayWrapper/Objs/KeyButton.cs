using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    public class KeyButton : GameObject
    {
        public Button core;
        public KeyboardKey key;
        public Action<KeyboardKey> keyChange;

        private bool _acceptingChange;

        public KeyButton(Vector2 pos, KeyboardKey key) : base(pos)
        {
            this.key = key;
            core = new Button(RectWrapper.AssembleRectFromVec(pos, new Vector2(0, 0)), $" {this.key.GetString()} ",
                Button.ButtonMode.SizeToText)
            {
                text = new Actionable<string>("", () => !_acceptingChange ? $" {this.key.GetString()} " : " _ ")
            };
            core.Clicked += () => _acceptingChange = !_acceptingChange;
        }

        public override void Update()
        {
            core.Update();
            if (!_acceptingChange) return;
            var pressed = Raylib.GetKeyPressed();
            if (pressed == 0) return;
            key = (KeyboardKey)pressed;
            keyChange.Invoke(key);
            _acceptingChange = false;
        }

        protected override void RenderCall() => core.Render();

        public override void PositionChange(Vector2 v2)
        {
        }

        public override Vector2 Size() => core.Size();
    }
}