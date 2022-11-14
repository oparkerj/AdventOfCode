using System.Collections.Generic;

namespace AdventToolkit.Extensions;

public static class Collect<T>
{
    public delegate bool OutFunc(out T t);

    public static IEnumerable<T> Out(OutFunc func)
    {
        while (func(out var t)) yield return t;
    }
}