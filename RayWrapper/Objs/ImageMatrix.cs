using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace RayWrapper.Objs
{
    public class ImageMatrix
    {
        private Image _img;
        private Dictionary<Enum, ImageObj> _slices = new();

        public ImageMatrix(string img) : this(LoadImage(img))
        {
        }

        public ImageMatrix(Image img) => _img = img;

        // rect is the slice of the bigger texture (i think)
        public void AddSlice(Enum e, Rectangle rect) =>
            _slices.Add(e, new ImageObj(Vector2.Zero, ImageFromImage(_img, rect)));

        public ImageObj this[Enum e] => _slices[e];
        ~ImageMatrix() => UnloadImage(_img);
    }
}