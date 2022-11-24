using Raylib_CsLo;
using RayWrapper.Base.Extras;
using RayWrapper.Imgui;
using static RayWrapperTesterImgui.Testing.FileLexers.BaseTokens;

// ReSharper disable HeapView.BoxingAllocation
namespace RayWrapperTesterImgui.Testing;

public static class FileLexers
{
    public enum BaseTokens
    {
        Comment,
        String,
        Character,
        Number,
        Type,
        LPara,
        RPara,
        Access,
        Super,
        Operator,
        NewLine
    }

    public static readonly Dictionary<string, Lexer> fileLexers = new()
    {
        ["cs"] = new Lexer().Add(new Tokenizer("//.*", Comment),
            new Tokenizer("/\\*(.|\\n)*?\\*/", Comment),
            new Tokenizer("(\\$|)\"(.|\\n)*?(?<![^\\\\]\\\\)\"", BaseTokens.String),
            new Tokenizer("(\\n\\r|\\r\\n|\\n|\\r)", NewLine),
            new Tokenizer("\'.\'", Character),
            new Tokenizer("([0-9]+|true|false)", Number),
            new Tokenizer("(public|static|internal|sealed|private|readonly)", Access),
            new Tokenizer("(using|class|enum|namespace|struct|void|unsafe|interface|abstract|override)", Super),
            new Tokenizer("\\(", LPara),
            new Tokenizer("(\\+|-|\\*|/|\\^|%|\\.\\.|=)", Operator), new Tokenizer("\\)", RPara),
            new Tokenizer("(ushort|short|uint|int|ulong|long|float|double|string|char|var|bool|this) ",
                BaseTokens.Type, 1)),
        ["default"] = new Lexer().Add(new Tokenizer("(\\n\\r|\\r\\n)", NewLine))
    };

    public static readonly Dictionary<Enum, uint> lexerColors = new()
    {
        [Lexer.Defualt.None] = Raylib.RAYWHITE.ToUint(),
        [Comment] = Raylib.GRAY.ToUint(),
        [BaseTokens.String] = Raylib.GREEN.ToUint(),
        [Character] = Raylib.DARKGREEN.ToUint(),
        [Number] = Raylib.SKYBLUE.ToUint(),
        [BaseTokens.Type] = Raylib.BLUE.ToUint(),
        [LPara] = Raylib.YELLOW.ToUint(),
        [RPara] = Raylib.YELLOW.ToUint(),
        [Access] = Raylib.PURPLE.ToUint(),
        [Super] = Raylib.VIOLET.ToUint(),
        [Operator] = Raylib.BEIGE.ToUint(),
        [NewLine] = Core.Transparent.ToUint()
    };
}