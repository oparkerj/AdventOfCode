using AdventToolkit2.Parsing.Builtin;
using AdventToolkit2.Parsing.Interface;

namespace AdventToolkit2.Parsing.Context;

/// <summary>
/// Actions that can be performed on an enumerable.
/// </summary>
public class EnumerableActions : IParserLookup
{
    public bool TryLookup<T>(Type inputType, T value, string extra, IParseContext context, out IParser parser)
    {
        // Flatten a double enumerable when the format is a full range with "*" format
        if (value is Range r && r.Equals(..))
        {
            if (extra == "*")
            {
                // Check if the input is double enumerable
                if (ParseUtil.TryGetInnerType(inputType, context, out var innerType, out var selector)
                    && ParseUtil.TryGetInnerType(innerType, context, out var elementType, out var innerSelector))
                {
                    var select = ParseJoin.MaybeInnerJoin(selector, innerSelector, context, 1);
                    parser = ParseJoin.MaybeJoin(select, EnumerableAdapter.Flatten(elementType));
                    return true;
                }
            }
            else if (extra == "<")
            {
                if (ParseUtil.TryGetInnerType(inputType, context, out var innerType, out var selector))
                {
                    parser = ParseJoin.MaybeJoin(selector, EnumerableAdapter.Sort(innerType));
                    return true;
                }
            }
            else if (extra == ">")
            {
                if (ParseUtil.TryGetInnerType(inputType, context, out var innerType, out var selector))
                {
                    parser = ParseJoin.MaybeJoin(selector, EnumerableAdapter.Sort(innerType, false));
                    return true;
                }
            }
        }

        parser = default!;
        return false;
    }
}