using System.Text.RegularExpressions;

namespace RayWrapper.Base;

public static class LanguageReader
{
    public static readonly Regex data = new(@"""(.*)"".*?""(.*)""");

    public static string currentLanguage;
    public static Dictionary<string, Dictionary<string, string>> languageDictionary = new();

    public static void ReadLangFile(string langKey, string path)
    {
        using var sr = new StreamReader(path);
        var dict = sr.ReadToEnd()
            .Replace("\r", "")
            .Split("\n")
            .Where(s => data.IsMatch(s))
            .Select(s =>
            {
                var m = data.Match(s).Groups;
                return (m[1].Value, m[2].Value);
            }).ToDictionary(m => m.Item1, m => m.Item2);
        languageDictionary.Add(langKey, dict);
        sr.Close();
    }

    public static string Tn(this string key) => languageDictionary[currentLanguage][key].Replace("\\n", "\n");
}