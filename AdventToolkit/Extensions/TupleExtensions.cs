using System.Collections.Generic;
using AdventToolkit.Utilities;

namespace AdventToolkit.Extensions;

public static class TupleExtensions
{
    public static (T, T) ToTuple2<T>(this IEnumerable<T> items)
    {
        using var i = new Iter<T>(items);
        return (i.Next(), i.Next());
    }
    
    public static (T, T) ToTuple2<T>(this T[] items)
    {
        return (items[0], items[1]);
    }
    
    public static (T, T, T) ToTuple3<T>(this IEnumerable<T> items)
    {
        using var i = new Iter<T>(items);
        return (i.Next(), i.Next(), i.Next());
    }
    
    public static (T, T, T) ToTuple3<T>(this T[] items)
    {
        return (items[0], items[1], items[2]);
    }
    
    public static (T, T, T, T) ToTuple4<T>(this IEnumerable<T> items)
    {
        using var i = new Iter<T>(items);
        return (i.Next(), i.Next(), i.Next(), i.Next());
    }
    
    public static (T, T, T, T) ToTuple4<T>(this T[] items)
    {
        return (items[0], items[1], items[2], items[3]);
    }
    
    public static (T, T, T, T, T) ToTuple5<T>(this IEnumerable<T> items)
    {
        using var i = new Iter<T>(items);
        return (i.Next(), i.Next(), i.Next(), i.Next(), i.Next());
    }
    
    public static (T, T, T, T, T) ToTuple5<T>(this T[] items)
    {
        return (items[0], items[1], items[2], items[3], items[4]);
    }
    
    public static (T, T, T, T, T, T) ToTuple6<T>(this IEnumerable<T> items)
    {
        using var i = new Iter<T>(items);
        return (i.Next(), i.Next(), i.Next(), i.Next(), i.Next(), i.Next());
    }
    
    public static (T, T, T, T, T, T) ToTuple6<T>(this T[] items)
    {
        return (items[0], items[1], items[2], items[3], items[4], items[5]);
    }
}