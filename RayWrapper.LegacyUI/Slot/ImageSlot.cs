using RayWrapper.LegacyUI.UI;

namespace RayWrapper.LegacyUI.Slot;

public class ImageSlot : Slot
{
    public ImageObj img;
    public ImageSlot(ImageObj img) : base(img.Position, img.Size) => this.img = img;

    protected override void Draw() => img.Render();
}