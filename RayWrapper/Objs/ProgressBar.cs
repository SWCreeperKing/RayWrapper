using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs
{
    public class ProgressBar : GameObject
    {
        public Vector2 size;
        public bool isVertical;
        public int outlineThickness = 3;
        public ColorModule backColor = new(Color.BLACK);
        public ColorModule fillColor = new(Color.RAYWHITE);
        public ColorModule toColor = new(Color.GOLD);
        public ColorModule finishedColor = new(Color.GREEN);
        public Func<float> percent;
        public bool hoverPercent = true;
        public bool useGradient = true;

        public ProgressBar(Rectangle rect, Func<float> percent) : base(rect.Pos()) => (this.percent, size) = (percent, rect.Size());

        public ProgressBar(float x, float y, float width, float height, Func<float> percent) : base(new Vector2(x, y)) =>
            (this.percent, size) = (percent, new Vector2(width, height));

        public override void Update()
        {
        }

        protected override void RenderCall()
        {
            var fill = percent.Invoke();
            var back = AssembleRectFromVec(Position, size).Grow(outlineThickness);
            if (!useGradient) back.DrawRounded(backColor);
            else back.Draw(backColor);
            if (fill >= 1) AssembleRectFromVec(Position, size).Draw(finishedColor);
            else
            {
                var newS = new Vector2(size.X * (isVertical ? 1 : fill), size.Y * (isVertical ? fill : 1));
                if (!useGradient)
                    AssembleRectFromVec(Position, newS).DrawRounded(((Color)fillColor).Percent(toColor, fill));
                else AssembleRectFromVec(Position, newS).DrawGradiant(fillColor, toColor);
            }

            if (hoverPercent) back.DrawTooltip(fill >= 1 ? "100%" : $"{fill:##0.00%}");
        }

        public override void PositionChange(Vector2 v2)
        {
        }

        public override Vector2 Size() => AssembleRectFromVec(Position, size).Grow(outlineThickness).Size();
    }
}