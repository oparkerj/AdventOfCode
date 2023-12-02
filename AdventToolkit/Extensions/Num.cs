using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AdventToolkit.Collections;
using MoreLinq;

namespace AdventToolkit.Extensions;

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

    public static int RoundToInt(this double d) => (int) Math.Round(d);

    public static bool Even(this int a) => a % 2 == 0;

    public static bool Odd(this int a) => a % 2 == 1;

    public static bool Even(this BigInteger a) => a.IsEven;

    public static bool Odd(this BigInteger a) => !a.IsEven;

    public static bool True(this int i) => i.AsBool();

    public static bool True(this long l) => l.AsBool();
    
    public static bool False(this int i) => !i.AsBool();

    public static bool False(this long l) => !l.AsBool();

    public static Func<int, int, int> ToIntFunc(Func<int, int, bool> func) => (a, b) => func(a, b).AsInt();

    public static Func<int, int, int> LookupOperator(string s)
    {
        return s switch
        {
            "+" => Add,
            "-" => Sub,
            "*" => Mul,
            "/" => Div,
            "^" => Xor,
            ">" => ToIntFunc(Gt),
            "<" => ToIntFunc(Lt),
            ">=" => ToIntFunc(Ge),
            "<=" => ToIntFunc(Le),
            _ => throw new Exception("Unknown operator.")
        };
    }

    public static BigInteger Factorial(this int i)
    {
        var prod = new BigInteger(1);
        for (var j = 2; j <= i; j++)
        {
            prod *= j;
        }
        return prod;
    }

    public static int Choose(this int n, int k)
    {
        var result = 1;
        for (var i = 1; i <= k; i++)
        {
            result *= n - (k - i);
            result /= i;
        }
        return result;
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

    public static T MaxOrDefault<T>(this IEnumerable<T> nums)
        where T : IComparisonOperators<T, T, bool>
    {
        using var e = nums.GetEnumerator();
        if (!e.MoveNext()) return default;
        var best = e.Current;
        while (e.MoveNext())
        {
            if (e.Current > best) best = e.Current;
        }
        return best;
    }
    
    public static T MinOrDefault<T>(this IEnumerable<T> nums)
        where T : IComparisonOperators<T, T, bool>
    {
        using var e = nums.GetEnumerator();
        if (!e.MoveNext()) return default;
        var best = e.Current;
        while (e.MoveNext())
        {
            if (e.Current < best) best = e.Current;
        }
        return best;
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
    
    public static long Pow(this long i, long power)
    {
        var pow = (ulong) power;
        var ret = 1L;
        while (pow != 0)
        {
            if ((pow & 1) == 1) ret *= i;
            i *= i;
            pow >>= 1;
        }
        return ret;
    }

    public static int Log(this int i, int @base = 0)
    {
        if (@base < 1) return (int) Math.Log(i);
        return (int) (Math.Log(i) / Math.Log(@base));
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
    
    public static IEnumerable<int> Digits(this BigInteger i)
    {
        if (i < 0) i = -i;
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

    public static IEnumerable<int> DigitsLtr(this BigInteger i) => i.Digits().Reverse();

    public static IEnumerable<int> DigitsLtr(this long i) => i.Digits().Reverse();

    public static IEnumerable<bool> Bits(this uint i, int n = 32)
    {
        for (var o = n - 1; o >= 0; o--)
        {
            yield return ((i >> o) & 1) == 1;
        }
    }

    public static int BinaryInt(this string i) => Convert.ToInt32(i, 2);

    public static int BitsToInt(this IEnumerable<int> bits)
    {
        return BinaryInt(Enumerable.TakeLast(bits, 32).Str());
    }

    public static int BitsToInt(this IEnumerable<bool> bits)
    {
        return BinaryInt(Enumerable.TakeLast(bits, 32).AsInts().Str());
    }

    public static BigInteger BitsToInteger(this IEnumerable<bool> bits, bool unsigned = true)
    {
        var bytes = bits.Reverse().Chunk(8).Select(bools => (byte) bools.Reverse().BitsToInt()).ToArray();
        return new BigInteger(bytes.AsSpan(), unsigned);
    }

    public static int CircularMod(this int i, int mod)
    {
        if (i < 0) return (mod + i % mod) % mod;
        return i % mod;
    }
    
    public static long CircularMod(this long i, long mod)
    {
        if (i < 0) return mod + i % mod;
        return i % mod;
    }

    public static int ModRange(this int i, Interval interval)
    {
        return (i - interval.Start).CircularMod(interval.Length) + interval.Start;
    }

    public static bool IsPerfectSquare(this int i)
    {
        var root = i.SqrtFloor();
        return root * root == i;
    }

    public static int SqrtFloor(this int i)
    {
        return (int) Math.Sqrt(i);
    }

    public static bool IsIncreasing(this IEnumerable<int> ints)
    {
        return ints.Pairwise((a, b) => a < b).AllEqual(true);
    }

    public static bool IsDecreasing(this IEnumerable<int> ints)
    {
        return ints.Pairwise((a, b) => a > b).AllEqual(true);
    }

    public static bool IsNonDecreasing(this IEnumerable<int> ints)
    {
        return ints.Pairwise((a, b) => a <= b).AllEqual(true);
    }

    public static bool IsNonIncreasing(this IEnumerable<int> ints)
    {
        return ints.Pairwise((a, b) => a >= b).AllEqual(true);
    }

    public static IEnumerable<int> Positive()
    {
        var i = 1;
        while (true) yield return i++;
    }

    public static int MinCompare(this int i)
    {
        return i < 0 ? int.MaxValue : i;
    }
}