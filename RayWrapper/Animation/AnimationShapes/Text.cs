using System;
using Raylib_cs;

namespace RayWrapper.Animation.AnimationShapes
{
    public class Text : AnimationShape
    {
        public string text;
        public Color color;
        public int fontSize;
        public Func<string> setText;

        public Text(string id, string text) : base(id) => this.text = text;

        public override void DrawShape() => GameBox.Font.DrawText(setText?.Invoke() ?? text, pos, color);

        public override AnimationShape CopyState() =>
            new Text(id, text) {pos = pos, color = color, fontSize = fontSize, isVisible = isVisible, setText = setText};
    }
}