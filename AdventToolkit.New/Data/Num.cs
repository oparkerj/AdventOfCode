using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace AdventToolkit.New.Data;

/// <summary>
/// Wrapper type for a number.
/// </summary>
/// <param name="Value"></param>
/// <typeparam name="T"></typeparam>
public record struct Num<T>(T Value) : INumber<Num<T>>
    where T : struct, INumber<T>
{
    public static Num<T> AdditiveIdentity => new(T.AdditiveIdentity);

    public static Num<T> MultiplicativeIdentity => new(T.MultiplicativeIdentity);

    public static Num<T> One => new(T.One);

    public static int Radix => T.Radix;

    public static Num<T> Zero => new(T.Zero);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);

    public int CompareTo(Num<T> other) => Value.CompareTo(other.Value);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return Value.ToString(format, formatProvider);
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        return Value.TryFormat(destination, out charsWritten, format, provider);
    }

    public static Num<T> Parse(string s, IFormatProvider? provider)
    {
        return new Num<T>(T.Parse(s, provider));
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out Num<T> result)
    {
        Unsafe.SkipInit(out result);
        return T.TryParse(s, provider, out Unsafe.As<Num<T>, T>(ref result));
    }

    public static Num<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        return new Num<T>(T.Parse(s, provider));
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Num<T> result)
    {
        Unsafe.SkipInit(out result);
        return T.TryParse(s, provider, out Unsafe.As<Num<T>, T>(ref result));
    }
    
    public static Num<T> Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        return new Num<T>(T.Parse(s, style, provider));
    }

    public static Num<T> Parse(string s, NumberStyles style, IFormatProvider? provider)
    {
        return new Num<T>(T.Parse(s, style, provider));
    }
    
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Num<T> result)
    {
        Unsafe.SkipInit(out result);
        return T.TryParse(s, style, provider, out Unsafe.As<Num<T>, T>(ref result));
    }

    public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out Num<T> result)
    {
        Unsafe.SkipInit(out result);
        return T.TryParse(s, style, provider, out Unsafe.As<Num<T>, T>(ref result));
    }

    public static implicit operator T(Num<T> num) => num.Value;

    public static implicit operator Num<T>(T t) => new(t);

    public static explicit operator bool(Num<T> num) => num.Value != T.Zero;

    public static implicit operator Num<T>(bool b) => new(b ? T.One : T.Zero);

    public static Num<T> operator +(Num<T> left, Num<T> right) => new(left.Value + right.Value);

    public static Num<T> operator -(Num<T> left, Num<T> right) => new(left.Value - right.Value);
    
    public static Num<T> operator *(Num<T> left, Num<T> right) => new(left.Value * right.Value);
    
    public static Num<T> operator /(Num<T> left, Num<T> right) => new(left.Value / right.Value);
    
    public static Num<T> operator %(Num<T> left, Num<T> right) => new(left.Value % right.Value);
    
    public static bool operator >(Num<T> left, Num<T> right) => left.Value > right.Value;

    public static bool operator >=(Num<T> left, Num<T> right) => left.Value >= right.Value;

    public static bool operator <(Num<T> left, Num<T> right) => left.Value < right.Value;

    public static bool operator <=(Num<T> left, Num<T> right) => left.Value <= right.Value;

    public static Num<T> operator --(Num<T> value) => new(value.Value - T.One);
    
    public static Num<T> operator ++(Num<T> value) => new(value.Value + T.One);
    
    public static Num<T> operator -(Num<T> value) => new(-value.Value);

    public static Num<T> operator +(Num<T> value) => new(+value.Value);

    public static Num<T> Abs(Num<T> value) => new(T.Abs(value.Value));

    public static bool IsCanonical(Num<T> value) => T.IsCanonical(value.Value);

    public static bool IsComplexNumber(Num<T> value) => T.IsComplexNumber(value.Value);

    public static bool IsEvenInteger(Num<T> value) => T.IsEvenInteger(value.Value);

    public static bool IsFinite(Num<T> value) => T.IsFinite(value.Value);

    public static bool IsImaginaryNumber(Num<T> value) => T.IsImaginaryNumber(value.Value);

    public static bool IsInfinity(Num<T> value) => T.IsInfinity(value.Value);

    public static bool IsInteger(Num<T> value) => T.IsInteger(value.Value);

    public static bool IsNaN(Num<T> value) => T.IsNaN(value.Value);

    public static bool IsNegative(Num<T> value) => T.IsNegative(value.Value);

    public static bool IsNegativeInfinity(Num<T> value) => T.IsNegativeInfinity(value.Value);

    public static bool IsNormal(Num<T> value) => T.IsNormal(value.Value);

    public static bool IsOddInteger(Num<T> value) => T.IsOddInteger(value.Value);

    public static bool IsPositive(Num<T> value) => T.IsPositive(value.Value);

    public static bool IsPositiveInfinity(Num<T> value) => T.IsPositiveInfinity(value.Value);

    public static bool IsRealNumber(Num<T> value) => T.IsRealNumber(value.Value);

    public static bool IsSubnormal(Num<T> value) => T.IsSubnormal(value.Value);

    public static bool IsZero(Num<T> value) => T.IsZero(value.Value);

    public static Num<T> MaxMagnitude(Num<T> x, Num<T> y)
    {
        return new Num<T>(T.MaxMagnitude(x.Value, y.Value));
    }

    public static Num<T> MaxMagnitudeNumber(Num<T> x, Num<T> y)
    {
        return new Num<T>(T.MaxMagnitudeNumber(x.Value, y.Value));
    }

    public static Num<T> MinMagnitude(Num<T> x, Num<T> y)
    {
        return new Num<T>(T.MinMagnitude(x.Value, y.Value));
    }

    public static Num<T> MinMagnitudeNumber(Num<T> x, Num<T> y)
    {
        return new Num<T>(T.MinMagnitudeNumber(x.Value, y.Value));
    }

    public static bool TryConvertFromChecked<TOther>(TOther value, out Num<T> result) where TOther : INumberBase<TOther>
    {
        Unsafe.SkipInit(out result);
        return T.TryConvertFromChecked(value, out Unsafe.As<Num<T>, T>(ref result));
    }

    public static bool TryConvertFromSaturating<TOther>(TOther value, out Num<T> result) where TOther : INumberBase<TOther>
    {
        Unsafe.SkipInit(out result);
        return T.TryConvertFromSaturating(value, out Unsafe.As<Num<T>, T>(ref result));
    }

    public static bool TryConvertFromTruncating<TOther>(TOther value, out Num<T> result) where TOther : INumberBase<TOther>
    {
        Unsafe.SkipInit(out result);
        return T.TryConvertFromTruncating(value, out Unsafe.As<Num<T>, T>(ref result));
    }

    public static bool TryConvertToChecked<TOther>(Num<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        return T.TryConvertToChecked(value.Value, out result);
    }

    public static bool TryConvertToSaturating<TOther>(Num<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        return T.TryConvertToSaturating(value.Value, out result);
    }

    public static bool TryConvertToTruncating<TOther>(Num<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        return T.TryConvertToTruncating(value.Value, out result);
    }
}