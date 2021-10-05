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

        public KeyButton(Vector2 pos, KeyboardKey key)
        {
            this.key = key;
            core = new Button(RectWrapper.AssembleRectFromVec(pos, new Vector2(0, 0)), $" {this.key.GetString()} ",
                Label.TextMode.SizeToText)
            {
                Text = new Actionable<string>("", () => !_acceptingChange ? $" {this.key.GetString()} " : " _ ")
            };
            core.Clicked += () => _acceptingChange = !_acceptingChange;
        }

        public override Vector2 Position
        {
            get => core.Position;
            set => core.Position = value;
        }

        public override Vector2 Size => core.Size;

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
    }
}