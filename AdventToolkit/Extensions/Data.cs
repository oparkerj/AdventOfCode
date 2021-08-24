using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MoreLinq;

namespace AdventToolkit.Extensions
{
    public static class Data
    {
        public static IEnumerable<T> Without<T>(this IEnumerable<T> enumerable, T value)
        {
            return enumerable.Where(item => !Equals(item, value));
        }
        
        public static IEnumerable<string> JoinEach(this IEnumerable<string[]> ie, string sep = null)
        {
            if (sep == null) return ie.Select(strings => string.Concat(strings));
            return ie.Select(strings => string.Join(sep, strings));
        }

        public static bool AllEqual<T>(this IEnumerable<T> source, T value)
        {
            return source.All(arg => Equals(arg, value));
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

        public static string Str<T>(this IEnumerable<T> source)
        {
            return string.Concat(source);
        }

        public static IEnumerable<string> ToStrings<T>(this IEnumerable<T> items)
        {
            return items.Select(arg => arg.ToString());
        }

        public static string[] Csv(this string s, bool space = false)
        {
            return space ? s.Split(", ") : s.Split(',');
        }

        public static string ToCsv<T>(this IEnumerable<T> source)
        {
            return string.Join(',', source);
        }

        public static IEnumerable<int> Ints(this IEnumerable<string> strings)
        {
            return strings.Select(int.Parse);
        }

        public static IEnumerable<int> Ints(this IEnumerable<char> chars)
        {
            return chars.Select(c => c - '0');
        }

        public static int AsInt<T>(this IEnumerable<T> source)
        {
            return int.Parse(source.Str());
        }

        public static int AsInt(this bool b) => b ? 1 : 0;

        public static bool AsBool(this int i) => i != 0;
        
        public static bool AsBool(this long i) => i != 0;

        public static IEnumerable<long> Longs(this IEnumerable<string> strings)
        {
            return strings.Select(long.Parse);
        }

        public static IEnumerable<long> Longs(this IEnumerable<int> ints)
        {
            return ints.Select(i => (long) i);
        }

        public static IEnumerable<int> Digits(this int i)
        {
            i = Math.Abs(i);
            if (i == 0)
            {
                yield return 0;
                yield break;
            }
            while (i > 0)
            {
                yield return i % 10;
                i /= 10;
            }
        }

        public static IEnumerable<KeyValuePair<T, int>> Frequencies<T>(this IEnumerable<T> source)
        {
            return source.CountBy(arg => arg);
        }

        public static IEnumerable<TU> QuickMap<T, TU>(this IEnumerable<T> source, T value, TU equal, TU notEqual)
        {
            return source.Select(arg => Equals(arg, value) ? equal : notEqual);
        }

        public static IEnumerable<IEnumerable<TU>> QuickMap<T, TU>(this IEnumerable<IEnumerable<T>> source, T value, TU equal, TU notEqual)
        {
            return source.Select(items => items.QuickMap(value, equal, notEqual));
        }

        public static int Pow(this int i, int power)
        {
            var pow = (uint) power;
            var ret = 1;
            while (pow != 0)
            {
                if ((pow & 1) == 1) ret *= i;
                i *= i;
                pow >>= 1;
            }
            return ret;
        }

        // Repeatedly apply a function to a value
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
            return ints.Longs().Aggregate((a, b) => a * b);
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

        public static int Count<T>(this IEnumerable<T> source, T item)
        {
            return source.Count(i => Equals(i, item));
        }

        public static int CountValues<T, TV>(this IEnumerable<KeyValuePair<T, TV>> pairs, TV value)
        {
            return pairs.Count(pair => Equals(pair.Value, value));
        }
        
        public static int CountValues<T, TV>(this IEnumerable<KeyValuePair<T, TV>> pairs, Func<TV, bool> func)
        {
            return pairs.Count(pair => func(pair.Value));
        }

        public static IEnumerable<T> Sorted<T>(this IEnumerable<T> items)
        {
            return items.OrderBy(arg => arg);
        }
        
        public static bool Matches(this string s, string regex)
        {
            return Regex.IsMatch(s, regex);
        }

        public static IEnumerable<T> EnumerateSingle<T>(this T t)
        {
            yield return t;
        }

        public delegate bool Selection<in T, TOut>(T input, out TOut output);

        public static IEnumerable<TOut> SelectWhere<T, TOut>(this IEnumerable<T> source, Selection<T, TOut> selection)
        {
            foreach (var item in source)
            {
                if (!selection(item, out var output)) continue;
                yield return output;
            }
        }

        public static TV GetOrSetValue<T, TV>(this Dictionary<T, TV> dict, T key, Func<TV> func)
        {
            if (dict.TryGetValue(key, out var value)) return value;
            return dict[key] = func();
        }

        public static IEnumerable<(TA, TB)[]> Split<TA, TB>(this (TA, TB)[] pairs, TA a, TB b, bool purgeEmpty = true)
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

        public static IEnumerable<T[]> MakeUnique<T>(this IEnumerable<T[]> arr)
        {
            foreach (var a in arr)
            {
                var copy = new T[a.Length];
                Array.Copy(a, copy, copy.Length);
                yield return copy;
            }
        }

        public static bool First<T>(this IEnumerable<T> source, Func<T, bool> func, out T first)
        {
            foreach (var item in source)
            {
                if (!func(item)) continue;
                first = item;
                return true;
            }
            first = default;
            return false;
        }

        public static IEnumerable<IList<T>> Subsequences<T>(this IEnumerable<T> source)
        {
            var list = source.ToList();
            for (var i = 1; i <= list.Count; i++)
            {
                for (var j = 0; j < list.Count - (i - 1); j++)
                {
                    yield return list.GetRange(j, i);
                }
            }
        }

        public static IEnumerable<T> WithoutSequence<T>(this IEnumerable<T> source, IEnumerable<T> other)
        {
            var section = other.ToList();
            var main = new List<T>(section.Count);
            foreach (var item in source)
            {
                if (main.Count < section.Count) main.Add(item);
                if (main.Count < section.Count) continue;
                for (var i = 1; i <= section.Count; i++)
                {
                    if (!Equals(main[^i], section[^i])) goto Advance;
                }
                main.Clear();
                continue;
                Advance:
                yield return main[0];
                main.RemoveAt(0);
            }
            foreach (var item in main)
            {
                yield return item;
            }
        }

        public static IEnumerable<T> Endless<T>(this IEnumerable<T> source)
        {
            var list = new List<T>();
            foreach (var item in source)
            {
                yield return item;
                list.Add(item);
            }
            while (true)
            {
                foreach (var item in list)
                {
                    yield return item;
                }
            }
        }

        public static T FirstRepeat<T>(this IEnumerable<T> source)
        {
            var seen = new HashSet<T>();
            foreach (var item in source)
            {
                if (seen.Contains(item)) return item;
                seen.Add(item);
            }
            return default;
        }

        public static Func<T, bool> Not<T>(this Func<T, bool> func)
        {
            return arg => !func(arg);
        }

        public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> source)
        {
            var list = new LinkedList<T>();
            foreach (var item in source)
            {
                list.AddLast(item);
            }
            return list;
        }
    }
}