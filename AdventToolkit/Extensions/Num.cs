using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventToolkit.Extensions
{
    public static class Num
    {
        public static int Add(this int a, int b) => a + b;

        public static long Add(this long a, long b) => a + b;

        public static long Add(this int a, long b) => a + b;

        public static long Add(this long a, int b) => a + b;
        
        public static int Sub(this int a, int b) => a - b;

        public static long Sub(this long a, long b) => a - b;

        public static long Sub(this int a, long b) => a - b;

        public static long Sub(this long a, int b) => a - b;
        
        public static int Div(this int a, int b) => a / b;

        public static long Div(this long a, long b) => a / b;

        public static long Div(this int a, long b) => a / b;

        public static long Div(this long a, int b) => a / b;
        
        public static int Mul(this int a, int b) => a * b;

        public static long Mul(this long a, long b) => a * b;

        public static long Mul(this int a, long b) => a * b;

        public static long Mul(this long a, int b) => a * b;

        public static int Xor(this int a, int b) => a ^ b;

        public static bool Eq(this int a, int b) => a == b;
        
        public static bool Neq(this int a, int b) => a != b;

        public static bool Gt(this int a, int b) => a > b;
        
        public static bool Ge(this int a, int b) => a >= b;

        public static bool Lt(this int a, int b) => a < b;
        
        public static bool Le(this int a, int b) => a <= b;
        
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

        public static bool Divides(this int a, int b)
        {
            if (b == 0) return false;
            return a % b == 0;
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

        // Get the digits starting with the ones digit
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
        
        public static IEnumerable<int> Digits(this long i)
        {
            i = Math.Abs(i);
            if (i == 0)
            {
                yield return 0;
                yield break;
            }
            while (i > 0)
            {
                yield return (int) (i % 10);
                i /= 10;
            }
        }
        
        public static IEnumerable<int> DigitsLtr(this int i) => i.Digits().Reverse();
        
        public static IEnumerable<int> DigitsLtr(this long i) => i.Digits().Reverse();

        public static int CircularMod(this int i, int mod)
        {
            if (i < 0) return mod + i % mod;
            return i % mod;
        }
    }
}