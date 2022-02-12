using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace RayWrapper
{
    public static class FontManager
    {
        public static Font DefaultFont { get; private set; } = GetFontDefault();
        public static TextureFilter fontFilter = TextureFilter.TEXTURE_FILTER_BILINEAR;

        private static bool isNewDefaultSet = false;
        private static string newDefault;
        private static Dictionary<string, Dictionary<int, Font>> _fonts = new();
        private static Dictionary<string, string> _fontPaths = new();

        public static Font GetDefFont(int size = 24) => !isNewDefaultSet ? DefaultFont : GetFont(newDefault, size);

        public static void SetDefFont(string font)
        {
            isNewDefaultSet = true;
            newDefault = font;
        }
        
        public static void RegisterFont(string name, string path)
        {
            if (_fontPaths.ContainsKey(name)) throw new ArgumentException($"Font name `{name}` already exists");
            _fontPaths.Add(name, path);
        }

        public static Font GetFont(string name, int size)
        {
            if (!_fontPaths.ContainsKey(name))
                throw new ArgumentException($"Font path does not exist for `{name}` please [RegisterFont]");

            if (_fonts.ContainsKey(name))
            {
                if (_fonts[name].ContainsKey(size)) return _fonts[name][size];
                var newFont = LoadFont(_fontPaths[name], size);
                _fonts[name].Add(size, newFont);
                return newFont;
            }

            var font = LoadFont(_fontPaths[name], size);
            _fonts.Add(name, new Dictionary<int, Font> { { size, font } });
            return font;
        }

        public static Vector2 MeasureText(string text, string fontName, int size) =>
            GetFont(fontName, size).MeasureText(text);

        private static Font LoadFont(string font, int fontSize, int toChar = 255)
        {
            var f = LoadFontEx(font, fontSize, null, toChar);
            SetTextureFilter(f.texture, fontFilter);
            return f;
        }
    }
}