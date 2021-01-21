using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventToolkit.Extensions
{
    public static class Data
    {
        public static IEnumerable<string> JoinEach(this IEnumerable<string[]> ie, string sep = null)
        {
            if (sep == null) return ie.Select(strings => string.Concat(strings));
            return ie.Select(strings => string.Join(sep, strings));
        }
        
        public static string Str(this IEnumerable<char> chars)
        {
            var b = new StringBuilder();
            foreach (var c in chars)
            {
                b.Append(c);
            }
            return b.ToString();
        }

        public static string[] Csv(this string s, bool space = false)
        {
            return space ? s.Split(", ") : s.Split(',');
        }

        public static IEnumerable<int> Ints(this IEnumerable<string> strings)
        {
            return strings.Select(int.Parse);
        }

        public static T Repeat<T>(this T t, Func<T, T> func, int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                t = func(t);
            }
            return t;
        }
        
        public static int Product(this IEnumerable<int> ints)
        {
            return ints.Aggregate((a, b) => a * b);
        }

        public static long LongProduct(this IEnumerable<int> ints)
        {
            return ints.Select(i => (long) i).Aggregate((a, b) => a * b);
        }

        public static IEnumerable<T> Get<T>(this T[] t, IEnumerable<int> indices)
        {
            return indices.Select(i => t[i]);
        }

        public static IEnumerable<T> GetFrom<T>(this IEnumerable<int> indices, T[] t)
        {
            return t.Get(indices);
        }

        public static void Swap<T>(this T[] arr, int a, int b)
        {
            var t = arr[a];
            arr[a] = arr[b];
            arr[b] = t;
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            var t = a;
            a = b;
            b = t;
        }

        public static bool Search<T>(this T[] arr, T t, out int i)
        {
            i = Array.IndexOf(arr, t);
            return i > -1;
        }

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
        
        public static int GetEndParen(this string s, int start)
        {
            var level = 0;
            for (var i = start; i < s.Length; i++)
            {
                if (s[i] == '(') level++;
                if (s[i] == ')') level--;
                if (level == 0) return i;
            }
            return -1;
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> ie)
        {
            return ie.SelectMany(e => e);
        }

        public static IEnumerator<int> GetEnumerator(this Range range)
        {
            if (range.Start.IsFromEnd || range.End.IsFromEnd) throw new Exception("Range has indices from end.");
            return Enumerable.Range(range.Start.Value, range.End.Value - range.Start.Value).GetEnumerator();
        }

        public static Range On(this Range range, int elements)
        {
            return new(range.Start.GetOffset(elements), range.End.GetOffset(elements));
        }

        public static string AsString(this IEnumerable<bool> bools)
        {
            return bools.Select(b => b ? '1' : '0').Str();
        }
        
        public static string Reversed(this string s)
        {
            var c = s.ToCharArray();
            Array.Reverse(c);
            return new string(c);
        }

        public static int Count(this IEnumerable<char> s, char c)
        {
            return s.Count(ch => ch == c);
        }

        public static int CountValues<T, TV>(this IEnumerable<KeyValuePair<T, TV>> pairs, TV value)
        {
            return pairs.Count(pair => Equals(pair.Value, value));
        }
        
        public static int CountValues<T, TV>(this IEnumerable<KeyValuePair<T, TV>> pairs, Func<TV, bool> func)
        {
            return pairs.Count(pair => func(pair.Value));
        }
        
        public static bool Matches(this string s, string regex)
        {
            return Regex.IsMatch(s, regex);
        }

        public static IEnumerable<T> EnumerateSingle<T>(this T t)
        {
            yield return t;
        }

        public static TV GetOrSetValue<T, TV>(this Dictionary<T, TV> dict, T key, Func<TV> func)
        {
            if (dict.TryGetValue(key, out var value)) return value;
            return dict[key] = func();
        }
    }
}