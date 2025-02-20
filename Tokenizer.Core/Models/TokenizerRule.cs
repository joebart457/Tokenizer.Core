
namespace Tokenizer.Core.Models;

public class TokenizerRule
{
    public string Type { get; set; }
    public string StringToMatch { get; set; }
    public string ReplaceWith { get; set; }
    public string EnclosingLeft { get; set; }
    public string EnclosingRight { get; set; }
    public bool IgnoreCase { get; set; }

    public int Length { get { return StringToMatch.Length; } }
    public bool IsEnclosed { get { return !string.IsNullOrEmpty(EnclosingLeft) && !string.IsNullOrEmpty(EnclosingRight); } }

    public TokenizerRule(string type, string stringToMatch, string? replaceWith = null, string enclosingLeft = "", string enclosingRight = "", bool ignoreCase = false)
    {
        Type = type;
        StringToMatch = stringToMatch;
        ReplaceWith = replaceWith ?? StringToMatch;
        IgnoreCase = ignoreCase;
        EnclosingLeft = enclosingLeft;
        EnclosingRight = enclosingRight;
        if (!string.IsNullOrEmpty(EnclosingLeft) && string.IsNullOrEmpty(EnclosingRight))
        {
            EnclosingRight = EnclosingLeft;
        }
    }
}
