using System.Collections;
using System.Numerics;
using System.Text.RegularExpressions;
using Raylib_CsLo;
using RayWrapper.Base.Extras;
using RayWrapper.Base.GameBox;
using RayWrapper.Base.GameObject;
using RayWrapper.Base.Primitives;

namespace RayWrapper.LegacyUI.UI;

public class RichText : GameObject
{
    public static Text.Style defaultStyle = new();

    private static readonly Regex RegEx = new(@"\[#([0-9a-fA-F]{6}|r)\]", RegexOptions.Compiled);
    private static readonly Regex RegExPlain = new(@"\[!\w+?\]", RegexOptions.Compiled);
    private static readonly Dictionary<int, Glyph> GlyphCache = new();
    private static readonly Dictionary<char, int> CodepointCache = new();
    private static readonly Dictionary<string, Color> ColorCache = new();

    public Text.Style style = defaultStyle.Copy();
    public Actionable<string?> text;

    private string _textCache;
    private List<Glyph> _displayText = new();

    public RichText(Actionable<string?> text, Vector2 pos)
    {
        Position = pos;
        this.text = text;
    }

    protected override void UpdateCall(float dt)
    {
        if (text is null) return;
        var txt = (string?) text;
        if (txt is null) return;
        if (txt == _textCache) return;
        UpdateText(txt);
    }

    protected override void RenderCall()
    {
        _displayText.ForEach(g => g.Draw(style));
    }

    public void UpdateText(string text)
    {
        _textCache = text;
        _displayText.Clear();
        var colorDict = ProcessString(text, out _textCache);
        var split = _textCache.Split('\n');

        var yOffset = Position.Y;
        var xOffset = Position.X;
        var overallIndex = 0;
        var color = colorDict[overallIndex];
        
        for (var y = 0; y < split.Length; y++, overallIndex++)
        {
            var yOffsetAdded = 0f;
            var line = split[y];

            for (var x = 0; x < line.Length; x++, overallIndex++)
            {
                if (colorDict.ContainsKey(overallIndex)) color = colorDict[overallIndex];

                var glyph = GetGlyphFromChar(line[x]) with
                {
                    Pos = new Vector2(x + xOffset, y + yOffset), Color = color
                };
                yOffsetAdded = Math.Max(yOffsetAdded, glyph.CharSize.Y);
                xOffset += glyph.CharSize.X + style.spacing;
                _displayText.Add(glyph);
            }

            xOffset = Position.X;
            yOffset += yOffsetAdded + style.spacing;
        }
    }

    private Dictionary<int, Color> ProcessString(string text, out string cleaned)
    {
        var inText = text.Replace('\r', ' ');
        Dictionary<int, Color> colorDict = new() { [0] = Raylib.WHITE };
        Stack<Color> colorStack = new();
        colorStack.Push(Raylib.WHITE);

        while (RegExPlain.IsMatch(inText))
        {
            var match = RegExPlain.Match(inText).Value;
            inText = inText.Replace(match, ColorIndex.ColorDict[match.ToLower()]);
        }

        while (RegEx.IsMatch(inText))
        {
            var rawMatch = RegEx.Match(inText);
            var match = rawMatch.Groups[1].Value;
            var index = inText.IndexOf(rawMatch.Value, StringComparison.Ordinal);

            if (match is "r" && colorStack.Count > 1) colorStack.Pop();
            else if (match is not "r") colorStack.Push(ParseColorHex(match));

            colorDict[index] = colorStack.Peek();
            inText = inText.Remove(index, rawMatch.Value.Length);
        }

        cleaned = inText;
        return colorDict;
    }

    private Color ParseColorHex(string colorHex)
    {
        if (ColorCache.ContainsKey(colorHex)) return ColorCache[colorHex];
        return ColorCache[colorHex] =
            new Color(StringToHex(colorHex[..2]), StringToHex(colorHex[2..4]), StringToHex(colorHex[4..6]), 255);
    }

    private int StringToHex(string hex) => Math.Clamp(Convert.ToInt32(hex, 16), 0, 255);

    private Glyph GetGlyphFromChar(char c)
    {
        if (CodepointCache.ContainsKey(c)) return GlyphCache[CodepointCache[c]];
        var codepoint = Raylib.GetCodepoint(c, out _);
        CodepointCache[c] = codepoint;
        return GlyphCache[codepoint] = new Glyph(codepoint, Vector2.Zero,
            style.Font.MeasureText($"{c}", style.fontSize, style.spacing), Raylib.WHITE);
    }

    private record Glyph(int Codepoint, Vector2 Pos, Vector2 CharSize, Color Color)
    {
        public void Draw(Text.Style style)
        {
            Raylib.DrawTextCodepoint(style.Font, Codepoint, Pos, style.fontSize, Color);
        }
    }
}

public static class ColorIndex
{
    // http://www.javascripter.net/faq/colornam.htm
    // I should be arrested for my crimes
    public static readonly Dictionary<string, string> ColorDict = new()
    {
        { "[!aliceblue]", "[#F0F8FF]" }, { "[!lightsalmon]", "[#FFA07A]" }, { "[!antiquewhite]", "[#FAEBD7]" },
        { "[!lightseagreen]", "[#20B2AA]" }, { "[!aqua]", "[#00FFFF]" }, { "[!lightskyblue]", "[#87CEFA]" },
        { "[!aquamarine]", "[#7FFFD4]" }, { "[!lightslategray]", "[#778899]" }, { "[!w]", "[#FFFFFF]" },
        { "[!lightslategrey]", "[#778899]" }, { "[!azure]", "[#F0FFFF]" }, { "[!lightsteelblue]", "[#B0C4DE]" },
        { "[!beige]", "[#F5F5DC]" }, { "[!lightyellow]", "[#FFFFE0]" }, { "[!bisque]", "[#FFE4C4]" },
        { "[!lime]", "[#00FF00]" }, { "[!black]", "[#000000]" }, { "[!limegreen]", "[#32CD32]" }, { "[!r]", "[#r]" },
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
        { "[!paleturquoise]", "[#AFEEEE]" }, { "[!darkslategray]", "[#2F4F4F]" }, { "[!darkyellow]", "[#8B8000]" },
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