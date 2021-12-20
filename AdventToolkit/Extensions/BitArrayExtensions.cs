using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventToolkit.Extensions;

public static class BitArrayExtensions
{
    public static BitArray AllHexBits(this string s)
    {
        IEnumerable<bool> HexBits(char c)
        {
            const int hexOffset = 'a' - 10;
            var value = c is >= '0' and <= '9' ? c - '0' : char.ToLowerInvariant(c) - hexOffset;
            yield return ((value >> 3) & 1) == 1;
            yield return ((value >> 2) & 1) == 1;
            yield return ((value >> 1) & 1) == 1;
            yield return (value & 1) == 1;
        }
    
        var array = new BitArray(s.Length * 4);
        var i = 0;
        foreach (var b in s.SelectMany(HexBits))
        {
            array.Set(i++, b);
        }
        return array;
    }

    public static BitArray ToBitArray(this string s, char set)
    {
        var array = new BitArray(s.Length);
        for (var i = 0; i < s.Length; i++)
        {
            array[i] = s[i] == set;
        }
        return array;
    }

    public static IEnumerable<bool> Values(this BitArray bits) => bits.Cast<bool>();

    public static bool TryConsume(this BitArray bits, int n, out BigInteger value)
    {
        if (bits.Length < n)
        {
            value = default;
            return false;
        }
        value = bits.Values().Take(n).BitsToInteger();
        bits.RightShift(n);
        bits.Length -= n;
        return true;
    }
    
    public static BigInteger Consume(this BitArray bits, int n)
    {
        if (bits.TryConsume(n, out var value)) return value;
        throw new Exception("Not enough bits.");
    }

    public static bool TryConsumeInt(this BitArray bits, int n, out int value)
    {
        if (bits.Length < n || n > 32)
        {
            value = default;
            return false;
        }
        value = bits.Values().Take(n).BitsToInt();
        bits.RightShift(n);
        bits.Length -= n;
        return true;
    }

    public static int ConsumeInt(this BitArray bits, int n = 32)
    {
        if (n > 32) throw new Exception("Too many bits for integer.");
        if (bits.TryConsume(n, out var value)) return (int) value;
        throw new Exception("Not enough bits.");
    }

    public static BitArray ConsumeArray(this BitArray bits, int n)
    {
        var array = new BitArray(bits.Values().Take(n).ToArray());
        bits.RightShift(n);
        bits.Length -= n;
        return array;
    }
}