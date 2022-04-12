using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    public class ImageObj : GameObject
    {
        public override Vector2 Position
        {
            get => _texture.Position;
            set => _texture.Position = value;
        }

        public override Vector2 Size => _texture.Size;

        public int ImageAlpha
        {
            get => _texture.ImageAlpha;
            set => _texture.ImageAlpha = value;
        } // transparency b/t 0-255

        public int Rotation
        {
            get => _texture.rotation;
            set => _texture.rotation = value;
        } // 0 to 360

        public Texture Texture => _texture;

        private TextureObj _texture;
        private Image _image;
        private Rectangle _textSize;

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
            _texture = new TextureObj(image.Texture(), pos);
            _textSize = RectWrapper.AssembleRectFromVec(Vector2.Zero, _texture.Size);
        }

        protected override void UpdateCall() => _texture.Update();
        protected override void RenderCall() => _texture.Render();

        public void RenderTo(Rectangle rect, Color? tint = null, Vector2? origin = null, float rotation = 0)
        {
            Raylib.DrawTexturePro(_texture, _textSize, rect, origin ?? Vector2.Zero, rotation, tint ?? Raylib.WHITE);
        }

        public void SetSize(Vector2 size) => _texture.SetSize(size);
        ~ImageObj() => Raylib.UnloadImage(_image);
    }
}