using System;
using System.Linq;

namespace AdventToolkit.Extensions
{
    public static class Algorithms
    {
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

        public static int[] NSum(this int[] ints, int n, int target)
        {
            var i = Enumerable.Range(0, n).ToArray();
            while (i[0] < ints.Length - n + 1)
            {
                if (ints.Get(i).Sum() == target) return i;
                for (var j = n - 1; j >= 0; j--)
                {
                    i[j]++;
                    if (i[j] >= ints.Length - (n - 1 - j))
                    {
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
                    break;
                }
            }
            return null;
        }
    }
}