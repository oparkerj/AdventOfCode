using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq;

namespace AdventToolkit.Extensions
{
    public static class EnumerableExtensions
    {
        // Simple exclusion of a single value from a sequence
        public static IEnumerable<T> Without<T>(this IEnumerable<T> enumerable, T value)
        {
            return enumerable.Where(item => !Equals(item, value));
        }
        
        // Inner join sequence of string arrays.
        public static IEnumerable<string> JoinEach(this IEnumerable<string[]> ie, string sep = null)
        {
            if (sep == null) return ie.Select(strings => string.Concat(strings));
            return ie.Select(strings => string.Join(sep, strings));
        }

        // Check that every element in the sequence is a particular value
        public static bool AllEqual<T>(this IEnumerable<T> source, T value)
        {
            return source.All(arg => Equals(arg, value));
        }

        // Check that every element in the sequence is equal
        // True for empty sequences
        public static bool AllEqual<T>(this IEnumerable<T> source)
        {
            using var e = source.GetEnumerator();
            if (!e.MoveNext()) return true;
            var first = e.Current;
            while (e.MoveNext())
            {
                if (!Equals(first, e.Current)) return false;
            }
            return true;
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
            return chars.Select(Conversions.AsInt);
        }

        public static IEnumerable<uint> Unsigned(this IEnumerable<int> source)
        {
            return source.Select(i => (uint) i);
        }

        public static int AsInt<T>(this IEnumerable<T> source)
        {
            return int.Parse(source.Str());
        }

        public static int ToInt(this IEnumerable<bool> source)
        {
            var shift = 0;
            var value = 0;
            foreach (var b in source)
            {
                if (b) value |= 1 << shift;
                shift++;
            }
            return value;
        }

        public static IEnumerable<long> Longs(this IEnumerable<string> strings)
        {
            return strings.Select(long.Parse);
        }

