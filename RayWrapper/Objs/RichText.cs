using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using Raylib_CsLo;
using RayWrapper.Base.GameObject;

namespace RayWrapper.Objs;

public class RichText : GameObject
{
    public static Text.Style defaultStyle = new();

    private static readonly Regex RegEx = new(@"\[#([0-9a-fA-F]{6})\]", RegexOptions.Compiled);
    private static readonly Regex RegExPlain = new(@"\[![a-zA-Z]+\]", RegexOptions.Compiled);

    public string PureText { get; private set; }

    public Text.Style style = defaultStyle.Copy();

    private PrintData[] _data;

    public RichText(string text, Vector2 pos)
    {
        this.pos = pos;
        UpdateText(text);
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

    public string CorrectString(string text)
    {
        var txt = text;
        while (RegExPlain.IsMatch(txt))
        {
            var match = RegExPlain.Match(txt).Value;
            txt = txt.Replace(match, ColorIndex.ColorDict[match]);
        }

        return txt.Replace("\r", "");
    }

    public void UpdateText(string text)
    {
        PureText = RegExPlain.Replace(RegEx.Replace(text, ""), "");
        List<PrintData> datas = new();

        var position = pos;
        var color = Raylib.RAYWHITE;
        string mText;

        foreach (var split in CorrectString(text).Split("\n"))
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
                position.X += measure.X + style.spacing;
                addedY = Math.Max(addedY, measure.Y);
                mText = mText[(index + 9)..];

                color = new Color(ParseHex(match[2..4]), ParseHex(match[4..6]), ParseHex(match[6..8]), 255);
            }

            datas.Add(new PrintData(position, mText, color));
            position.Y += addedY;
            position.X = pos.X;
        }

        _data = datas.ToArray();
    }

    public int ParseHex(string hex) => Convert.ToInt32(hex, 16);

    public readonly struct PrintData
    {
        public readonly Vector2 pos;
        public readonly string text;
        public readonly Color color;

        public PrintData(Vector2 pos, string text, Color color)
        {
            this.pos = pos;
            this.text = text;
            this.color = color;
        }

        public void Deconstruct(out Vector2 pos, out string text, out Color color)
        {
            pos = this.pos;
            text = this.text;
            color = this.color;
        }
    };
}

