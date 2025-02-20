using System.Text;
using Tokenizer.Core.Constants;
using Tokenizer.Core.Models;

namespace Tokenizer.Core;

public class Tokenizer
{
    private bool _bAtEnd;
    private int _nIndex;
    private int _nRow;
    private int _nColumn;
    private char _cCurrent;
    private string _text = "";
    private IEnumerable<TokenizerRule> _rules = new List<TokenizerRule>();
    TokenizerSettings _settings;
    public Tokenizer(IEnumerable<TokenizerRule> rules, TokenizerSettings? settings = null)
    {
        _rules = rules.OrderBy((rule) => rule.Length).Reverse();
        _settings = settings ?? TokenizerSettings.Default;
    }

    public IEnumerable<Token> Tokenize(string text, bool bDebug = false)
    {
        Initialize(text);
        while (!_bAtEnd)
        {
            var token = Next();
            if (bDebug)
            {
                Console.Write($"{token}, ");
            }
            if (!_settings.CommentsAsTokens &&
                (token.Type == BuiltinTokenTypes.EndOfLineComment || token.Type == BuiltinTokenTypes.MultiLineComment)) continue;
            yield return token;
        }
        Reset();
        yield break;
    }

    private bool Initialize(string text)
    {
        Reset();
        _text = text;
        if (_text.Length == 0)
        {
            _bAtEnd = true;
            return false;
        }
        _cCurrent = _text[0];
        return true;
    }


    private void Reset()
    {
        _text = "";
        _nIndex = 0;
        _nRow = 0;
        _nColumn = 0;
        _bAtEnd = false;
    }

    private Token Next()
    {
        while (!_bAtEnd)
        {
            var start = new Location(_nRow, _nColumn);
            if (_cCurrent == '\0')
            {
                Advance();
            }

            if (char.IsWhiteSpace(_cCurrent))
            {
                if (_settings.SkipWhiteSpace)
                {
                    if (!_settings.NewlinesAsTokens)
                    {
                        Advance();
                        continue;
                    }
                    if (LookAhead(1) == "\r\n")
                    {
                        Advance();
                        var end = Advance();
                        return new Token(BuiltinTokenTypes.Newline, "\r\n", start, end);
                    }

                }
                else
                {
                    if (_cCurrent == ' ')
                    {
                        var end = Advance();
                        return new Token(BuiltinTokenTypes.Space, _cCurrent.ToString(), start, end);
                    }
                    if (_cCurrent == '\t')
                    {
                        var end = Advance();
                        return new Token(BuiltinTokenTypes.Tab, _cCurrent.ToString(), start, end);
                    }
                    if (_cCurrent == '\r')
                    {
                        var end = Advance();
                        return new Token(BuiltinTokenTypes.CarriageReturn, _cCurrent.ToString(), start, end);
                    }
                    if (_cCurrent == '\n')
                    {
                        var end = Advance();
                        return new Token(BuiltinTokenTypes.LineFeed, _cCurrent.ToString(), start, end);
                    }
                }

            }

            if (_cCurrent == '_' || char.IsLetter(_cCurrent) || _settings.WordStarters.Contains(_cCurrent))
            {
                return Word();
            }


            foreach (TokenizerRule rule in _rules)
            {
                if (CompareRule(LookAhead(rule.Length), rule))
                {
                    Advance(rule.Length);

                    if (rule.Type == BuiltinTokenTypes.EndOfLineComment)
                    {
                        return EndOfLineComment(start);
                    }

                    if (rule.Type == BuiltinTokenTypes.MultiLineComment)
                    {
                        return GetEnclosedToken(rule, start);
                    }

                    if (rule.Type == BuiltinTokenTypes.String && rule.IsEnclosed)
                    {
                        return GetStringToken(rule.EnclosingRight, start);
                    }

                    if (rule.IsEnclosed)
                    {
                        return GetEnclosedToken(rule, start);
                    }
                    var end = CurrentLocation();
                    return new Token(rule, start, end);
                }
            }

            if (_cCurrent == '_' || char.IsLetter(_cCurrent) || _settings.WordStarters.Contains(_cCurrent))
            {
                return Word();
            }

            if (char.IsDigit(_cCurrent) || (_settings.AllowNegatives && _cCurrent == _settings.NegativeChar))
            {
                return Number();
            }



            Token result = new Token(string.IsNullOrWhiteSpace(_settings.CatchAllType) ? _cCurrent.ToString() : _settings.CatchAllType, _cCurrent.ToString(), start, CurrentLocation());
            Advance();
            return result;

        }
        return new Token(BuiltinTokenTypes.EndOfFile, BuiltinTokenTypes.EndOfFile, CurrentLocation(), CurrentLocation());
    }

