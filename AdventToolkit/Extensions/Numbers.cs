using System;
using System.Numerics;

namespace AdventToolkit.Extensions
{
    public static class Numbers
    {
        public static BigInteger Factorial(this int i)
        {
            var prod = new BigInteger(1);
            for (var j = 2; j <= i; j++)
            {
                prod *= j;
            }
            return prod;
        }

        public static long Choose(this long n, long k)
        {
            var result = 1L;
            for (var i = 1; i <= k; i++)
            {
                result *= n - (k - i);
                result /= i;
            }
            return result;
        }

        public static int Gcd(this int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            while (b > 0)
            {
                var rem = a % b;
                a = b;
                b = rem;
            }
            return a;
        }
        
        public static long Gcd(this long a, long b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            while (b > 0)
            {
                var rem = a % b;
                a = b;
                b = rem;
            }
            return a;
        }

        public static int Lcm(this int a, int b)
        {
            return a / Gcd(a, b) * b;
        }
        
        public static long Lcm(this long a, long b)
        {
            return a / Gcd(a, b) * b;
        }

        public static void Times(this int i, Action action)
        {
            for (var j = 0; j < i; j++) action();
        }
    }
}