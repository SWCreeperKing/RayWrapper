using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace RayWrapper.Objs
{
    public class ImageMatrix
    {
        private Image _image;
        private Dictionary<Enum, ImageObj> _slices = new();

        public ImageMatrix(string image) : this(LoadImage(image))
        {
        }

        public ImageMatrix(Image image) => _image = image;

        // rect is the slice of the bigger texture (i think)
        public void AddSlice(Enum e, Rectangle rect) =>
            _slices.Add(e, new ImageObj(ImageFromImage(_image, rect), Vector2.Zero));

        public ImageObj this[Enum e] => _slices[e];
        ~ImageMatrix() => UnloadImage(_image);
    }
}