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
            // If the type is passive select, then try to adapt before trying to enumerate
            if (context.TryLookupType(inputType, out var descriptor) && descriptor.PassiveSelect)
            {
                if (TryAdapt(out parser)) return true;
                if (TryEnumerate(out parser)) return true;
            }
            else
            {
                // For type that are not passive select, you normally still want to try to adapt
                // first, but for the "join to string" parser, nested elements are preferred.
                if (TryEnumerate(out parser)) return true;
                if (TryAdapt(out parser)) return true;
            }
        }

        parser = default!;
        return false;

        // If enumerable, then try to convert the inner element to string.
        bool TryEnumerate(out IParser parser)
        {
            if (ParseUtil.TryGetInnerType(inputType, context, out var inner, out var selector)
                && context.TryLookupParser(inner, value, extra, out var innerToString))
            {
                var strings = ParseJoin.MaybeInnerJoin(selector, innerToString, context, 1);
                parser = ParseJoin.Create(strings, GetJoiner(extra));
                return true;
            }

            parser = default!;
            return false;
        }

        // Check if there is an adapter to convert the type to string
        bool TryAdapt(out IParser parser) => context.TryLookupAdapter(inputType, typeof(string), out parser);
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