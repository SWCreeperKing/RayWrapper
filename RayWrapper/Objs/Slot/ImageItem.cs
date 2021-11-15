using System.Numerics;

namespace RayWrapper.Objs.Slot
{
    public class ImageItem : SlotItem
    {
        public ImageObj img;

        public ImageItem(ImageObj img) : base(img.Position, img.Size) => this.img = img;

        public override void Draw(Vector2 pos, Vector2 size, int alpha)
        {
            img.Position = pos;
            img.SetSize(size);
            if (img.ImageAlpha != alpha) img.ImageAlpha = alpha;
            img.Render();
        }
    }
}