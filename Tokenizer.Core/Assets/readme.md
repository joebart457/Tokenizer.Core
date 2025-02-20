# Tokenizer.Core

## Overview

**Tokenizer.Core** is a C# NuGet package designed to assist developers with the process of tokenization in text processing. Tokenization is the task of splitting text into individual tokens, such as words, phrases, or symbols, which can be further processed by a parser.

## Features

- Tokenize text based on spaces, punctuation, or custom delimiters.
- Builtin support for various token types (e.g., word, string, integer, floating-point).
- Customizable tokenization rules for whatever your use case may be.
- Lightweight and easy to integrate into existing C# projects.

## Installation

To install the Tokenizer.Core package, use the following command in the NuGet Package Manager Console:

```bash
Install-Package Tokenizer.Core
```

## Usage

### Basic Tokenization

Here is an example of how to use the TokenizationHelper package for basic word tokenization:

```csharp
using Tokenizer.Core;

class Program
{
    static void Main(string[] args)
    {
        var text = "Hello, world! Welcome to tokenization with TokenizationHelper.";
	var tokenizer = new Tokenizer();
        var tokens = tokenizer.Tokenize(text);

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }
}
```

### Custom Delimiters

You can also customize the delimiters used for tokenization:

```csharp
using TokenizationHelper;

class Program
{
    static void Main(string[] args)
    {
        string text = "Hello|world|Welcome|to|tokenization|with|TokenizationHelper";
        var customDelimiters = new char[] { '|' };
        var tokens = Tokenizer.Tokenize(text, customDelimiters);

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }
}
```

### Sentence Tokenization

For sentence tokenization, you can use the `SentenceTokenizer` class:

```csharp
using TokenizationHelper;

class Program
{
    static void Main(string[] args)
    {
        string text = "Hello, world! Welcome to tokenization with TokenizationHelper. This is another sentence.";
        var sentences = SentenceTokenizer.Tokenize(text);

        foreach (var sentence in sentences)
        {
            Console.WriteLine(sentence);
        }
    }
}
```

## Conclusion

The **Tokenizer.Core** NuGet package is a versatile tool for text tokenization tasks in C#. With its easy integration and customizable features, it can help developers efficiently process text for various tasks.

## Disclaimer

**Tokenizer.Core** is still a work in progress. While stable, it needs plenty more optimization. The primary focus of this package was to provide ease of use, not necessarily speed of text processing. 