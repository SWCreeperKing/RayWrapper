using RayWrapper.LegacyUI.Animation.SinglePurposeObjects;

namespace RayWrapper.LegacyUI.Animation.Transitions;

public class ImageChangeTransition : Transition
{
    public TextureAtlasDrawing obj;
    public string toIdAfterDuration;

    public ImageChangeTransition(TextureAtlasDrawing obj, string toIdAfterDuration, float duration = 5) : base(obj,
        duration)
    {
        this.obj = obj;
        this.toIdAfterDuration = toIdAfterDuration;
    }

    public override void InitTransition()
    {
    }

    public override void UpdateTransition(float deltaTime)
    {
    }

    public override void SnapTransition() => obj.id = toIdAfterDuration;
}