using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventToolkit.Extensions
{
    public enum TokenType
    {
        None,
        Number,
        Word,
        Symbol,
        Whitespace,
    }

    public static class Tokens
    {
        private static TokenType TypeOf(char c)
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
            return Tokenize(s).Select(tuple => tuple.token);
        }
        
        // Separate a string into tokens.
        // The tokens consist of:
        // Words: start with a letter and contain letters, numbers, and underscores.
        // Numbers: contain numbers and periods.
        // Symbol: any single character that doesn't fit into the other categories.
        public static IEnumerable<(string token, TokenType type)> Tokenize(this string s)
        {
            var b = new StringBuilder();
            var type = TokenType.None;
            foreach (var c in s)
            {
                var t = TypeOf(c);
                if (t == TokenType.Whitespace)
                {
                    if (b.Length > 0) yield return (b.ToString(), type);
                    b.Length = 0;
                    type = TokenType.None;
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
                    b.Append(c);
                    continue;
                }
                // Allow words to be alphanumeric
                if (type == TokenType.Word && (t == TokenType.Number || c == '_'))
                {
                    b.Append(c);
                    continue;
                }
                if (b.Length <= 0) continue;
                yield return (b.ToString(), type);
                b.Length = 0;
                type = TypeOf(c);
                b.Append(c);
            }
            if (b.Length > 0) yield return (b.ToString(), type);
        }
    }
}