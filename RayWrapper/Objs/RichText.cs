using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using Raylib_CsLo;
using RayWrapper.Vars;

namespace RayWrapper.Objs
{
    public class RichText : GameObject
    {
        public static Text.Style defaultStyle = new();

        private static readonly Regex RegEx = new(@"\[#([0-9a-fA-F]{6})\]");

        public override Vector2 Position
        {
            get => _pos;
            set => _pos = value;
        }

        public override Vector2 Size { get; }

        public Text.Style style = defaultStyle.Copy();

        private PrintData[] _data;
        private Vector2 _pos;

        public RichText(Vector2 pos, string text)
        {
            _pos = pos;
            UpdateText(text);
        }

        protected override void UpdateCall()
        {
        }

        protected override void RenderCall()
        {
            if (_data is null) return;
            foreach (var (pos, txt, clr) in _data)
            {
                style.color = clr;
                style.Draw(txt, pos);
            }
        }

        public void UpdateText(string text)
        {
            List<PrintData> datas = new();

            var position = _pos;
            var color = Raylib.RAYWHITE;
            string mText;

            foreach (var split in text.Replace("\r", "").Split("\n"))
            {
                var addedY = 0f;
                mText = split;
                while (RegEx.IsMatch(mText))
                {
                    var match = RegEx.Match(mText).Value;
                    var index = mText.IndexOf(match, StringComparison.Ordinal);

                    var txt = mText[..index];
                    datas.Add(new PrintData(position, txt, color));
                    var measure = style.MeasureText(txt);
                    position.X += measure.X;
                    addedY = Math.Max(addedY, measure.Y);
                    mText = mText[(index + 9)..];

                    color = new Color(ParseHex(match[2..4]), ParseHex(match[4..6]), ParseHex(match[6..8]), 255);
                }

                datas.Add(new PrintData(position, mText, color));
                position.Y += addedY;
                position.X = _pos.X;
            }

            _data = datas.ToArray();
        }

        public int ParseHex(string hex) => Convert.ToInt32(hex, 16);

        public record PrintData(Vector2 Pos, string Text, Color Color);
    }
}