    private bool CompareRule(string a, TokenizerRule rule)
    {
        var stringToMatch = rule.StringToMatch;
        if (rule.IsEnclosed) stringToMatch = rule.EnclosingLeft;
        if (_settings.IgnoreCase || rule.IgnoreCase) return a.ToLower() == stringToMatch.ToLower();
        return a == stringToMatch;
    }


    private void Advance(int next)
    {
        for (int i = 0; i < next; i++)
        {
            Advance();
        }
    }
    private Location Advance()
    {
        var location = Count();
        _nIndex++;
        if (_nIndex >= _text.Length)
        {
            _bAtEnd = true;
            return location;
        }
        _cCurrent = _text[_nIndex];
        return location;
    }

    private Location Count()
    {
        if (_settings.AllOneLine)
        {
            _nColumn++;
            return new Location(_nRow, _nColumn);
        }
        if (_cCurrent == '\n')
        {
            _nRow++;
            _nColumn = 0;
        }
        else if (_cCurrent == '\r')
        {
            _nColumn = 0;
        }
        else if (_cCurrent == '\t')
        {
            _nColumn += _settings.TabSize;
        }
        else
        {
            _nColumn++;
        }
        return new Location(_nRow, _nColumn);
    }

    private string LookAhead(int peek)
    {
        string result = "";

        for (int i = 0; i < peek; i++)
        {
            if (_nIndex + i < _text.Length)
            {
                result += _text[Convert.ToInt32(_nIndex + i)];
                continue;
            }
            break;
        }
        return result;
    }


    // Multi-Character token processing

    private Token GetStringToken(string enclosing, Location start)
    {
        StringBuilder result = new StringBuilder();
        bool bSlash = false;
        while (!_bAtEnd && (LookAhead(enclosing.Length) != enclosing || bSlash))
        {
            if (_cCurrent == '\\' && !bSlash && _settings.ParseEscapeSequences)
            {
                bSlash = true;
                Advance();
                continue;
            }
            if (bSlash)
            {
                if (_cCurrent == 'n')
                {
                    result.Append('\n');
                    Advance();
                }
                else if (_cCurrent == 't')
                {
                    result.Append('\t');
                    Advance();
                }
                else if (_cCurrent == 'r')
                {
                    result.Append('\r');
                    Advance();
                }
                else if (_cCurrent == 'a')
                {
                    result.Append('\a');
                    Advance();
                }
                else if (_cCurrent == 'b')
                {
                    result.Append('\b');
                    Advance();
                }
                else if (_cCurrent == 'v')
                {
                    result.Append('\v');
                    Advance();
                }
                else if (_cCurrent == 'f')
                {
                    result.Append('\f');
                    Advance();
                }
                else if (_cCurrent == '"')
                {
                    result.Append('\"');
                    Advance();
                }
                else if (_cCurrent == '\'')
                {
                    result.Append('\'');
                    Advance();
                }
                else if (_cCurrent == '0')
                {
                    result.Append('\0');
                    Advance();
                }
                else if (_cCurrent == '\\')
                {
                    result.Append('\\');
                    Advance();
                }
                else
                {
                    result.Append('\\');
                    result.Append(_cCurrent);
                    Advance();
                }
                bSlash = false;
                continue;
            }
            else
            {
                result.Append(_cCurrent);
                Advance();
                bSlash = false;
            }
        }

        if (!_bAtEnd)
        {
            Advance(enclosing.Length);
        }
        var end = CurrentLocation();
        return new Token(BuiltinTokenTypes.String, result.ToString(), start, end);
    }

