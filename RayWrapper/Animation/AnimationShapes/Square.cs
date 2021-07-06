using System.Numerics;
using Raylib_cs;

namespace RayWrapper.Animation.AnimationShapes
{
    public class Square : AnimationShape
    {
        public Color color;

        public Square(string id) : this(id, Color.LIGHTGRAY)
        {
        }

        public Square(string id, Color color) : base(id) => this.color = color;

        public override void DrawShape() => RectWrapper.AssembleRectFromVec(pos, size).Draw(color);
        
        public override AnimationShape CopyState() =>
            new Square(id, color) {pos = new Vector2(pos.X, pos.Y), size = new Vector2(size.X, size.Y), isVisible = isVisible};
    }
}