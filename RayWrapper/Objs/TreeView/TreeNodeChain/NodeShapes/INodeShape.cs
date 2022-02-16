using System.Numerics;

namespace RayWrapper.Objs.TreeView.TreeNodeChain.NodeShapes
{
    public interface INodeShape
    {
        public bool IsMouseIn(Vector2 off, float scale);

        public void DrawOnHover(Vector2 off, float scale);
        public Vector2 Center(Vector2 off, float scale);
        public void DrawShape(Vector2 off, float scale);

        public void Update(Vector2 off, float scale, bool context, bool next);

        public void Draw(Vector2 off, float scale);
    }
}