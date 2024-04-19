using System.Numerics;
using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;

namespace AdventToolkit.New.Parsing.Context;

/// <summary>
/// Provides parse conversions for char.
/// </summary>
public class CharParse : IAdapterLookup<char>
{
    public bool TryLookup(Type to, IParseContext context, out IParser parser)
    {
        // If the result is a number type, parse as a digit
        if (to.TryGetTypeArguments(typeof(INumber<>), out _))
        {
            parser = typeof(CharToDigit<>).NewParserGeneric([to]);
            return true;
        }

        parser = default!;
        return false;
    }

    /// <summary>
    /// Interprets the char as a digit of type <see cref="int"/> and attempts to
    /// convert to a target number type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CharToDigit<T> : IParser<char, T>
        where T : INumber<T>
    {
        public T Parse(char input) => T.CreateTruncating(input - '0');
    }
}