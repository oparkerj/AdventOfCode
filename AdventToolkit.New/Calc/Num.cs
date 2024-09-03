using System.Diagnostics;
using System.Numerics;
using AdventToolkit.New.Space.Bound;

namespace AdventToolkit.New.Calc;

public static class NumVal<T>
    where T : INumber<T>
{
    /// <summary>
    /// Equivalent to <see cref="INumberBase{T}.One"/>
    /// </summary>
    public static readonly T One = T.One;

    /// <summary>
    /// The value of adding <see cref="One"/> to itself.
    /// </summary>
    public static readonly T Two = One + One;
}

/// <summary>
/// Number extensions
/// </summary>
public static class Num
{
    /// <summary>
    /// Get the product of numbers in a sequence.
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Product<T>(this IEnumerable<T> source)
        where T : INumber<T>
    {
        var result = T.One;

        foreach (var n in source)
        {
            result *= n;
        }

        return result;
    }
    
    /// <summary>
    /// Performs the mod operation.
    /// This method always produces non-negative results.
    /// </summary>
    /// <param name="num">Numerator</param>
    /// <param name="mod">Denominator</param>
    /// <typeparam name="T">Number type</typeparam>
    /// <returns>Modulus of the operands.</returns>
    public static T Mod<T>(this T num, T mod)
        where T : INumber<T>
    {
        var r = num % mod;
        return T.IsNegative(r) ? r + mod : r;
    }

    /// <summary>
    /// Similar to <see cref="Math.Sign(int)"/> but the return
    /// type matches the input type.
    /// </summary>
    /// <param name="num"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns><see cref="INumberBase{T}.Zero"/> if the value is zero.
    /// Otherwise, positive or negative <see cref="INumberBase{T}.One"/></returns>
    public static T Sign<T>(this T num)
        where T : INumber<T>
    {
        return T.IsZero(num) ? T.Zero : T.IsPositive(num) ? T.One : -T.One;
    }

    /// <summary>
    /// Compute the greatest common divisor.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Gcd<T>(this T a, T b)
        where T : INumber<T>
    {
        a = T.Abs(a);
        b = T.Abs(b);

        while (b > T.Zero)
        {
            (a, b) = (b, a % b);
        }

        return a;
    }

    /// <summary>
    /// Compute the least common multiple.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Lcm<T>(this T a, T b)
        where T : INumber<T>
    {
        return a / a.Gcd(b) * b;
    }

    /// <summary>
    /// Sum the values between min and max (inclusive).
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T SumSpan<T>(T min, T max)
        where T : INumber<T>
    {
        Debug.Assert(min <= max);
        return (max - min + T.One) * ((max + min) / NumVal<T>.Two);
    }

    /// <summary>
    /// Sum the values between <see cref="INumberBase{TSelf}.One"/> and a given value.
    /// </summary>
    /// <param name="n"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Sum1ToN<T>(T n)
        where T : INumber<T>
    {
        Debug.Assert(n >= T.Zero);
        return n * ((n + T.One) / NumVal<T>.Two);
    }

    /// <summary>
    /// Sum the values between <see cref="INumberBase{T}.Zero"/> and a given value.
    /// The requested value is 0 + 1 + 2 + ... + N
    /// = 0 + SUM(1...N)
    /// = SUM(1...N)
    /// So this method just delegates to <see cref="Sum1ToN{T}"/>.
    /// </summary>
    /// <param name="n"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Sum0ToN<T>(T n)
        where T : INumber<T> =>
        Sum1ToN(n);

    /// <summary>
    /// Sum the values in the interval.
    /// </summary>
    /// <param name="interval"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Sum<T>(this Interval<T> interval)
        where T : INumber<T>
    {
        Debug.Assert(interval.Length >= T.Zero);
        return interval.Length * ((interval.Min + interval.Last) / NumVal<T>.Two);
    }