        public static IEnumerable<long> Longs(this IEnumerable<int> ints)
        {
            return ints.Select(i => (long) i);
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

        public static int Product(this IEnumerable<int> ints)
        {
            return ints.Aggregate((a, b) => a * b);
        }

        public static long LongProduct(this IEnumerable<int> ints)
        {
            return ints.Longs().Aggregate((a, b) => a * b);
        }

        public static IEnumerable<T> GetFrom<T>(this IEnumerable<int> indices, IList<T> t)
        {
            return t.Get(indices);
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> ie)
        {
            return ie.SelectMany(e => e);
        }

        public static string AsString(this IEnumerable<bool> bools)
        {
            return bools.Select(b => b ? '1' : '0').Str();
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

        public delegate bool Selection<in T, TOut>(T input, out TOut output);

        public static IEnumerable<TOut> SelectWhere<T, TOut>(this IEnumerable<T> source, Selection<T, TOut> selection)
        {
            foreach (var item in source)
            {
                if (!selection(item, out var output)) continue;
                yield return output;
            }
        }

        public static IEnumerable<T> WhereType<T>(this IEnumerable source)
        {
            foreach (var o in source)
            {
                if (o is T t) yield return t;
            }
        }

        public static IEnumerable<T> WhereType<T>(this IEnumerable source, Func<T, bool> predicate)
        {
            foreach (var o in source)
            {
                if (o is T t && predicate(t)) yield return t;
            }
        }

        // Create unique arrays from an enumerable that returns the same array each time
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

        public static bool First<T>(this IEnumerable<T> source, out T first)
        {
            first = default;
            using var e = source.GetEnumerator();
            if (!e.MoveNext()) return false;
            first = e.Current;
            return true;
        }

        public static bool Single<T>(this IEnumerable<T> source, out T single)
        {
            single = default;
            using var e = source.GetEnumerator();
            if (!e.MoveNext()) return false;
            var result = e.Current;
            if (e.MoveNext()) return false;
            single = result;
            return true;
        }

        // Returns all sub ranges of length 1, 2, 3, ..., N where N is the length of the source
        public static IEnumerable<ArraySegment<T>> SubRanges<T>(this IEnumerable<T> source)
        {
            var arr = source.ToArray();
            for (var i = 1; i <= arr.Length; i++)
            {
                for (var j = 0; j < arr.Length - (i - 1); j++)
                {
                    yield return new ArraySegment<T>(arr, j, i);
                }
            }
        }

        public static IEnumerable<T> WithoutSequence<T>(this IEnumerable<T> source, IEnumerable<T> other)
        {
            var section = other.ToArray();
            var main = new List<T>(section.Length);
            foreach (var item in source)
            {
                if (main.Count < section.Length) main.Add(item);
                if (main.Count < section.Length) continue;
                for (var i = 1; i <= section.Length; i++)
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

        // Repeat a source forever
        // The original source is only enumerated once.
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
            // ReSharper disable once IteratorNeverReturns
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

        public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> source)
        {
            var list = new LinkedList<T>();
            foreach (var item in source)
            {
                list.AddLast(item);
            }
            return list;
        }

        public static T SelectMinBy<T, TCompare>(this IEnumerable<T> source, Func<T, TCompare> compare)
            where TCompare : IComparable<TCompare>
        {
            using var e = source.GetEnumerator();
            if (!e.MoveNext()) throw new Exception("Source contains no elements.");
            var min = e.Current;
            var value = compare(min);
            while (e.MoveNext())
            {
                var compareValue = compare(e.Current);
                if (compareValue.CompareTo(value) >= 0) continue;
                min = e.Current;
                value = compareValue;
            }
            return min;
        }

        public static T SelectMinBy<T>(this IEnumerable<T> source, IComparer<T> comparer)
        {
            using var e = source.GetEnumerator();
            if (!e.MoveNext()) throw new Exception("Source contains no elements.");
            var min = e.Current;
            while (e.MoveNext())
            {
                if (comparer.Compare(e.Current, min) >= 0) continue;
                min = e.Current;
            }
            return min;
        }

        public static TCompare SelectMin<T, TCompare>(this IEnumerable<T> source, Func<T, TCompare> compare, IComparer<TCompare> comparer = null)
        {
            comparer ??= Comparer<TCompare>.Default;
            using var e = source.GetEnumerator();
            if (!e.MoveNext()) throw new Exception("Source contains no elements.");
            var value = compare(e.Current);
            while (e.MoveNext())
            {
                var compareValue = compare(e.Current);
                if (comparer.Compare(compareValue, value) >= 0) continue;
                value = compareValue;
            }
            return value;
        }

        public static T SelectMaxBy<T, TCompare>(this IEnumerable<T> source, Func<T, TCompare> compare)
            where TCompare : IComparable<TCompare>
        {
            using var e = source.GetEnumerator();
            if (!e.MoveNext()) throw new Exception("Source contains no elements.");
            var max = e.Current;
            var value = compare(max);
            while (e.MoveNext())
            {
                var compareValue = compare(e.Current);
                if (compareValue.CompareTo(value) <= 0) continue;
                max = e.Current;
                value = compareValue;
            }
            return max;
        }

        public static T SelectMaxBy<T, TCompare>(this IEnumerable<T> source, IComparer<T> comparer)
        {
            using var e = source.GetEnumerator();
            if (!e.MoveNext()) throw new Exception("Source contains no elements.");
            var max = e.Current;
            while (e.MoveNext())
            {
                if (comparer.Compare(e.Current, max) <= 0) continue;
                max = e.Current;
            }
            return max;
        }

        public static TCompare SelectMax<T, TCompare>(this IEnumerable<T> source, Func<T, TCompare> compare, IComparer<TCompare> comparer = null)
        {
            comparer ??= Comparer<TCompare>.Default;
            using var e = source.GetEnumerator();
            if (!e.MoveNext()) throw new Exception("Source contains no elements.");
            var value = compare(e.Current);
            while (e.MoveNext())
            {
                var compareValue = compare(e.Current);
                if (comparer.Compare(compareValue, value) <= 0) continue;
                value = compareValue;
            }
            return value;
        }

        public static T SingleOrDefault<T>(this IEnumerable<T> source, T defaultValue)
        {
            using var e = source.GetEnumerator();
            if (!e.MoveNext()) return defaultValue;
            var result = e.Current;
            return e.MoveNext() ? defaultValue : result;
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> source, T defaultValue)
        {
            using var e = source.GetEnumerator();
            return !e.MoveNext() ? defaultValue : e.Current;
        }

        public static IEnumerable<KeyValuePair<T, TV>> With<T, TV>(this IEnumerable<T> source, Func<T, TV> func)
        {
            return source.Select(item => new KeyValuePair<T, TV>(item, func(item)));
        }

        public static T[] ToArray<T>(this IEnumerable<T> source, int size)
        {
            return source.ToArray(new T[size]);
        }

        // Put an enumerable in an array assuming it can hold all the elements.
        public static T[] ToArray<T>(this IEnumerable<T> source, T[] array)
        {
            var i = 0;
            using var e = source.GetEnumerator();
            while (e.MoveNext())
            {
                array[i] = e.Current;
                i++;
            }
            return array;
        }

        public static IEnumerable<T> ChangeLast<T>(this IEnumerable<T> items, Func<T, T> convert)
        {
            using var e = items.GetEnumerator();
            if (!e.MoveNext()) yield break;
            var last = e.Current;
            while (e.MoveNext())
            {
                yield return last;
                last = e.Current;
            }
            yield return convert(last);
        }

        // Alternative to ToDictionary where duplicate keys are allowed, only the first entry is saved
        public static Dictionary<TKey, TValue> ToDictionaryFirst<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            var d = new Dictionary<TKey, TValue>();
            foreach (var (key, value) in items)
            {
                d.TryAdd(key, value);
            }
            return d;
        }
        
        // Alternative to ToDictionary where duplicate keys are allowed, duplicate entries will overwrite previous occurrences
        public static Dictionary<TKey, TValue> ToDictionaryLast<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            var d = new Dictionary<TKey, TValue>();
            foreach (var (key, value) in items)
            {
                d[key] = value;
            }
            return d;
        }
    }
}