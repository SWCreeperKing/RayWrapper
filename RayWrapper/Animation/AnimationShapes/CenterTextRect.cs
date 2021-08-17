using System;
using Raylib_cs;

namespace RayWrapper.Animation.AnimationShapes
{
    public class CenterTextRect : AnimationShape
    {
        public string text;
        public Color color;
        public int fontSize;
        public Func<string> setText;

        public CenterTextRect(string id, string text) : base(id) => this.text = text;

        public override void DrawShape() =>
            GameBox.Font.DrawCenterWrapText(RectWrapper.AssembleRectFromVec(pos, size), setText?.Invoke() ?? text,
                color);

        public override AnimationShape CopyState() =>
            new CenterTextRect(id, text)
                {pos = pos, size = size, color = color, fontSize = fontSize, isVisible = isVisible, setText = setText};
    }
}