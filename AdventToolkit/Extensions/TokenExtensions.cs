using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventToolkit.Utilities;

namespace AdventToolkit.Extensions
{
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
        
        // Separate a string into tokens.
        // The tokens consist of:
        // Words: start with a letter and contain letters, numbers, and underscores.
        // Numbers: contain numbers and periods.
        // Symbol: any single character that doesn't fit into the other categories.
        public static IEnumerable<Token> Tokenize(this string s)
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
                    continue;
                }
                if (type == TokenType.None)
                {
                    type = t;
                    b.Append(c);
                    continue;
                }
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
                yield return new Token(b.ToString(), type);
                b.Length = 0;
                type = HintType(c);
                b.Append(c);
            }
            if (b.Length > 0) yield return new Token(b.ToString(), type);
        }
    }
}