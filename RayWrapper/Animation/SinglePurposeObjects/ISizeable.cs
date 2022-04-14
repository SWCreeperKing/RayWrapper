using System.Numerics;

namespace RayWrapper.Animation.SinglePurposeObjects;

public interface ISizeable
{
    public void SetSize(Vector2 size);
    public void AddSize(Vector2 size);
}