
namespace Tokenizer.Core.Models;

public class TokenizerSettings
{
    /// <summary>
    /// Specifies which symbols are allowed inside of words
    /// </summary>
    public string WordIncluded { get; set; } = "";
    /// <summary>
    /// Specifies which characters (besides alphabetical characters) can be used to start a word 
    /// </summary>
    public string WordStarters { get; set; } = "_";
    public string CatchAllType { get; set; } = "";
    public bool SkipWhiteSpace { get; set; } = true;
    public bool CommentsAsTokens { get; set; } = false;
    public bool IgnoreCase { get; set; } = false;

    /// <summary>
    /// When <value>true</value>, counts tabs and newlines as one char column and does not add to the row counter
    /// </summary>
    public bool AllOneLine { get; set; }
    public bool AllowNegatives { get; set; } = false;
    public char NegativeChar { get; set; } = '-';
    public bool NewlinesAsTokens { get; set; } = false;
    public int TabSize { get; set; } = 4;

    // Allows for replacing of escape sequences in strings with their actual value
    // IE \\ will be replaced with \
    // Default true.
    public bool ParseEscapeSequences { get; set; } = true;
    public static TokenizerSettings Default { get { return new TokenizerSettings(); } }
}
