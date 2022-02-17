using System;
using RayWrapper.Animation.SinglePurposeObjects;
using RayWrapper.Vars;

namespace RayWrapper.Animation.Transitions
{
    public class AlphaTransition<T> : Transition where T : IGameObject, IAlphable
    {
        public T t;
        public int alphaTo;
        public int startAlpha;
        public float currAlpha;

        public AlphaTransition(int alphaTo, T obj, float duration = 5) : base(obj, duration) =>
            (t, this.alphaTo) = (obj, Math.Clamp(alphaTo, 0, 255));

        public override void InitTransition() => currAlpha = startAlpha = t.GetAlpha();

        public override void UpdateTransition(float deltaTime)
        {
            currAlpha += (alphaTo - startAlpha) * deltaTime / duration;
            t.SetAlpha((int) Math.Clamp(currAlpha, 0, 255));
        }

        public override void SnapTransition() => t.SetAlpha(alphaTo);
    }
}