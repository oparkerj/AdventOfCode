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
            while (true)
            {
                if (a == 0) return b;
                var a1 = a;
                a = b % a;
                b = a1;
            }
        }

        public static int Lcm(this int a, int b)
        {
            return a / a.Gcd(b) * b;
        }
    }
}