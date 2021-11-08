using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;

namespace RayWrapper.Animation.SinglePerposeObjects
{
    /// <summary>
    /// Mostly just use for transitions :p
    /// </summary>
    public class Rect : GameObject, ISizeable
    {
        public override Vector2 Position
        {
            get => rect.Pos();
            set => rect = rect.MoveTo(value);
        }

        public override Vector2 Size => rect.Size();

        public ColorModule color = Color.WHITE;
        public ColorModule outlineColor = Color.BLACK;
        public bool outline;
        public int outlineThickness = 3;
        public bool isRound;
        public float roundness = .5f;
        public Rectangle rect;

        public Rect(Rectangle rect) => this.rect = rect;

        protected override void UpdateCall()
        {
        }

        protected override void RenderCall()
        {
            if (isRound)
            {
                rect.DrawRounded(color, roundness);
                if (outline) rect.DrawRoundedLines(outlineColor, roundness, outlineThickness);
            }
            else
            {
                rect.Draw(color);
                if (outline) rect.DrawHallowRect(outlineColor, outlineThickness);
            }
        }

        public void SetSize(Vector2 size) => rect = rect.SetSize(size);
        public void AddSize(Vector2 size) => SetSize(Size + size);

        public static implicit operator Rectangle(Rect rect) => rect.rect;
        public static implicit operator Rect(Rectangle rect) => new(rect);
    }
}