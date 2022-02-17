using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using RayWrapper.Vars;
using static Raylib_cs.Raylib;

namespace RayWrapper
{
    public static class FontManager
    {
        public static Font DefaultFont { get; } = GetFontDefault();
        public static TextureFilter fontFilter = TextureFilter.TEXTURE_FILTER_BILINEAR;

        private static bool isNewDefaultSet;
        private static string newDefault;

        private static IDictionary<string, IDictionary<int, Font>> _fonts =
            new Dictionary<string, IDictionary<int, Font>>();

        private static IDictionary<string, string> _fontPaths = new Dictionary<string, string>();

        public static Font GetDefFont(int size = 24)
        {
            if (!isNewDefaultSet && size != 24)
                Logger.Log(Logger.Level.Warning, "Default font can only load with a size of 24!");
            return !isNewDefaultSet ? DefaultFont : GetFont(newDefault, size);
        }

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
            if (_fonts.ContainsKey(name) && _fonts[name].ContainsKey(size)) return _fonts[name][size];

            if (!_fontPaths.ContainsKey(name))
                throw new ArgumentException($"Font path does not exist for `{name}` please [RegisterFont]");

            if (_fonts.ContainsKey(name))
            {
                var newFont = LoadFont(_fontPaths[name], size);
                _fonts[name].Add(size, newFont);
                return newFont;
            }

            var font = LoadFont(_fontPaths[name], size);
            _fonts.Add(name, new Dictionary<int, Font> {{size, font}});
            return font;
        }

        public static Vector2 MeasureText(string text, string fontName, int size) =>
            GetFont(fontName, size).MeasureText(text);

        private static Font LoadFont(string font, int fontSize, int toChar = 8595)
        {
            var f = LoadFontEx(font, fontSize, null, toChar);
            SetTextureFilter(f.texture, fontFilter);
            return f;
        }
    }
}