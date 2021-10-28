using System;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs
{
    public class ImageObj : GameObject
    {
        public override Vector2 Position
        {
            get => _pos;
            set
            {
                if (_pos == value) return;
                _pos = value;
                Change();
            }
        }

        public override Vector2 Size => _size;

        public int imageAlpha
        {
            get => _imgColor.a;
            set => _imgColor = new Color(255, 255, 255, value);
        } // transparency b/t 0-255

        public int rotation; // 0 to 360

        private Vector2 _pos;
        private Vector2 _size;
        private Image _img;
        private Color _imgColor = new(255, 255, 255, 255);
        private Rectangle _imageRect;
        private Rectangle _destRect;
        private Texture2D _imgText;

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
            (_pos, _img) = (pos, img);
            _imageRect = AssembleRectFromVec(Vector2.Zero, _img.Size());
            _imgText = img.Texture();
            Change();
        }

        protected override void UpdateCall()
        {
        }

        protected override void RenderCall() =>
            Raylib.DrawTexturePro(_imgText, _imageRect, _destRect, Vector2.Zero, rotation % 360, _imgColor);

        public void SetSize(Vector2 size)
        {
            if (size == _size) return;
            _size = size;
            Change();
        }

        public void Change() => _destRect = AssembleRectFromVec(_pos, _size);
        ~ImageObj() => Raylib.UnloadImage(_img);
    }
}