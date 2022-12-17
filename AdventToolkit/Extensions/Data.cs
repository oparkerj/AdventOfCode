using System;
using System.Collections.Generic;
using AdventToolkit.Collections;
using AdventToolkit.Utilities;

namespace AdventToolkit.Extensions;

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

    public static Interval Interval(this Range range, int length = -1)
    {
        if (length < 0) return range;
        var (start, len) = range.GetOffsetAndLength(length);
        return new Interval(start, len);
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

    public static Func<T1, TR> Memoize<T1, TR>(Func<T1, TR> func)
    {
        var inputs = new Dictionary<T1, TR>();
        return input =>
        {
            if (inputs.TryGetValue(input, out var result)) return result;
            return inputs[input] = func(input);
        };
    }
    
    public static Func<T1, T2, TR> Memoize<T1, T2, TR>(Func<T1, T2, TR> func)
    {
        var inputs = new Dictionary<(T1, T2), TR>();
        return (a, b) =>
        {
            if (inputs.TryGetValue((a, b), out var result)) return result;
            return inputs[(a, b)] = func(a, b);
        };
    }
    
    public static Func<T1, T2, T3, TR> Memoize<T1, T2, T3, TR>(Func<T1, T2, T3, TR> func)
    {
        var inputs = new Dictionary<(T1, T2, T3), TR>();
        return (a, b, c) =>
        {
            if (inputs.TryGetValue((a, b, c), out var result)) return result;
            return inputs[(a, b, c)] = func(a, b, c);
        };
    }
    
    public static Func<T1, T2, T3, T4, TR> Memoize<T1, T2, T3, T4, TR>(Func<T1, T2, T3, T4, TR> func)
    {
        var inputs = new Dictionary<(T1, T2, T3, T4), TR>();
        return (a, b, c, d) =>
        {
            if (inputs.TryGetValue((a, b, c, d), out var result)) return result;
            return inputs[(a, b, c, d)] = func(a, b, c, d);
        };
    }

    public static Func<T, T> Identity<T>()
    {
        return t => t;
    }

    public static void Init<T>(this T[] array)
        where T : new()
    {
        for (var i = 0; i < array.Length; i++)
        {
            array[i] = new T();
        }
    }
}