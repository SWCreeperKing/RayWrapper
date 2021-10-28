using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    /// <summary>
    /// Mostly just use for transitions :p
    /// </summary>
    public class MaskText : GameObject, ISizeable
    {
        public override Vector2 Position
        {
            get => rect.Pos();
            set => rect = rect.MoveTo(value);
        }

        public override Vector2 Size => rect.Size();

        public ColorModule color = Color.WHITE;
        public Actionable<string> text;
        public Rectangle rect;
        public Actionable<string> tooltip = null;

        public MaskText(Actionable<string> text, Rectangle rect) => (this.text, this.rect) = (text, rect);
        public MaskText(Rectangle rect) => this.rect = rect;

        protected override void UpdateCall()
        {
        }

        protected override void RenderCall()
        {
            rect.Grow(2).MaskDraw(() => GameBox.Font.DrawText(text, rect.Pos(), color));
            if (tooltip is not null) rect.DrawTooltip(tooltip); 
        }

        public void SetSize(Vector2 size) => rect = rect.SetSize(size);
        public void AddSize(Vector2 size) => SetSize(Size + size);

        public static implicit operator MaskText(Rect r) => new(r);
    }
}