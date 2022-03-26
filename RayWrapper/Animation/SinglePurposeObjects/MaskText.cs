using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Objs;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;

namespace RayWrapper.Animation.SinglePurposeObjects
{
    /// <summary>
    /// Mostly just use for transitions :p
    /// </summary>
    public class MaskText : GameObject, ISizeable, IAlphable
    {
        public static Text.Style defaultStyle = new();

        public override Vector2 Position
        {
            get => rect.Pos();
            set => rect.MoveTo(value);
        }

        public override Vector2 Size => rect.Size();

        public ColorModule ColorMod
        {
            get => ((Color) _color).SetAlpha(alpha);
            set => (_color, alpha) = (value, ((Color) value).a);
        }

        public Text.Style style = defaultStyle.Copy();
        public int alpha = 255;
        public Actionable<string> text;
        public Rectangle rect;
        public Tooltip tooltip;

        private ColorModule _color = WHITE;

        public MaskText(Rectangle rect) : this("", rect)
        {
        }

        public MaskText(Actionable<string> text, Rectangle rect)
        {
            (this.text, this.rect) = (text, rect);
            style.color = new ColorModule(() =>
            {
                var color = (Color) ColorMod;
                return color.a != alpha ? color.SetAlpha(alpha) : color;
            });
        }

        protected override void UpdateCall()
        {
        }

        protected override void RenderCall()
        {
            rect.Grow(2).MaskDraw(() => style.Draw(text, rect));
            tooltip?.Draw(rect);
        }

        public void SetSize(Vector2 size) => rect = rect.SetSize(size);
        public void AddSize(Vector2 size) => SetSize(Size + size);
        public static implicit operator MaskText(Rect r) => new(r);
        public int GetAlpha() => alpha;
        public void SetAlpha(int alpha) => this.alpha = alpha;
    }
}