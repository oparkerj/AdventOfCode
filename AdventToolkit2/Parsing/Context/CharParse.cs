using System.Diagnostics;
using System.Numerics;
using AdventToolkit2.Parsing.Disambiguation;
using AdventToolkit2.Parsing.Interface;
using AdventToolkit2.Reflect;

namespace AdventToolkit2.Parsing.Context;

/// <summary>
/// Provides parse conversions for char.
/// </summary>
public class CharParse : IAdapterLookup<char>
{
    public bool TryLookup(Type to, IParseContext context, out IParser parser)
    {
        if (context.ApplyDisambiguation(typeof(No<char>)))
        {
            parser = default!;
            return false;
        }
        
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
        public T Parse(char input)
        {
            var value = input - '0';
            Debug.Assert(value is >= 0 and <= 9);
            return T.CreateTruncating(value);
        }
    }
}