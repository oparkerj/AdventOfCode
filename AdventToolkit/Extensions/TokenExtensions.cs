using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventToolkit.Utilities;

namespace AdventToolkit.Extensions;

public static class TokenExtensions
{
    private static TokenType HintType(this char c)
    {
        return c switch
        {
            { } when char.IsDigit(c) => TokenType.Number,
            { } when char.IsLetter(c) => TokenType.Word,
            { } when char.IsWhiteSpace(c) => TokenType.Whitespace,
            _ => TokenType.Symbol
        };
    }

    public static IEnumerable<string> TokenSplit(this string s)
    {
        return Tokenize(s).Select(token => token.Content);
    }

    public static IEnumerable<Token> Tokenize(this string s) => Tokenize(s, default);

    // Separate a string into tokens.
    // The tokens consist of:
    // Words: start with a letter and contain letters, numbers, and underscores.
    // Numbers: contain numbers and periods.
    // Symbol: any single character that doesn't fit into the other categories.
    public static IEnumerable<Token> Tokenize(this string s, TokenSettings settings)
    {
        var b = new StringBuilder();
        var type = TokenType.None;
        foreach (var c in s)
        {
            var t = HintType(c);
            if (t == TokenType.Whitespace)
            {
                if (b.Length > 0) yield return new Token(b.ToString(), type);
                b.Length = 0;
                type = TokenType.None;
                if (settings.KeepWhitespace) yield return new Token(c.ToString(), t);
                continue;
            }
            if (type == TokenType.None)
            {
                type = t;
                b.Append(c);
                continue;
            }
            // A dot after a number will be considered a part of the number
            if (type == TokenType.Number && t == TokenType.Symbol && c == '.')
            {
                t = TokenType.Number;
            }
            if (type == t && type != TokenType.Symbol)
            {
                if (type == TokenType.Word && settings.SingleLetters && b.Length > 0)
                {
                    yield return new Token(b.ToString(), type);
                    b.Length = 0;
                }
                b.Append(c);
                continue;
            }
            // Allow words to be alphanumeric
            if (type == TokenType.Word && (t == TokenType.Number || c == '_'))
            {
                if (settings.SingleLetters && b.Length > 0)
                {
                    yield return new Token(b.ToString(), type);
                    b.Length = 0;
                }
                b.Append(c);
                continue;
            }
            if (b.Length <= 0) continue;
            yield return new Token(b.ToString(), type);
            b.Length = 0;
            type = HintType(c);
            b.Append(c);
        }
        if (b.Length > 0) yield return new Token(b.ToString(), type);
    }
}

public struct TokenSettings
{
    public bool KeepWhitespace;
    public bool SingleLetters;
}