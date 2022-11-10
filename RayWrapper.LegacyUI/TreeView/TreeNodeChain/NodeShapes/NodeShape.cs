using System.Numerics;
using RayWrapper.Base.Extras;
using RayWrapper.Base.GameBox;
using RayWrapper.Base.Primitives;
using static Raylib_CsLo.Raylib;

namespace RayWrapper.LegacyUI.TreeView.TreeNodeChain.NodeShapes;

public abstract class NodeShape : INodeShape
{
    protected Vector2 position;
    protected Vector2 size;

    // TODO: Turn into properties and move to interface.
    public ColorModule color = new(0);
    public ColorModule completeColor = new(255);
    public Actionable<bool> completed = new(false);
    public Action lClick;
    public Action rClick;
    public bool isVisibleCompleted = false;
    public Actionable<string> tooltip;

    protected NodeShape(Vector2 position, Vector2 size, Actionable<string> tooltip = null) =>
        (this.position, this.size, this.tooltip) = (position, size, tooltip);

    public virtual bool IsMouseIn(Vector2 off, float scale) =>
        Input.MousePosition.currentPosition.IsVectInVects(off + position * scale, size * scale);

    public virtual void DrawOnHover(Vector2 off, float scale) => DrawShape(off, scale);
    public virtual Vector2 Center(Vector2 off, float scale) => position * scale + off + size / 2 * scale;
    public abstract void DrawShape(Vector2 off, float scale);

    public void Update(Vector2 off, float scale, bool context, bool next)
    {
        if (!IsMouseIn(off, scale)) return;
        var isLeft = IsMouseButtonPressed(MOUSE_LEFT_BUTTON);
        var isRight = IsMouseButtonPressed(MOUSE_RIGHT_BUTTON);
        if (isLeft && context && !completed) lClick?.Invoke();
        if (isRight && completed && !next) rClick?.Invoke();
    }

    public string Draw(Vector2 off, float scale)
    {
        if (!isVisibleCompleted || completed) DrawShape(off, scale);
        if (!IsMouseIn(off, scale)) return null;
        DrawOnHover(off, scale);
        return tooltip;
    }
}