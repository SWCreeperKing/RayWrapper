using System;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace RayWrapper.Objs
{
    public class Label : IGameObject
    {
        public enum TextMode
        {
            AlignLeft,
            AlignCenter,
            AlignRight,
            SizeToText,
            WrapText
        }

        public Rectangle back;
        public Color backColor = new(50, 50, 50, 255);
        public Color fontColor = new(192, 192, 198, 255);
        public TextMode textMode;
        public string text;
        public float fontSize = 24;
        public float spacing = 1.5f;

        public Label(Rectangle back, string text = "Untitled Label", TextMode textMode = TextMode.AlignLeft) =>
            (this.back, this.text, this.textMode) = (back, text, textMode);

        public void Update()
        {
        }

        public void Render()
        {
            var adjust = back.Shrink(4);
            var textSize = MeasureTextEx(GameBox.font, text, fontSize, spacing);

            void DrawTxt(Vector2 pos) => DrawTextEx(GameBox.font, text, pos, fontSize, spacing, fontColor);

            if (textMode != TextMode.SizeToText) back.Draw(backColor);

            switch (textMode)
            {
                case TextMode.AlignLeft:
                    DrawTxt(new(adjust.x, adjust.y + (adjust.height / 2 - textSize.Y / 2)));
                    break;
                case TextMode.AlignCenter:
                    GameBox.font.DrawCenterText(adjust.Center(), text, fontColor, fontSize, spacing);
                    break;
                case TextMode.AlignRight:
                    DrawTxt(new(adjust.x + adjust.width - textSize.X, adjust.y + (adjust.height / 2 - textSize.Y / 2)));
                    break;
                case TextMode.WrapText:
                    DrawTextRec(GameBox.font, text, adjust, fontSize, spacing, true, fontColor);
                    break;
                case TextMode.SizeToText:
                    var rect = RayWrapper.AssembleRectFromVec(adjust.Pos(), textSize);
                    rect.Grow(4).Draw(backColor);
                    DrawTxt(adjust.Pos());
                    break;
            }
        }

        public void NewPos(Vector2 pos) => back = new(pos.X, pos.Y, back.width, back.height);
    }
}