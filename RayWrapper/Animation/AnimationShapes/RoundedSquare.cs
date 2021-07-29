using System.Numerics;
using Raylib_cs;

namespace RayWrapper.Animation.AnimationShapes
{
    public class RoundedSquare : AnimationShape
    {
        public Color color;
        public Color lineColor;
        public float roundness = .5f;
        public int lineThickness = 5;
        public bool lines;

        public RoundedSquare(string id) : this(id, Color.LIGHTGRAY)
        {
        }

        public RoundedSquare(string id, Color color) : base(id) => this.color = color;

        public override void DrawShape()
        {
            var rect = RectWrapper.AssembleRectFromVec(pos, size);

            rect.DrawRounded(color, roundness);
            if (lines) rect.DrawRoundedLines(lineColor, roundness, lineThickness);
        }

        public override AnimationShape CopyState() =>
            new RoundedSquare(id, color)
            {
                pos = new Vector2(pos.X, pos.Y), size = new Vector2(size.X, size.Y), isVisible = isVisible,
                roundness = roundness, lines = lines, lineThickness = lineThickness, lineColor = lineColor
            };
    }
}