using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AdventToolkit.Extensions
{
    public static class StringExtensions
    {
        public static bool Search(this string s, string n, out int i)
        {
            i = s.IndexOf(n, StringComparison.Ordinal);
            return i > -1;
        }

        public static bool Search(this string s, char c, out int i)
        {
            i = s.IndexOf(c);
            return i > -1;
        }

        public static IEnumerable<int> IndicesOf(this string s, char c)
        {
            var i = 0;
            while (true)
            {
                i = s.IndexOf(c, i);
                if (i < 0) yield break;
                yield return i;
                i++;
            }
        }
        
        public static IEnumerable<int> IndicesOf(this string s, string sub, bool overlap = false)
        {
            var i = 0;
            while (true)
            {
                i = s.IndexOf(sub, i, StringComparison.Ordinal);
                if (i < 0) yield break;
                yield return i;
                i += overlap ? 1 : sub.Length;
            }
        }

        public static string Reversed(this string s)
        {
            var c = s.ToCharArray();
            Array.Reverse(c);
            return new string(c);
        }
        
        public static bool Matches(this string s, string regex)
        {
            return Regex.IsMatch(s, regex);
        }
    }
}