using AdventToolkit2.Parsing.Interface;

namespace AdventToolkit2.Parsing.Context;

/// <summary>
/// Convert the input to string. If it enumerable, each element
/// is converted to string and joined together.
/// </summary>
public class JoinToString : IParserLookup
{
    public static IParser GetJoiner(string separator) => separator == string.Empty ? new Concat() : new Join(separator);

    public bool TryLookup<T>(Type inputType, T value, string extra, IParseContext context, out IParser parser)
    {
        if (value is "")
        {
            // If there is an existing conversion to string, then use it
            if (context.TryLookupAdapter(inputType, typeof(string), out parser))
            {
                return true;
            }

            // Check if enumerable and try to convert the inner element to string
            if (context.TryLookupType(inputType, out var descriptor)
                && descriptor.TryGetInnerType(inputType, out var inner, out var selector)
                && context.TryLookupParser(inner, value, extra, out var innerToString))
            {
                var strings = ParseJoin.MaybeInnerJoin(selector, innerToString, context, 1);
                parser = ParseJoin.Create(strings, GetJoiner(extra));
                return true;
            }
        }

        parser = default!;
        return false;
    }

    /// <summary>
    /// Concat a sequence of strings.
    /// </summary>
    public class Concat : IParser<IEnumerable<string>, string>
    {
        public string Parse(IEnumerable<string> input) => string.Concat(input);
    }

    /// <summary>
    /// Join a sequence of strings.
    /// </summary>
    /// <param name="separator"></param>
    public class Join(string separator) : IParser<IEnumerable<string>, string>
    {
        public string Parse(IEnumerable<string> input) => string.Join(separator, input);
    }
}