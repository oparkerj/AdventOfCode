using System;
using System.Numerics;

namespace AdventToolkit.Utilities.Computer;

public class MemNumber<T> : Mem<T>
    where T : struct, IBinaryInteger<T>
{
    public MemNumber(Func<T> read, Action<T> write) : base(read, write) { }
    
    public MemNumber(Mem<T> mem) : this(mem.Read, mem.Write) { }

    public static T operator +(MemNumber<T> left, MemNumber<T> right) => left.Value + right.Value;

    public static T operator +(MemNumber<T> left, T right) => left.Value + right;

    public static T operator +(T left, MemNumber<T> right) => left + right.Value;

    public static T operator +(MemNumber<T> mem) => +mem.Value;

    public static T operator -(MemNumber<T> left, MemNumber<T> right) => left.Value - right.Value;

    public static T operator -(MemNumber<T> left, T right) => left.Value - right;

    public static T operator -(T left, MemNumber<T> right) => left - right.Value;

    public static T operator -(MemNumber<T> mem) => -mem.Value;
    
    public static T operator *(MemNumber<T> left, MemNumber<T> right) => left.Value * right.Value;

    public static T operator *(MemNumber<T> left, T right) => left.Value * right;

    public static T operator *(T left, MemNumber<T> right) => left * right.Value;
    
    public static T operator /(MemNumber<T> left, MemNumber<T> right) => left.Value / right.Value;

    public static T operator /(MemNumber<T> left, T right) => left.Value / right;

    public static T operator /(T left, MemNumber<T> right) => left / right.Value;
    
    public static T operator %(MemNumber<T> left, MemNumber<T> right) => left.Value % right.Value;

    public static T operator %(MemNumber<T> left, T right) => left.Value % right;

    public static T operator %(T left, MemNumber<T> right) => left % right.Value;
    
    public static bool operator <(MemNumber<T> left, MemNumber<T> right) => left.Value < right.Value;

    public static bool operator <(MemNumber<T> left, T right) => left.Value < right;

    public static bool operator <(T left, MemNumber<T> right) => left < right.Value;
    
    public static bool operator >(MemNumber<T> left, MemNumber<T> right) => left.Value > right.Value;

    public static bool operator >(MemNumber<T> left, T right) => left.Value > right;

    public static bool operator >(T left, MemNumber<T> right) => left > right.Value;
    
    public static bool operator <=(MemNumber<T> left, MemNumber<T> right) => left.Value <= right.Value;

    public static bool operator <=(MemNumber<T> left, T right) => left.Value <= right;

    public static bool operator <=(T left, MemNumber<T> right) => left <= right.Value;
    
    public static bool operator >=(MemNumber<T> left, MemNumber<T> right) => left.Value >= right.Value;

    public static bool operator >=(MemNumber<T> left, T right) => left.Value >= right;

    public static bool operator >=(T left, MemNumber<T> right) => left >= right.Value;

    public static bool operator ==(MemNumber<T> left, MemNumber<T> right) => left!.Value == right!.Value;

    public static bool operator ==(MemNumber<T> left, T right) => left!.Value == right;

    public static bool operator ==(T left, MemNumber<T> right) => left == right!.Value;
    
    public static bool operator !=(MemNumber<T> left, MemNumber<T> right) => left!.Value != right!.Value;

    public static bool operator !=(MemNumber<T> left, T right) => left!.Value != right;

    public static bool operator !=(T left, MemNumber<T> right) => left != right!.Value;
    
    public static T operator >>(MemNumber<T> left, int right) => left.Value >> right;
    
    public static T operator <<(MemNumber<T> left, int right) => left.Value << right;
    
    public static T operator &(MemNumber<T> left, MemNumber<T> right) => left.Value & right.Value;

    public static T operator &(MemNumber<T> left, T right) => left.Value & right;

    public static T operator &(T left, MemNumber<T> right) => left & right.Value;
    
    public static T operator |(MemNumber<T> left, MemNumber<T> right) => left.Value | right.Value;

    public static T operator |(MemNumber<T> left, T right) => left.Value | right;

    public static T operator |(T left, MemNumber<T> right) => left | right.Value;
    
    public static T operator ^(MemNumber<T> left, MemNumber<T> right) => left.Value ^ right.Value;

    public static T operator ^(MemNumber<T> left, T right) => left.Value ^ right;

    public static T operator ^(T left, MemNumber<T> right) => left ^ right.Value;

    public static T operator ~(MemNumber<T> mem) => ~mem.Value;

    public override bool Equals(object obj) => Value.Equals(obj);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}