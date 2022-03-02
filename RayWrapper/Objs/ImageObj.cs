using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    public class ImageObj : GameObject
    {
        public override Vector2 Position
        {
            get => _Texture.Position;
            set => _Texture.Position = value;
        }

        public override Vector2 Size => _Texture.Size;

        public int ImageAlpha
        {
            get => _Texture.ImageAlpha;
            set => _Texture.ImageAlpha = value;
        } // transparency b/t 0-255

        public int Rotation
        {
            get => _Texture.rotation;
            set => _Texture.rotation = value;
        } // 0 to 360
        
        private TextureObj _Texture;
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
            _Texture = new TextureObj(image.Texture(), pos);
        }

        protected override void UpdateCall() => _Texture.Update();
        protected override void RenderCall() => _Texture.Render();
        public void SetSize(Vector2 size) => _Texture.SetSize(size);
        ~ImageObj() => Raylib.UnloadImage(_image);
    }
}