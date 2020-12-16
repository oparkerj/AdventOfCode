using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RegExtract;

namespace AdventToolkit.Extensions
{
    public static class Data
    {
        public static IEnumerable<string> Join(this IEnumerable<string[]> ie, string sep = null)
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

        public static void Swap<T>(this T[] arr, int a, int b)
        {
            var t = arr[a];
            arr[a] = arr[b];
            arr[b] = t;
        }
        
        public static IEnumerable<T> FromFormat<T>(this IEnumerable<string> strings, string regex)
        {
            return strings.Select(s => s.Extract<T>(regex));
        }
    }
}