using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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

}