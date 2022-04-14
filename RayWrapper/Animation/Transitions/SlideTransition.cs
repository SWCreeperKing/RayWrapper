using System.Numerics;
using RayWrapper.Animation.SinglePurposeObjects;
using RayWrapper.Var_Interfaces;

namespace RayWrapper.Animation.Transitions;

public class SlideTransition<T> : Transition where T : IGameObject, ISizeable
{
    public Vector2 moveBy;
    public Vector2 moveByNeg;
    public T t;

    private Vector2 _snapPos;
    private Vector2 _snapSize;

    public SlideTransition(Vector2 moveBy, T t, float duration = 5) : base(t, duration) =>
        (this.moveBy, moveByNeg, this.t) = (moveBy, -moveBy, t);

    public override void InitTransition()
    {
        _snapPos = t.Position + moveBy;
        _snapSize = t.Size + moveByNeg;
    }

    public override void UpdateTransition(float deltaTime)
    {
        obj.Position += moveBy * deltaTime / duration;
        t.AddSize(moveByNeg * deltaTime / duration);
    }

    public override void SnapTransition()
    {
        obj.Position = _snapPos;
        t.SetSize(_snapSize);
    }
}