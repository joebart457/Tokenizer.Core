# Tokenizer.Core
The official repository for the Tokenizer.Core Nuget package.

## Overview

**Tokenizer.Core** is a C# NuGet package designed to assist developers with the process of tokenization in text processing. Tokenization is the task of splitting text into individual tokens, such as words, phrases, or symbols, which can be further processed by a parser.

## Features

- Tokenize text based on spaces, punctuation, or custom delimiters.
- Built in support for various token types (e.g., word, string, integer, floating-point).
- Customizable tokenization rules for whatever your use case may be.
- Lightweight and easy to integrate into existing C# projects.

## Installation

To install the Tokenizer.Core package, use the following command in the NuGet Package Manager Console:

```bash
Install-Package Tokenizer.Core
```

## Usage

### Basic Tokenization

Here is an example of how to use the TextTokenizer for basic word tokenization:

```csharp
using Tokenizer.Core;
using Tokenizer.Core.Constants;
using Tokenizer.Core.Models;

internal class Program
{
    private static int Main(string[] args)
    {
        var rules = new List<TokenizerRule>()
        {
            new TokenizerRule("OperatorEquals", "="),
            new TokenizerRule("PuncuationSemi", ";"),
            new TokenizerRule("PuncuationLParen", "("),
            new TokenizerRule("PuncuationRParen", ")"),
            new TokenizerRule("BooleanTrue", "true"),
            new TokenizerRule("BooleanFalse", "false"),
            new TokenizerRule("ConstantNull", "null", ignoreCase: true),
            new TokenizerRule(BuiltinTokenTypes.EndOfLineComment, "//"),
            new TokenizerRule(BuiltinTokenTypes.MultiLineComment, "#comment", enclosingLeft: "#comment", enclosingRight: "#endcomment"),
            new TokenizerRule(BuiltinTokenTypes.String, "\"", enclosingLeft: "\"", enclosingRight: "\""),
        };

        var tokenizer = new TextTokenizer(rules);
        var tokens = tokenizer.Tokenize("""
                // This is a single line comment
                #comment
                    this is a multi-line comment that will
                    be ignored by the tokenizer unless TokenizerSettings.CommentsAsTokens = true
                #endcomment
                var shouldPrint = true;
                string msg = null;
                if (shouldPrint) Console.WriteLine(msg);
            """);

        foreach(var token in tokens)
            Console.WriteLine(token);

        return 0;
    }
}
```

Example output:

```plaintext
[Ln. 5, Col. 4](Word, var)
[Ln. 5, Col. 8](Word, shouldPrint)
[Ln. 5, Col. 20](OperatorEquals, =)
[Ln. 5, Col. 22](BooleanTrue, true)
[Ln. 5, Col. 26](PuncuationSemi, ;)
[Ln. 6, Col. 4](Word, string)
[Ln. 6, Col. 11](Word, msg)
[Ln. 6, Col. 15](OperatorEquals, =)
[Ln. 6, Col. 17](ConstantNull, null)
[Ln. 6, Col. 21](PuncuationSemi, ;)
[Ln. 7, Col. 4](Word, if)
[Ln. 7, Col. 7](PuncuationLParen, ()
[Ln. 7, Col. 8](Word, shouldPrint)
[Ln. 7, Col. 19](PuncuationRParen, ))
[Ln. 7, Col. 21](Word, Console)
[Ln. 7, Col. 28](., .)
[Ln. 7, Col. 29](Word, WriteLine)
[Ln. 7, Col. 38](PuncuationLParen, ()
[Ln. 7, Col. 39](Word, msg)
[Ln. 7, Col. 42](PuncuationRParen, ))
[Ln. 7, Col. 43](PuncuationSemi, ;)
```

## Conclusion

The **Tokenizer.Core** NuGet package is a versatile tool for text tokenization tasks in C#. With its easy integration and customizable features, it can help developers efficiently process text for various tasks.

## Disclaimer

**Tokenizer.Core** is still a work in progress. While stable, it needs plenty more optimization. The primary focus of this package was to provide ease of use, not necessarily speed of text processing. 
