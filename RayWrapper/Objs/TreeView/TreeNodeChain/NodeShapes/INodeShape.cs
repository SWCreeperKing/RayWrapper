using System.Numerics;

namespace RayWrapper.Objs.TreeView.TreeNodeChain.NodeShapes;

public interface INodeShape
{
    bool IsMouseIn(Vector2 off, float scale);

    void DrawOnHover(Vector2 off, float scale);
    Vector2 Center(Vector2 off, float scale);
    void DrawShape(Vector2 off, float scale);

    void Update(Vector2 off, float scale, bool context, bool next);

    string Draw(Vector2 off, float scale);
}