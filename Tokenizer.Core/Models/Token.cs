
namespace Tokenizer.Core.Models;

public class Token
{
    public string Type { get; set; }
    public string Lexeme { get; set; }
    public Location Start { get; set; }
    public Location End { get; set; }

    public Token(TokenizerRule rule, Location start, Location end)
    {
        Lexeme = rule.ReplaceWith;
        Type = rule.Type;
        Start = start;
        End = end;
    }

    public Token(string type, string lexeme, Location start, Location end)
    {
        Lexeme = lexeme;
        Type = type;
        Start = start;
        End = end;
    }

    public override string ToString()
    {
        return $"[{Start}]({Type}, {Lexeme})";
    }

    public override int GetHashCode()
    {
        return Type.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Token token)
        {
            if (!Start.Equals(token.Start)) return false;
            if (!End.Equals(token.End)) return false;
            return Type == token.Type && Lexeme == token.Lexeme;
        }
        return false;
    }
}
