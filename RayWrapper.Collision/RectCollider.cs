using System.Numerics;
using Raylib_CsLo;

namespace RayWrapper.Collision;

public abstract class RectCollider : Collider
{
    public Rectangle rect;

    public RectCollider(Rectangle rect) : base(rect.Pos()) => this.rect = rect;

    public override bool CheckCollision(Collider c) =>
        c switch
        {
            RectCollider rc => Raylib.CheckCollisionRecs(this, rc),
            CircleCollider cc => Raylib.CheckCollisionCircleRec(cc.Position, cc.radius, this),
            _ => false
        };

    protected override Vector2 GetPosition() => rect.Pos();
    protected override Vector2 GetSize() => rect.Size();
    protected override void UpdatePosition(Vector2 newPos) => rect.MoveTo(newPos);
    protected override void UpdateSize(Vector2 newSize) => rect.SetSize(newSize);
    public override bool SampleCollision(Rectangle c) => Raylib.CheckCollisionRecs(this, c);
    public static implicit operator Rectangle(RectCollider rc) => rc.rect.Clone();
}