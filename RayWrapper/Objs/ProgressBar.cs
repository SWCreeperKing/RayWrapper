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
        public Color backColor = Color.BLACK;
        public Color fillColor = Color.RAYWHITE;
        public Func<float> percent;

        public ProgressBar(Rectangle rect, Func<float> percent) : base(rect.Pos())
        {
            this.percent = percent;
            size = rect.Size();
        }

        public override void Update()
        {
        }

        protected override void RenderCall()
        {
            var fill = percent.Invoke();
            var newS = new Vector2(size.X * (isVertical ? 1 : fill), size.Y * (isVertical ? fill : 1));
            AssembleRectFromVec(Position, size).Grow(outlineThickness).Draw(backColor);
            AssembleRectFromVec(Position, newS).Draw(fillColor);
        }

        public override void PositionChange(Vector2 v2)
        {
        }
    }
}