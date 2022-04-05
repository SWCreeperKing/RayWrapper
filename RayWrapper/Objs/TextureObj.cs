using System.Numerics;
using Raylib_CsLo;
using RayWrapper.Vars;
using static RayWrapper.RectWrapper;

namespace RayWrapper.Objs
{
    public class TextureObj : GameObject
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

        public int ImageAlpha
        {
            get => _t2dColor.a;
            set => _t2dColor = new Color(255, 255, 255, value);
        } // transparency b/t 0-255

        public int rotation; // 0 to 360

        private Texture _Texture;
        private Vector2 _pos;
        private Vector2 _size;
        private Color _t2dColor = new(255, 255, 255, 255);
        private Rectangle _t2dRect;
        private Rectangle _destRect;
        
        public TextureObj(Texture t2d, Vector2 pos)
        {
            (_Texture, _pos) = (t2d, pos);
            _t2dRect = AssembleRectFromVec(Vector2.Zero, _size = _Texture.Size());
            Change();
        }

        protected override void UpdateCall()
        {
        }

        protected override void RenderCall() => 
            Raylib.DrawTexturePro(_Texture, _t2dRect, _destRect, Vector2.Zero, rotation % 360, _t2dColor);
        
        public void SetSize(Vector2 size)
        {
            if (size == _size) return;
            _size = size;
            Change();
        }

        public static implicit operator Texture(TextureObj tObj) => tObj._Texture;
        
        public void Change() => _destRect = AssembleRectFromVec(_pos, _size);
        ~TextureObj() => Raylib.UnloadTexture(_Texture);
    }
}