    private Token GetEnclosedToken(TokenizerRule rule, Location start)
    {
        StringBuilder result = new StringBuilder();
        while (!_bAtEnd && LookAhead(rule.EnclosingRight.Length) != rule.EnclosingRight)
        {
            result.Append(_cCurrent);
            Advance();
        }
        if (!_bAtEnd)
        {
            Advance(rule.EnclosingRight.Length);
        }
        var end = CurrentLocation();
        return new Token(rule.Type, result.ToString(), start, end);
    }

    private Location CurrentLocation() => new Location(_nRow, _nColumn);

    private Token Word()
    {
        var start = CurrentLocation();
        StringBuilder result = new StringBuilder();

        string type = BuiltinTokenTypes.Word;
        while (!_bAtEnd && (_cCurrent == '_' || char.IsLetterOrDigit(_cCurrent) || _cCurrent == '\0' || _settings.WordIncluded.Contains(_cCurrent)))
        {
            if (_cCurrent != '\0')
            {
                result.Append(_cCurrent);
            }
            Advance();
        }
        var end = CurrentLocation();
        var stringResult = result.ToString();

        foreach (TokenizerRule rule in _rules)
        {
            if (CompareRule(stringResult, rule))
            {
                if (rule.Type == BuiltinTokenTypes.EndOfLineComment)
                {
                    return EndOfLineComment(start);
                }

                if (rule.Type == BuiltinTokenTypes.MultiLineComment)
                {
                    return GetEnclosedToken(rule, start);
                }

                if (rule.Type == BuiltinTokenTypes.String && rule.IsEnclosed)
                {
                    return GetStringToken(rule.EnclosingRight, start);
                }

                if (rule.IsEnclosed)
                {
                    return GetEnclosedToken(rule, start);
                }

                return new Token(rule, start, end);
            }
        }

        return new Token(type, stringResult, start, end);
    }

    private Token Number()
    {
        StringBuilder result = new StringBuilder();
        var start = CurrentLocation();
        bool bHadDecimal = false;
        bool bIsFloat = false;
        bool bIsDouble = false;
        bool bIsUnsigned = false;
        bool bIsByte = false;
        while (!_bAtEnd &&
            (Char.IsDigit(_cCurrent)
            || (_cCurrent == '.' && !bHadDecimal)
            || (_cCurrent == 'f' && bHadDecimal)
            || (_cCurrent == 'd' && bHadDecimal)
            || (_cCurrent == 'u' && !bHadDecimal)
            || (_cCurrent == 'b' && !bHadDecimal)
            || (_settings.AllowNegatives && _cCurrent == _settings.NegativeChar && result.Length == 0)))
        {
            if (_cCurrent == '.')
            {
                bHadDecimal = true;
            }
            if (_cCurrent == 'f')
            {
                bIsFloat = true;
                Advance();
                break;
            }
            if (_cCurrent == 'd')
            {
                bIsDouble = true;
                Advance();
                break;
            }
            if (_cCurrent == 'u')
            {
                bIsUnsigned = true;
                Advance();
                break;
            }
            if (_cCurrent == 'b')
            {
                bIsByte = true;
                Advance();
                break;
            }

            result.Append(_cCurrent);
            Advance();
        }

        string type = BuiltinTokenTypes.Integer;
        if (bHadDecimal)
        {
            if (bIsFloat)
            {
                type = BuiltinTokenTypes.Float;
            }
            else if (bIsDouble)
            {
                type = BuiltinTokenTypes.Double;
            }
            else
            {
                type = BuiltinTokenTypes.Double;
            }
        }
        else if (bIsUnsigned)
        {
            type = BuiltinTokenTypes.UnsignedInteger;
        }
        else if (bIsByte)
        {
            type = BuiltinTokenTypes.Byte;
        }
        var end = CurrentLocation();
        return new Token(type, result.ToString(), start, end);
    }


    private Token EndOfLineComment(Location start)
    {
        StringBuilder result = new StringBuilder();
        while (!_bAtEnd && _cCurrent != '\n' && _cCurrent != '\r')
        {
            result.Append(_cCurrent);
            Advance();
        }
        var end = CurrentLocation();
        return new Token(BuiltinTokenTypes.EndOfLineComment, result.ToString(), start, end);
    }
}