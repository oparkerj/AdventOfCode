using System;
using System.Collections.Generic;
using AdventToolkit.Utilities;

namespace AdventToolkit.Extensions
{
    public static class Data
    {
        public static string[] Csv(this string s, bool space = false)
        {
            return space ? s.Split(", ") : s.Split(',');
        }

        public static string[] Spaced(this string s)
        {
            return s.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] Tabbed(this string s)
        {
            return s.Split('\t');
        }

        public static (string Left, string Right) SingleSplit(this string s, char c)
        {
            var i = s.IndexOf(c);
            return (s[..i], s[(i + 1)..]);
        }

        public static (string Left, string Right) SingleSplit(this string s, string split)
        {
            var i = s.IndexOf(split, StringComparison.Ordinal);
            return (s[..i], s[(i + split.Length)..]);
        }

        public static string[] SingleSplitArray(this string s, char c)
        {
            var result = new string[2];
            (result[0], result[1]) = s.SingleSplit(c);
            return result;
        }

        public static void Swap<T>(this T[] arr, int a, int b)
        {
            (arr[a], arr[b]) = (arr[b], arr[a]);
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            (a, b) = (b, a);
        }

        public static bool Search<T>(this T[] arr, T t, out int i)
        {
            i = Array.IndexOf(arr, t);
            return i > -1;
        }

        public static Range On(this Range range, int elements)
        {
            return new Range(range.Start.GetOffset(elements), range.End.GetOffset(elements));
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

        public static IEnumerable<Token[]> Split<TA, TB>(this Token[] pairs, TA a, TB b, bool purgeEmpty = true)
        {
            var last = 0;
            for (var i = 0; i < pairs.Length; i++)
            {
                var (pa, pb) = pairs[i];
                if (pa.Equals(a) && pb.Equals(b))
                {
                    if (i > last || !purgeEmpty)
                    {
                        yield return pairs[last..i];
                    }
                    last = i + 1;
                }
            }
            if (last < pairs.Length) yield return pairs[last..];
        }

        public static IComparer<TU> SelectFrom<T, TU>(this IComparer<T> comparer, Func<TU, T> func)
        {
            return Comparer<TU>.Create((a, b) => comparer.Compare(func(a), func(b)));
        }

        public static Func<T, bool> NotNull<T>()
        {
            return arg => arg != null;
        }

        public static Func<TIn, TOut> Memoize<TIn, TOut>(Func<TIn, TOut> func)
        {
            var inputs = new Dictionary<TIn, TOut>();
            return input =>
            {
                if (inputs.TryGetValue(input, out var result)) return result;
                return inputs[input] = func(input);
            };
        }

        public static Func<T, T> Identity<T>()
        {
            return t => t;
        }
    }
}