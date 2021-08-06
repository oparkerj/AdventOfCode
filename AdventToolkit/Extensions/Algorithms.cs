using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Utilities;

namespace AdventToolkit.Extensions
{
    public static class Algorithms
    {
        public static int SumRange(int min, int max)
        {
            return (max - min + 1) * (max + min) / 2;
        }

        public static int Sum(this IEnumerable<int> ints, out int min, out int max)
        {
            bool start = false, end = false;
            var sum = 0;
            min = 0;
            max = 0;
            foreach (var i in ints)
            {
                sum += i;
                if (!start || i < min)
                {
                    start = true;
                    min = i;
                }
                if (!end || i > max)
                {
                    end = true;
                    max = i;
                }
            }
            return sum;
        }
        
        public static int ExtendedEuclidean(this (int a, int m) p, out int x, out int y)
        {
            var (oldR, r) = p;
            var (oldS, s) = (1, 0);
            var (oldT, t) = (0, 1);
            while (r != 0)
            {
                var q = oldR / r;
                (oldR, r) = (r, oldR - q * r);
                (oldS, s) = (s, oldS - q * s);
                (oldT, t) = (t, oldT - q * t);
            }
            x = oldS;
            y = oldT;
            return oldR;
        }

        public static int ModularInverse(this int i, int mod)
        {
            var g = (i, mod).ExtendedEuclidean(out var x, out _);
            if (g != 1) throw new ArithmeticException($"No modular inverse for {i}, {mod}");
            return (x % mod + mod) % mod;
        }
        
        public static long ExtendedEuclidean(this (long a, long m) p, out long x, out long y)
        {
            var (oldR, r) = p;
            var (oldS, s) = (1L, 0L);
            var (oldT, t) = (0L, 1L);
            while (r != 0)
            {
                var q = oldR / r;
                (oldR, r) = (r, oldR - q * r);
                (oldS, s) = (s, oldS - q * s);
                (oldT, t) = (t, oldT - q * t);
            }
            x = oldS;
            y = oldT;
            return oldR;
        }

        public static long ModularInverse(this long i, long mod)
        {
            var g = (i, mod).ExtendedEuclidean(out var x, out _);
            if (g != 1) throw new ArithmeticException($"No modular inverse for {i}, {mod}");
            return (x % mod + mod) % mod;
        }

        /// <summary>
        /// Get the indices of N integers that sum to the given value.
        /// </summary>
        /// <param name="ints">Array of integers.</param>
        /// <param name="n">Number of indices to sum.</param>
        /// <param name="target">Target sum.</param>
        /// <returns>Indices of N integers that sum to the target value.</returns>
        public static int[] NSum(this int[] ints, int n, int target)
        {
            var i = Enumerable.Range(0, n).ToArray();
            while (i[0] < ints.Length - n + 1)
            {
                if (ints.Get(i).Sum() == target) return i;
                var j = n - 1;
                i[j]++;
                if (i[j] < ints.Length - (n - 1 - j)) continue;
                var k = j;
                while (true)
                {
                    k--;
                    if (k < 0) return null;
                    if (i[k] >= ints.Length - (n - k)) continue;
                    var v = ++i[k];
                    for (var m = 1; m < j - k + 1; m++)
                    {
                        i[k + m] = v + m;
                    }
                    break;
                }
            }
            return null;
        }

        // Narrow a set of possibilities using options that only have one possible outcome
        public static void MakeSingles<T, TV>(this Dictionary<T, HashSet<TV>> possible)
        {
            var done = new HashSet<T>();
            for (var i = 0; i < possible.Count - 1; i++)
            {
                var (key, value) = possible.Single(pair => !done.Contains(pair.Key) && pair.Value.Count == 1);
                done.Add(key);
                var remove = value.First();
                foreach (var k in possible.Keys.Where(k => !k.Equals(key)))
                {
                    possible[k].Remove(remove);
                }
            }
        }

        public static T Trace<T>(this T pos, T delta, Func<T, bool> hit)
            where T : IAdd<T>
        {
            while (true)
            {
                pos = pos.Add(delta);
                if (hit(pos)) break;
            }
            return pos;
        }
    }
}