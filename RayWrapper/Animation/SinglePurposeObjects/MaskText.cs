using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;

namespace RayWrapper.Animation.SinglePurposeObjects
{
    /// <summary>
    /// Mostly just use for transitions :p
    /// </summary>
    public class MaskText : GameObject, ISizeable, IAlphable
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

        public int alpha = 255;
        public Actionable<string> text;
        public Rectangle rect;
        public Actionable<string> tooltip = null;
        
        private ColorModule _color = Color.WHITE;
        
        public MaskText(Actionable<string> text, Rectangle rect) => (this.text, this.rect) = (text, rect);
        public MaskText(Rectangle rect) => this.rect = rect;

        protected override void UpdateCall()
        {
        }

        protected override void RenderCall()
        {
            var color = (Color)ColorMod;
            rect.Grow(2).MaskDraw(() =>
                GameBox.Font.DrawText(text, rect.Pos(), color.a != alpha ? color.SetAlpha(alpha) : color));
            if (tooltip is not null) rect.DrawTooltip(tooltip);
        }

        public void SetSize(Vector2 size) => rect = rect.SetSize(size);
        public void AddSize(Vector2 size) => SetSize(Size + size);
        public static implicit operator MaskText(Rect r) => new(r);
        public int GetAlpha() => alpha;
        public void SetAlpha(int alpha) => this.alpha = alpha;
    }
}