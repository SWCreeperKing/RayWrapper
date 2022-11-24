using System.Text.RegularExpressions;

namespace RayWrapperTesterImgui.Testing;

public class Lexer
{
    public enum Defualt
    {
        None
    }

    private List<Tokenizer> _tokenizers = new();

    public Lexer Add(Tokenizer tokenizer)
    {
        _tokenizers.Add(tokenizer);
        return this;
    }

    public Lexer Add(params Tokenizer[] tokenizer)
    {
        _tokenizers.AddRange(tokenizer);
        return this;
    }

    public List<Token> GetTokens(string s)
    {
        List<Token> tokens = new();
        Span<char> span = new(s.ToCharArray());

        var startingIndex = 0;
        var next = 0;
        while (next < s.Length)
        {
            var hasEnd = false;
            foreach (var tokenizer in _tokenizers)
            {
                if (!tokenizer.Match(span[next..])) continue;
                hasEnd = true;
                if (next > startingIndex)
                {
                    tokens.Add(new Token(Defualt.None, s[startingIndex..next]));
                    startingIndex = next;
                }

                tokens.Add(tokenizer.Get(s[next..]));

                break;
            }

            if (hasEnd)
            {
                next += tokens[^1].value.Length;
                startingIndex = next;
            }
            else next++;
        }

        if (startingIndex < s.Length) tokens.Add(new Token(Defualt.None, s[startingIndex..]));

        return tokens;
    }
}

public readonly struct Token
{
    public readonly Enum tokenType;
    public readonly string value;

    public Token(Enum type, string value)
    {
        tokenType = type;
        this.value = value;
    }
}

public readonly struct Tokenizer
{
    public enum SetterType
    {
        ByRegexGroup,
        Value
    }

    public readonly SetterType setType;
    public readonly Regex regex;
    public readonly Enum tokenType;
    public readonly int groupNumber;
    public readonly string value;

    public Tokenizer(string regex, Enum type, int groupNumber = 0, bool ignoreCase = true)
    {
        var options = RegexOptions.Compiled;
        if (ignoreCase) options |= RegexOptions.IgnoreCase;
        this.regex = new Regex(regex.StartsWith('^') ? regex : $"^{regex}", options);
        this.groupNumber = groupNumber;
        tokenType = type;
        setType = SetterType.ByRegexGroup;
    }

    public Tokenizer(string regex, Enum type, string value, bool ignoreCase = true)
    {
        var options = RegexOptions.Compiled;
        if (ignoreCase) options |= RegexOptions.IgnoreCase;
        this.regex = new Regex(regex, options);
        this.value = value;
        tokenType = type;
        setType = SetterType.Value;
    }

    public bool Match(Span<char> s) => regex.IsMatch(s);

    public Token Get(string s)
    {
        return new(tokenType, setType is SetterType.Value ? value : regex.Match(s).Groups[groupNumber].Value);
    }
}