    /// <summary>
    /// Determine if every value in a sequence is pairwise coprime.
    /// This uses two facts:
    /// For coprime integers, a*b = LCM(a, b)
    /// Iff a is coprime to b and c, then a is coprime to b*c.
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsCoprime<T>(this IEnumerable<T> source)
        where T : INumber<T>
    {
        using var e = source.GetEnumerator();
        if (!e.MoveNext()) return true;

        var product = e.Current;
        var lcm = product;

        while (e.MoveNext())
        {
            product *= e.Current;
            lcm = lcm.Lcm(e.Current);
        }

        return product == lcm;
    }

    /// <summary>
    /// Computes both the GCD of two values and the coefficients to Bézout's identity.
    /// The result is integers x and y such that a*x + b*y = GCD(a, b)
    /// </summary>
    /// <param name="a">First value.</param>
    /// <param name="b">Second value.</param>
    /// <param name="x">First coefficient.</param>
    /// <param name="y">Second coefficient.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>GCD(a, b)</returns>
    public static T ExtendedEuclidean<T>(T a, T b, out T x, out T y)
        where T : INumber<T>
    {
        var (oldR, r) = (a, b);
        var (oldS, s) = (T.One, T.Zero);
        var (oldT, t) = (T.Zero, T.One);
        
        while (r != T.Zero)
        {
            var quotient = oldR / r;
            (oldR, r) = (r, oldR - quotient * r);
            (oldS, s) = (s, oldS - quotient * s);
            (oldT, t) = (t, oldT - quotient * t);
        }
        
        // Bézout coefficients
        x = oldS;
        y = oldT;
        // GCD
        return oldR;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ExtendedEuclidean<T>(this (T a, T b) input, out T x, out T y)
        where T : INumber<T> =>
        ExtendedEuclidean(input.a, input.b, out x, out y);

    /// <summary>
    /// Compute the modular multiplicative inverse.
    /// This is the integer x such that a*x ≡ 1 (mod M).
    /// In other words, M evenly divides a*x - 1, or, a*x / M has a remainder of 1.
    ///
    /// There is only a solution to a*x ≡ b (mod M) if GCD(a, M) divides b, and since
    /// b = 1, then GCD(a, M) must equal 1, meaning a and M must be coprime.
    /// (It also means there is exactly one solution).
    ///
    /// The extended Euclidean algorithm can be used to solve a*x + b*y = GCD(a, b)
    /// In this case, a*x + M*y = 1
    /// or a*x - 1 = -y*M
    /// or a*x ≡ 1 (mod M) (the y goes away because -y*M (mod M) is 0)
    /// So we can use the extended Euclidean algorithm to find the modular inverse.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="mod"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ModularInverse<T>(this T a, T mod)
        where T : INumber<T>
    {
        // ReSharper disable once RedundantAssignment
        var gcd = ExtendedEuclidean(a, mod, out var x, out _);
        Debug.Assert(gcd == T.One, $"{a} does not have an inverse modulo {mod}");
        return x.Mod(mod);
    }

    /// <summary>
    /// Apply the Chinese Remainder Theorem.
    /// For sets of integers a and m where the values of m are pairwise coprime,
    /// the solution N to the relations N ≡ a_i (mod m_i) is
    /// sum(a_i * b_i * M / m_i (mod M))
    /// where M = product(m)
    /// where b_i * M / m_i ≡ 1 (mod m_i)
    /// </summary>
    /// <param name="values"></param>
    /// <param name="mods"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ChineseRemainder<T>(this IEnumerable<T> values, IEnumerable<T> mods)
        where T : INumber<T>
    {
        // Get the product of the moduli
        var m = new List<T>();
        var product = T.One;
        foreach (var n in mods)
        {
            m.Add(n);
            product *= n;
        }
        Debug.Assert(m.IsCoprime(), "Moduli must be pairwise coprime");

        // Sum each element
        var sum = T.Zero;
        var i = 0;
        foreach (var a in values)
        {
            var mi = m[i];
            var x = product / mi;
            var b = x.ModularInverse(mi);
            sum += a.Mod(mi) * b * x;
            i++;
        }

        return sum % product;
    }
}