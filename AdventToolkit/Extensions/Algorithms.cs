using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Utilities.Arithmetic;
using MoreLinq;

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
            return SequencesIncreasing(n, ints.Length, true)
                .FirstOrDefault(indices => ints.Get(indices).Sum() == target);
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

        // Returns all sequences of a given length from {1,1,...,1} to {n,n,...,n}
        // The array yielded by this method is the same array each time.
        // index = true makes the sequence start from 0 and n is exclusive
        public static IEnumerable<int[]> Sequences(int length, int n, bool index = false)
        {
            if (n < (index ? 0 : 1)) yield break;
            var arr = new int[length];
            Array.Fill(arr, index ? 0 : 1);
            Main:
            yield return arr;
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i] == (index ? n - 1 : n)) arr[i] = index ? 0 : 1;
                else
                {
                    arr[i]++;
                    goto Main;
                }
            }
        }

        // Generates all sequences of a given length
        // from {1, 2, 3, ..., length} to {..., n - 2, n - 1, n}
        // The array yielded by this method is the same array each time.
        // index = true makes the sequence start from 0 and n is exclusive
        public static IEnumerable<int[]> SequencesIncreasing(int length, int n, bool index = false)
        {
            if (n < (index ? length - 1 : length)) yield break;
            var arr = Enumerable.Range(index ? 0 : 1, length).ToArray();
            while (true)
            {
                yield return arr;
                // Increase last
                var i = length - 1;
                arr[i]++;
                if (arr[i] <= (index ? n - 1 : n)) continue;
                // On overflow, find next index from end to increase
                var j = i;
                do
                {
                    j--;
                    if (j < 0) yield break;
                }
                while (arr[j] >= (index ? n - 1 : n) - (length - j - 1));
                // Increment current index and set indexes
                // after to increasing numbers.
                var v = ++arr[j];
                for (var m = 1; m < length - j; m++)
                {
                    arr[j + m] = v + m;
                }
            }
        }

        public static long CommonCycle<T, TC>(T data, Func<T, IEnumerable<TC>> cycles, Action<T> step)
        {
            var initial = new Dictionary<int, long>();
            var state = new List<TC>();
            foreach (var (i, cycle) in cycles(data).Index())
            {
                initial[i] = -1;
                state.Add(cycle);
            }
            var steps = 0L;
            while (true)
            {
                if (initial.Values.All(cycle => cycle > -1)) break;
                step(data);
                steps++;
                foreach (var (i, cycle) in cycles(data).Index())
                {
                    if (initial[i] == -1 && Equals(cycle, state[i]))
                    {
                        initial[i] = steps;
                    }
                }
            }
            return initial.Values.Aggregate((a, b) => a.Lcm(b));
        }
    }
}