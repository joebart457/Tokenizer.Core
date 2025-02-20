using Tokenizer.Core.Constants;
using Tokenizer.Core.Exceptions;
using Tokenizer.Core.Models;

namespace Tokenizer.Core;

public class TokenParser
{
    private int _index = 0;
    private bool _atEnd = false;
    private Token? _current = null;
    private IList<Token> _tokens = new List<Token>();
    protected bool OverrideCurrentOnNull = false;
    public void Initialize(IList<Token> tokens)
    {
        _tokens = tokens;
        _index = 0;
        _atEnd = tokens.Count == 0;
        _current = tokens.FirstOrDefault();
    }

    public void Initialize(IList<Token> tokens, int index)
    {
        _tokens = tokens;
        _index = index;
        _atEnd = tokens.Count >= _index;
        _current = tokens.ElementAtOrDefault(index);
    }

    public void SeekBeginning()
    {
        _index = 0;
        _atEnd = _tokens.Count == 0;
        _current = _tokens.FirstOrDefault();
    }

    public void Advance()
    {
        if (_atEnd) throw new IndexOutOfRangeException("unable to advance past end");
        _index++;
        if (_index >= _tokens.Count) _atEnd = true;
        _current = _tokens.ElementAtOrDefault(_index);
    }

    public bool AdvanceIfMatch(string tokenType)
    {
        if (_atEnd || _current == null) return false;
        if (_current.Type != tokenType) return false;
        Advance();
        return true;
    }

    public bool Match(string tokenType)
    {
        if (_atEnd || _current == null) return false;
        return _current.Type == tokenType;
    }

    public bool Match(Token token, string type)
    {
        return token.Type == type;
    }

    public bool MatchLexeme(string lexeme)
    {
        if (_atEnd || _current == null) return false;
        return _current.Lexeme == lexeme;
    }

    public bool PeekMatch(int offset, string type)
    {
        if (_index + offset < _tokens.Count())
        {
            return _tokens[_index + offset].Type == type;
        }
        return false;
    }

    public Token Consume(string type, string errorMessage)
    {
        var copy = _current;
        if (!AdvanceIfMatch(type) || copy == null) throw new ParsingException(_current == null ? PreviousOrDefault(new Token(BuiltinTokenTypes.Default, "", Location.Zero, Location.Zero)) : _current, errorMessage);
        return copy;
    }

    public Token Consume(string type, Exception ex)
    {
        var copy = _current;
        if (!AdvanceIfMatch(type) || copy == null) throw ex;
        return copy;
    }

    public bool AtEnd()
    {
        return _atEnd;
    }

    public Token Current()
    {
        if (_current == null && OverrideCurrentOnNull) return new Token(BuiltinTokenTypes.EndOfFile, "", Location.Zero, Location.Zero);
        return _current ?? throw new IndexOutOfRangeException($"unable to retrieve current token at index {_index}");
    }

    public Token Previous()
    {
        if (_index - 1 >= _tokens.Count() || _index - 1 < 0)
        {
            throw new IndexOutOfRangeException("failed getting previous token");
        }
        return _tokens[_index - 1];
    }

    public Token? PreviousOrDefault()
    {
        if (_index - 1 >= _tokens.Count() || _index - 1 < 0)
        {
            return default;
        }
        return _tokens[_index - 1];
    }

    public Token PreviousOrDefault(Token @default)
    {
        if (_index - 1 >= _tokens.Count() || _index - 1 < 0)
        {
            return @default;
        }
        return _tokens[_index - 1];
    }

    public IList<Token> GetTokens()
    {
        return _tokens;
    }

    public int Index()
    {
        return _index;
    }
}
