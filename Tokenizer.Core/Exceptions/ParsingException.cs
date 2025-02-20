using Tokenizer.Core.Models;

namespace Tokenizer.Core.Exceptions;

public class ParsingException : System.Exception
{
    public Token Token { get; set; }

    public ParsingException(Token token, string message)
        : base(message)
    {
        Token = token;
    }

    public override string ToString()
    {
        return $"[{Token.Start}] {Message}";
    }
}
