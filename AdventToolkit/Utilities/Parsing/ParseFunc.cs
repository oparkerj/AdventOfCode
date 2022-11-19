using System;

namespace AdventToolkit.Utilities.Parsing;

public static class ParseFunc
{
    public static Func<string, T> Of<T>(Func<string, IFormatProvider, T> func)
    {
        return s => func(s, null);
    }
}