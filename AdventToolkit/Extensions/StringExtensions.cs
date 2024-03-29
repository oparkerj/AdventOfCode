using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AdventToolkit.Common;
using RegExtract;

namespace AdventToolkit.Extensions;

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

    public static IEnumerable<int> Digits(this string s)
    {
        return s.Select(Conversions.AsInt);
    }

    public static IEnumerable<IEnumerable<int>> Digits(this IEnumerable<string> strings)
    {
        return strings.Select(s => s.Digits());
    }

    public static int TakeInt(this string s, int start = 0)
    {
        var span = s.AsSpan();
        var i = start;
        if (span[i] is '-' or '+') i++;
        while (char.IsDigit(span[i])) i++;
        return int.Parse(span[start..i]);
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
        if (overlap)
        {
            while (true)
            {
                i = s.IndexOf(sub, i, StringComparison.Ordinal);
                if (i < 0) yield break;
                yield return i;
                i += 1;
            }
        }
        while (true)
        {
            i = s.IndexOf(sub, i, StringComparison.Ordinal);
            if (i < 0) yield break;
            yield return i;
            i += sub.Length;
        }
    }

    public static IEnumerable<int> IndicesOf(this string s, string sub, Func<string, int, int> advance)
    {
        var i = 0;
        while (true)
        {
            i = s.IndexOf(sub, i, StringComparison.Ordinal);
            if (i < 0) yield break;
            yield return i;
            i += advance(s, i);
        }
    }

    public static string Reversed(this string s)
    {
        return string.Create(s.Length, s, (result, str) =>
        {
            str.CopyTo(result);
            result.Reverse();
        });
    }

    public static bool Matches(this string s, string regex)
    {
        return Regex.IsMatch(s, regex);
    }

    public static Dictionary<string, string> ReadKeys(this IEnumerable<string> s, string sep = ":", bool trim = true)
    {
        var result = new Dictionary<string, string>();
        foreach (var line in s)
        {
            var parts = line.Split(sep);
            if (trim) result[parts[0].Trim()] = parts[1].Trim();
            result[parts[0]] = parts[1];
        }
        return result;
    }

    public static Dictionary<TKey, TValue> ReadKeys<TKey, TValue>(this IEnumerable<string> items, Func<string, TKey> key, Func<string, TValue> value, string sep = ":", bool trim = true)
    {
        var result = new Dictionary<TKey, TValue>();
        foreach (var line in items)
        {
            var parts = line.Split(sep);
            if (trim) result[key(parts[0].Trim())] = value(parts[1].Trim());
            result[key(parts[0])] = value(parts[1]);
        }
        return result;
    }

    public static string Repeat(this char c, int times)
    {
        return new string(c, times);
    }

    public static IEnumerable<int> Ascii(this string s)
    {
        return s.Select(c => (int) c);
    }

    public static IEnumerable<string> ToStrings(this IEnumerable<IEnumerable<char>> chars)
    {
        return chars.Select(c => c.Str());
    }

    // Simple caesar cipher
    public static string Caesar(this string s, int shift)
    {
        return string.Create(s.Length, (s, shift), (result, context) =>
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var original = context.s.AsSpan();
            for (var i = 0; i < result.Length; i++)
            {
                if (char.IsUpper(original[i]))
                {
                    result[i] = upper[(original[i] - 'A' + context.shift).CircularMod(26)];
                }
                else if (char.IsLower(original[i]))
                {
                    result[i] = lower[(original[i] - 'a' + context.shift).CircularMod(26)];
                }
                else result[i] = original[i];
            }
        });
    }

    public static string Repeat(this string s, int times)
    {
        return string.Create(s.Length * times, (s, times), (result, context) =>
        {
            var source = context.s.AsSpan();
            for (var i = 0; i < context.times; i++)
            {
                source.CopyTo(result[(i * source.Length)..]);
            }
        });
    }

    public static string Before(this string s, string search, bool empty = true)
    {
        var index = s.IndexOf(search, StringComparison.Ordinal);
        if (index > -1) return s[..index];
        return empty ? "" : null;
    }

    public static string Before(this string s, char search, bool empty = true)
    {
        var index = s.IndexOf(search);
        if (index > -1) return s[..index];
        return empty ? "" : null;
    }

    public static string After(this string s, string search, bool empty = true)
    {
        var index = s.IndexOf(search, StringComparison.Ordinal);
        if (index > -1) return s[(index + search.Length)..];
        return empty ? "" : null;
    }

    public static string After(this string s, char search, bool empty = true)
    {
        var index = s.IndexOf(search);
        if (index > -1) return s[(index + 1)..];
        return empty ? "" : null;
    }

    public static string Between(this string s, char first, char last)
    {
        var start = s.IndexOf(first);
        if (start == -1) return string.Empty;
        var end = s.IndexOf(last, start + 1);
        if (end == -1) return string.Empty;
        return s[(start + 1)..end];
    }

    public static string Hash(this string s, MD5 md5, bool upper = true)
    {
        const int length = 32;
        var hexBase = (byte) (upper ? 'A' - 10 : 'a' - 10);

        return string.Create(length, (s, md5, hexBase), (result, context) =>
        {
            var bytes = context.s.Length <= 256 ? stackalloc byte[context.s.Length] : new byte[context.s.Length];
            Encoding.ASCII.GetBytes(context.s.AsSpan(), bytes);
            Span<byte> hash = stackalloc byte[length / 2];

            if (!context.md5.TryComputeHash(bytes, hash, out _)) throw new Exception("Hash error.");
            for (var i = 0; i < length / 2; i++)
            {
                var digit = hash[i] & 0xF;
                result[i * 2 + 1] = (char) (digit + (digit < 10 ? (byte) '0' : context.hexBase));
                digit = (hash[i] >> 4) & 0xF;
                result[i * 2] = (char) (digit + (digit < 10 ? (byte) '0' : context.hexBase));
            }
        });
    }

    public static bool TryExtract<T>(this string s, string regex, out T result)
    {
        var r = new Regex(regex);
        var match = r.Match(s);
        if (!match.Success)
        {
            result = default;
            return false;
        }

        result = ExtractionPlan<T>.CreatePlan(r).Extract(match);
        return true;
    }

    public static string ReplaceAt(this string s, int index, int count, string replace)
    {
        if (index < 0 || index + count > s.Length) throw new ArgumentException("Index out of bounds.");
        return string.Create(s.Length - count + replace.Length, (s, index, count, replace), (result, context) =>
        {
            var span = context.s.AsSpan();
            span[..context.index].CopyTo(result);
            context.replace.CopyTo(result[context.index..]);
            span[(context.index + context.count)..].CopyTo(result[(context.index + context.replace.Length)..]);
        });
    }

    public static int LevenshteinDistance(this string a, string b)
    {
        var length = b.Length + 1;
        var last = length <= 256 ? stackalloc int[length] : new int[length];
        var current = length <= 256 ? stackalloc int[length] : new int[length];

        for (var i = 0; i < length; i++)
        {
            last[i] = i;
        }

        for (var i = 0; i < a.Length; i++)
        {
            current[0] = i + 1;
            for (var j = 0; j < b.Length; j++)
            {
                var delete = last[j + 1] + 1;
                var insert = current[j] + 1;
                var change = a[i] == b[j] ? last[j] : last[j] + 1;
                current[j + 1] = Math.Min(delete, Math.Min(insert, change));
            }
            var temp = last;
            last = current;
            current = temp;
        }
        return last[b.Length];
    }

    public static IEnumerable<string> SplitSpaceOrComma(this string s)
    {
        var start = -1;
        for (var i = 0; i < s.Length; i++)
        {
            var c = s[i];
            if (c is ' ' or ',')
            {
                if (start >= 0)
                {
                    yield return new string(s.AsSpan(start, i - start));
                    start = -1;
                }
                else start = -1;
            }
            else if (start == -1) start = i;
        }
        if (start != -1) yield return new string(s.AsSpan(start));
    }

    public static (string First, string Second) SplitHalf(this string s)
    {
        var len = s.Length / 2;
        return (s[..len], s[len..]);
    }

    public static IEnumerable<T> GetAll<T>(this string s, Regex regex, Func<string, T> parse, int gap = 0, int start = 0)
    {
        while (start < s.Length && regex.Match(s, start) is var match && match.Success)
        {
            yield return parse(match.Value);
            start = match.Index + match.Length + gap;
        }
    }

    public static IEnumerable<T> GetAll<T>(this string s, Regex regex, int gap = 0, int start = 0)
        where T : IParsable<T>
    {
        return s.GetAll(regex, str => T.Parse(str, null), gap, start);
    }

    public static IEnumerable<int> GetInts(this string s)
    {
        return s.GetAll<int>(Patterns.Int, 1);
    }

    public static int GetNextInt(this string s, int start = 0)
    {
        return s.GetAll<int>(Patterns.Int, 1, start).First();
    }
    
    public static int Count(this string s, Regex regex)
    {
        return s.GetAll<object>(regex, _ => null).Count();
    }

    public static string Cut(this string s, string search, int extraLeft = 0, int extraRight = 0)
    {
        var i = s.IndexOf(search, StringComparison.Ordinal);
        if (i < 0) return s;
        var start = Math.Max(0, i - extraLeft);
        var length = Math.Min(s.Length - start, search.Length + extraRight);
        return string.Concat(s.AsSpan(0, start), s.AsSpan(i + length));
    }

    public static IEnumerable<string> SplitRegex(this string s, string r) => s.SplitRegex(new Regex(r));

    public static IEnumerable<string> SplitRegex(this string s, Regex r, StringSplitOptions options = StringSplitOptions.None)
    {
        var start = 0;
        while (r.Match(s, start) is var match && match.Success)
        {
            if (start == match.Index && options.HasFlag(StringSplitOptions.RemoveEmptyEntries)) goto Next;
            yield return s[start..match.Index];
            Next:
            start = match.Index + Math.Max(match.Length, 1);
        }
        if (start < s.Length || !options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
        {
            yield return s[start..];
        }
    }

    public static IEnumerable<string> TakeRegex(this string s, string r) => s.TakeRegex(new Regex(r));

    public static IEnumerable<string> TakeRegex(this string s, Regex r)
    {
        var start = 0;
        while (r.Match(s, start) is var match && match.Success)
        {
            yield return s[match.Index..(match.Index + match.Length)];
            start = match.Index + match.Length;
        }
    }

    public static KeyValuePair<string, int> FindFirst(this string source, IEnumerable<string> search)
    {
        return search.With(s => source.AsSpan().IndexOf(s))
            .OrderByValue()
            .FirstOrDefault(pair => pair.Value > -1, new KeyValuePair<string, int>(null, -1));
    }
    
    public static KeyValuePair<string, int> FindLast(this string source, IEnumerable<string> search)
    {
        return search.With(s => source.AsSpan().LastIndexOf(s))
            .OrderByValueDescending()
            .FirstOrDefault(pair => pair.Value > -1, new KeyValuePair<string, int>(null, -1));
    }
    
    public static int IndexOfFirst(this string source, IEnumerable<string> search)
    {
        return source.FindFirst(search).Value;
    }

    public static int IndexOfLast(this string source, IEnumerable<string> search)
    {
        return source.FindLast(search).Value;
    }

    public static string StartsWith(this string source, IEnumerable<string> test, int index = 0)
    {
        return test.First(s => source.AsSpan(index).StartsWith(s));
    }
}