using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    public class ImageObj : GameObject
    {
        public override Vector2 Position
        {
            get => _texture2d.Position;
            set => _texture2d.Position = value;
        }

        public override Vector2 Size => _texture2d.Size;

        public int ImageAlpha
        {
            get => _texture2d.ImageAlpha;
            set => _texture2d.ImageAlpha = value;
        } // transparency b/t 0-255

        public int Rotation
        {
            get => _texture2d.rotation;
            set => _texture2d.rotation = value;
        } // 0 to 360
        
        private TextureObj _texture2d;
        private Image _image;

        public ImageObj(string imageFile) : this(Raylib.LoadImage(imageFile), Vector2.Zero)
        {
        }
        
        public ImageObj(string imageFile, Vector2 pos) : this(Raylib.LoadImage(imageFile), pos)
        {
        }

        public ImageObj(Image image) : this(image, Vector2.Zero)
        {
        }

        public ImageObj(Image image, Vector2 pos)
        {
            _image = image;
            _texture2d = new TextureObj(image.Texture(), pos);
        }

        protected override void UpdateCall() => _texture2d.Update();
        protected override void RenderCall() => _texture2d.Render();
        public void SetSize(Vector2 size) => _texture2d.SetSize(size);
        ~ImageObj() => Raylib.UnloadImage(_image);
    }
}