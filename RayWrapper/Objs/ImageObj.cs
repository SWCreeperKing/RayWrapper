using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    public class ImageObj : GameObject
    {
        public override Vector2 Position
        {
            get => _t2d.Position;
            set => _t2d.Position = value;
        }

        public override Vector2 Size => _t2d.Size;

        public int ImageAlpha
        {
            get => _t2d.ImageAlpha;
            set => _t2d.ImageAlpha = value;
        } // transparency b/t 0-255

        public int Rotation
        {
            get => _t2d.rotation;
            set => _t2d.rotation = value;
        } // 0 to 360
        
        private TextureObj _t2d;
        private Image _img;

        public ImageObj(string imageFile) : this(Raylib.LoadImage(imageFile), Vector2.Zero)
        {
        }
        
        public ImageObj(string imageFile, Vector2 pos) : this(Raylib.LoadImage(imageFile), pos)
        {
        }

        public ImageObj(Image img) : this(img, Vector2.Zero)
        {
        }

        public ImageObj(Image img, Vector2 pos)
        {
            _img = img;
            _t2d = new TextureObj(img.Texture(), pos);
        }

        protected override void UpdateCall() => _t2d.Update();
        protected override void RenderCall() => _t2d.Render();
        public void SetSize(Vector2 size) => _t2d.SetSize(size);
        ~ImageObj() => Raylib.UnloadImage(_img);
    }
}