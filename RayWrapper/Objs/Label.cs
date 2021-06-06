﻿using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static Raylib_cs.Raylib;

namespace RayWrapper.Objs
{
    public class Label : GameObject
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
        public Action clicked;
        public Color fontColor = new(192, 192, 198, 255);
        public float fontSize = 24;
        public Func<int> getId = null;
        public bool isMouseIn;
        public TextMode textMode;
        public string text;
        public float spacing = 1.5f;

        public Label(Rectangle back, string text = "Untitled Label", TextMode textMode = TextMode.AlignLeft) :
            base(back.Pos()) =>
            (this.back, this.text, this.textMode) = (back, text, textMode);

        public override void Update()
        {
            isMouseIn = back.IsMouseIn();
            if (isMouseIn && IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON)) clicked?.Invoke();
        }

        public override void Render()
        {
            var adjust = back.Shrink(4);
            var textSize = MeasureTextEx(GameBox.font, text, fontSize, spacing);

            void DrawTxt(Vector2 pos) => DrawTextEx(GameBox.font, text, pos, fontSize, spacing, fontColor);

            if (textMode != TextMode.SizeToText) back.Draw(backColor);

            switch (textMode)
            {
                case TextMode.AlignLeft:
                    DrawTxt(new Vector2(adjust.x, adjust.y + (adjust.height / 2 - textSize.Y / 2)));
                    break;
                case TextMode.AlignCenter:
                    GameBox.font.DrawCenterText(adjust.Center(), text, fontColor, fontSize, spacing);
                    break;
                case TextMode.AlignRight:
                    DrawTxt(new Vector2(adjust.x + adjust.width - textSize.X,
                        adjust.y + (adjust.height / 2 - textSize.Y / 2)));
                    break;
                case TextMode.WrapText:
                    DrawTextRec(GameBox.font, text, adjust, fontSize, spacing, true, fontColor);
                    break;
                case TextMode.SizeToText:
                    var rect = RectWrapper.AssembleRectFromVec(adjust.Pos(), textSize);
                    rect.Grow(4).Draw(backColor);
                    DrawTxt(adjust.Pos());
                    break;
            }
        }

        public override void PositionChange(Vector2 v2) => back = back.MoveTo(v2);
        public void NewPos(Vector2 pos) => back = new Rectangle(pos.X, pos.Y, back.width, back.height);
    }
}