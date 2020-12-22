using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventToolkit.Extensions
{
    public static class Tokens
    {
        public enum Type
        {
            None,
            Number,
            Word,
            Symbol,
            Whitespace,
        }

        private static Type TypeOf(char c)
        {
            return c switch
            {
                { } when char.IsDigit(c) => Type.Number,
                { } when char.IsLetter(c) => Type.Word,
                { } when char.IsWhiteSpace(c) => Type.Whitespace,
                _ => Type.Symbol
            };
        }

        public static IEnumerable<string> TokenSplit(this string s)
        {
            return Tokenize(s).Select(tuple => tuple.Item1);
        }
        
        public static IEnumerable<(string, Type)> Tokenize(this string s)
        {
            var b = new StringBuilder();
            var type = Type.None;
            foreach (var c in s)
            {
                var t = TypeOf(c);
                if (t == Type.Whitespace)
                {
                    if (b.Length > 0) yield return (b.ToString(), type);
                    b.Length = 0;
                    type = Type.None;
                    continue;
                }
                if (type == Type.None)
                {
                    type = t;
                    b.Append(c);
                    continue;
                }
                if (type == Type.Number && t == Type.Symbol && c == '.')
                {
                    t = Type.Number;
                }
                if (type == t && type != Type.Symbol)
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