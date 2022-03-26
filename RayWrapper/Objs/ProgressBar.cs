using System;
using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static Raylib_CsLo.Raylib;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs
{
    public class ProgressBar : GameObject
    {
        public override Vector2 Position
        {
            get => _pos;
            set => _pos = value;
        }

        public override Vector2 Size => AssembleRectFromVec(Position, size).Grow(outlineThickness).Size();

        public ColorModule backColor = new(BLACK);
        public ColorModule fillColor = new(RAYWHITE);
        public ColorModule toColor = new(GOLD);
        public ColorModule finishedColor = new(GREEN);
        public bool hoverPercent = true;
        public bool isVertical;
        public int outlineThickness = 3;
        public Func<float> percent;
        public Vector2 size;
        public bool useGradient = true;
        public Tooltip tooltip;

        private Vector2 _pos;

        public ProgressBar(Rectangle rect, Func<float> percent)
        {
            (this.percent, _pos, size) = (percent, rect.Pos(), rect.Size());
            tooltip = new GameBox.DefaultTooltip(new Actionable<string>(() =>
                percent.Invoke() >= 1 ? "100%" : $"{percent.Invoke():##0.00%}"));
        }

        public ProgressBar(float x, float y, float width, float height, Func<float> percent)
        {
            (this.percent, _pos, size) = (percent, new Vector2(x, y), new Vector2(width, height));
            tooltip = new GameBox.DefaultTooltip(new Actionable<string>(() =>
                percent.Invoke() >= 1 ? "100%" : $"{percent.Invoke():##0.00%}"));
        }

        protected override void UpdateCall()
        {
        }

        protected override void RenderCall()
        {
            var fill = FixedPercent();
            var back = AssembleRectFromVec(Position, size).Grow(outlineThickness);
            if (!useGradient) back.DrawRounded(backColor);
            else back.Draw(backColor);
            if (fill >= 1) AssembleRectFromVec(Position, size).Draw(finishedColor);
            else
            {
                var newS = new Vector2(size.X * (isVertical ? 1 : fill), size.Y * (isVertical ? fill : 1));
                if (!useGradient)
                    AssembleRectFromVec(Position, newS).DrawRounded(((Color) fillColor).Percent(toColor, fill));
                else AssembleRectFromVec(Position, newS).DrawGradient(fillColor, toColor);
            }

            if (!hoverPercent) return;
            tooltip.Draw(back);
        }

        public float FixedPercent()
        {
            var fill = percent.Invoke();
            var fix = fill.IsFixable() ? fill.Fix() : fill;
            return Math.Clamp(fix, 0, 1);
        }
    }
}