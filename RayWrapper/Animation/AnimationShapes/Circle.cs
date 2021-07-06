using Raylib_cs;

namespace RayWrapper.Animation.AnimationShapes
{
    public class Circle : AnimationShape
    {
        public Color color;

        public Circle(string id) : this(id, Color.LIGHTGRAY)
        {
        }

        public Circle(string id, Color color) : base(id) => this.color = color;

        public override void DrawShape() => RectWrapper.AssembleRectFromVec(pos, size).DrawCircle(color);

        public override AnimationShape CopyState() =>
            new Circle(id, color) {pos = pos, size = size, isVisible = isVisible};
    }
}