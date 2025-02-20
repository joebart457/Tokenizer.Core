
namespace Tokenizer.Core.Constants;

public static class BuiltinTokenTypes
{
    public const string Space = "Space";
    public const string Tab = "Tab";
    public const string CarriageReturn = "CarriageReturn";
    public const string LineFeed = "LineFeed";
    public const string Newline = "Newline"; // only used when settings.NewlinesAsTokens == TRUE
    public const string Word = "Word";
    public const string String = "String";
    public const string Integer = "Integer";
    public const string UnsignedInteger = "UnsignedInteger";
    public const string Double = "Double";
    public const string Float = "Float";
    public const string Byte = "Byte";

    public const string EndOfLineComment = "EndOfLineComment";
    public const string MultiLineComment = "MultiLineComment";

    public const string EndOfFile = "EndOfFile";
    public const string Default = "_Default"; // Default token type to be used only as a last resort fallback
}

