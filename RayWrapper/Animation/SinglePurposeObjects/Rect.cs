using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;

namespace RayWrapper.Animation.SinglePurposeObjects
{
    /// <summary>
    /// Mostly just use for transitions :p
    /// </summary>
    public class Rect : GameObject, ISizeable, IAlphable
    {
        public override Vector2 Position
        {
            get => rect.Pos();
            set => rect.MoveTo(value);
        }

        public override Vector2 Size => rect.Size();

        public ColorModule ColorMod
        {
            get => ((Color)_color).SetAlpha(alpha);
            set => (_color, alpha) = (value, ((Color)value).a);
        }      
        
        public ColorModule OutlineMod
        {
            get => ((Color)_outlineColor).SetAlpha(alpha);
            set => (_outlineColor, alpha) = (value, ((Color)value).a);
        }
        
        public int alpha = 255;
        public bool outline;
        public int outlineThickness = 3;
        public bool isRound;
        public float roundness = .5f;
        public Rectangle rect;
        
        private ColorModule _color = Color.WHITE;
        private ColorModule _outlineColor = Color.BLACK;

        public Rect(Rectangle rect) => this.rect = rect;

        protected override void UpdateCall()
        {
        }

        protected override void RenderCall()
        {
            if (isRound)
            {
                rect.DrawRounded(ColorMod, roundness);
                if (outline) rect.DrawRoundedLines(OutlineMod, roundness, outlineThickness);
            }
            else
            {
                rect.Draw(ColorMod);
                if (outline) rect.DrawHallowRect(OutlineMod, outlineThickness);
            }
        }

        public void SetSize(Vector2 size) => rect = rect.SetSize(size);
        public void AddSize(Vector2 size) => SetSize(Size + size);

        public static implicit operator Rectangle(Rect rect) => rect.rect;
        public static implicit operator Rect(Rectangle rect) => new(rect);
        public int GetAlpha() => alpha;
        public void SetAlpha(int alpha) => this.alpha = alpha;
    }
}