public static class ColorIndex
{
    // http://www.javascripter.net/faq/colornam.htm
    // I should be arrested for my crimes
    public static readonly Dictionary<string, string> ColorDict = new()
    {
        { "[!aliceblue]", "[#F0F8FF]" }, { "[!lightsalmon]", "[#FFA07A]" }, { "[!antiquewhite]", "[#FAEBD7]" },
        { "[!lightseagreen]", "[#20B2AA]" }, { "[!aqua]", "[#00FFFF]" }, { "[!lightskyblue]", "[#87CEFA]" },
        { "[!aquamarine]", "[#7FFFD4]" }, { "[!lightslategray]", "[#778899]" },
        { "[!lightslategrey]", "[#778899]" }, { "[!azure]", "[#F0FFFF]" }, { "[!lightsteelblue]", "[#B0C4DE]" },
        { "[!beige]", "[#F5F5DC]" }, { "[!lightyellow]", "[#FFFFE0]" }, { "[!bisque]", "[#FFE4C4]" },
        { "[!lime]", "[#00FF00]" }, { "[!black]", "[#000000]" }, { "[!limegreen]", "[#32CD32]" },
        { "[!blanchedalmond]", "[#FFEBCD]" }, { "[!linen]", "[#FAF0E6]" }, { "[!blue]", "[#0000FF]" },
        { "[!magenta]", "[#FF00FF]" }, { "[!blueviolet]", "[#8A2BE2]" }, { "[!maroon]", "[#800000]" },
        { "[!brown]", "[#A52A2A]" }, { "[!mediumaquamarine]", "[#66CDAA]" }, { "[!burlywood]", "[#DEB887]" },
        { "[!mediumblue]", "[#0000CD]" }, { "[!cadetblue]", "[#5F9EA0]" }, { "[!mediumorchid]", "[#BA55D3]" },
        { "[!chartreuse]", "[#7FFF00]" }, { "[!mediumpurple]", "[#9370DB]" }, { "[!chocolate]", "[#D2691E]" },
        { "[!mediumseagreen]", "[#3CB371]" }, { "[!coral]", "[#FF7F50]" }, { "[!mediumslateblue]", "[#7B68EE]" },
        { "[!cornflowerblue]", "[#6495ED]" }, { "[!mediumspringgreen]", "[#00FA9A]" },
        { "[!cornsilk]", "[#FFF8DC]" }, { "[!mediumturquoise]", "[#48D1CC]" }, { "[!crimson]", "[#DC143C]" },
        { "[!mediumvioletred]", "[#C71585]" }, { "[!cyan]", "[#00FFFF]" }, { "[!midnightblue]", "[#191970]" },
        { "[!darkblue]", "[#00008B]" }, { "[!mintcream]", "[#F5FFFA]" }, { "[!darkcyan]", "[#008B8B]" },
        { "[!darkaqua]", "[#008B8B]" }, { "[!mistyrose]", "[#FFE4E1]" }, { "[!darkgoldenrod]", "[#B8860B]" },
        { "[!moccasin]", "[#FFE4B5]" }, { "[!darkgray]", "[#A9A9A9]" }, { "[!darkgrey]", "[#A9A9A9]" },
        { "[!navajowhite]", "[#FFDEAD]" }, { "[!darkgreen]", "[#006400]" }, { "[!navy]", "[#000080]" },
        { "[!darkkhaki]", "[#BDB76B]" }, { "[!oldlace]", "[#FDF5E6]" }, { "[!darkmagenta]", "[#8B008B]" },
        { "[!darkfuchsia]", "[#8B008B]" }, { "[!olive]", "[#808000]" }, { "[!darkolivegreen]", "[#556B2F]" },
        { "[!olivedrab]", "[#6B8E23]" }, { "[!darkorange]", "[#FF8C00]" }, { "[!orange]", "[#FFA500]" },
        { "[!darkorchid]", "[#9932CC]" }, { "[!orangered]", "[#FF4500]" }, { "[!darkred]", "[#8B0000]" },
        { "[!orchid]", "[#DA70D6]" }, { "[!darksalmon]", "[#E9967A]" }, { "[!palegoldenrod]", "[#EEE8AA]" },
        { "[!darkseagreen]", "[#8FBC8F]" }, { "[!palegreen]", "[#98FB98]" }, { "[!darkslateblue]", "[#483D8B]" },
        { "[!paleturquoise]", "[#AFEEEE]" }, { "[!darkslategray]", "[#2F4F4F]" },
        { "[!darkslategrey]", "[#2F4F4F]" }, { "[!palevioletred]", "[#DB7093]" },
        { "[!darkturquoise]", "[#00CED1]" }, { "[!papayawhip]", "[#FFEFD5]" }, { "[!darkviolet]", "[#9400D3]" },
        { "[!peachpuff]", "[#FFDAB9]" }, { "[!deeppink]", "[#FF1493]" }, { "[!peru]", "[#CD853F]" },
        { "[!deepskyblue]", "[#00BFFF]" }, { "[!pink]", "[#FFC0CB]" }, { "[!dimgray]", "[#696969]" },
        { "[!dimgrey]", "[#696969]" }, { "[!plum]", "[#DDA0DD]" }, { "[!dodgerblue]", "[#1E90FF]" },
        { "[!powderblue]", "[#B0E0E6]" }, { "[!firebrick]", "[#B22222]" }, { "[!purple]", "[#800080]" },
        { "[!floralwhite]", "[#FFFAF0]" }, { "[!red]", "[#FF0000]" }, { "[!forestgreen]", "[#228B22]" },
        { "[!rosybrown]", "[#BC8F8F]" }, { "[!fuchsia]", "[#FF00FF]" }, { "[!royalblue]", "[#4169E1]" },
        { "[!gainsboro]", "[#DCDCDC]" }, { "[!saddlebrown]", "[#8B4513]" }, { "[!ghostwhite]", "[#F8F8FF]" },
        { "[!salmon]", "[#FA8072]" }, { "[!gold]", "[#FFD700]" }, { "[!sandybrown]", "[#FAA460]" },
        { "[!goldenrod]", "[#DAA520]" }, { "[!seagreen]", "[#2E8B57]" }, { "[!gray]", "[#808080]" },
        { "[!grey]", "[#808080]" }, { "[!seashell]", "[#FFF5EE]" }, { "[!green]", "[#008000]" },
        { "[!sienna]", "[#A0522D]" }, { "[!greenyellow]", "[#ADFF2F]" }, { "[!silver]", "[#C0C0C0]" },
        { "[!honeydew]", "[#F0FFF0]" }, { "[!skyblue]", "[#87CEEB]" }, { "[!hotpink]", "[#FF69B4]" },
        { "[!slateblue]", "[#6A5ACD]" }, { "[!indianred]", "[#CD5C5C]" }, { "[!slategray]", "[#708090]" },
        { "[!slategrey]", "[#708090]" }, { "[!indigo]", "[#4B0082]" }, { "[!snow]", "[#FFFAFA]" },
        { "[!ivory]", "[#FFFFF0]" }, { "[!springgreen]", "[#00FF7F]" }, { "[!khaki]", "[#F0E68C]" },
        { "[!steelblue]", "[#4682B4]" }, { "[!lavender]", "[#E6E6FA]" }, { "[!tan]", "[#D2B48C]" },
        { "[!lavenderblush]", "[#FFF0F5]" }, { "[!teal]", "[#008080]" }, { "[!lawngreen]", "[#7CFC00]" },
        { "[!thistle]", "[#D8BFD8]" }, { "[!lemonchiffon]", "[#FFFACD]" }, { "[!tomato]", "[#FF6347]" },
        { "[!lightblue]", "[#ADD8E6]" }, { "[!turquoise]", "[#40E0D0]" }, { "[!lightcoral]", "[#F08080]" },
        { "[!violet]", "[#EE82EE]" }, { "[!lightcyan]", "[#E0FFFF]" }, { "[!lightaqua]", "[#E0FFFF]" },
        { "[!wheat]", "[#F5DEB3]" }, { "[!lightgoldenrodyellow]", "[#FAFAD2]" }, { "[!white]", "[#FFFFFF]" },
        { "[!lightgreen]", "[#90EE90]" }, { "[!whitesmoke]", "[#F5F5F5]" }, { "[!lightgray]", "[#D3D3D3]" },
        { "[!lightgrey]", "[#D3D3D3]" }, { "[!yellow]", "[#FFFF00]" }, { "[!lightpink]", "[#FFB6C1]" },
        { "[!yellowgreen]", "[#9ACD32]" }
    };
}