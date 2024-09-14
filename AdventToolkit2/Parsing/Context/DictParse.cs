using AdventToolkit2.Parsing.Interface;
using AdventToolkit2.Reflect;

namespace AdventToolkit2.Parsing.Context;

/// <summary>
/// Parsing support for dictionaries.
///
/// A dictionary will perform a mapping of values.
/// </summary>
public class DictParse : IParserLookup
{
    public bool TryLookup<T>(Type inputType, T value, string extra, IParseContext context, out IParser parser)
    {
        // TODO maybe this could be loosened to IDictionary
        if (!value!.TryGetTypeArgumentsOf(typeof(Dictionary<,>), out var types))
        {
            parser = default!;
            return false;
        }
        
        // If 'a' is specified in the format, the input type will be adapted to the
        // key type of the dictionary.
        if (extra.Contains('a'))
        {
            if (!ParseAdapt.TryAdapt(inputType, types[0], context, out var inputAdapter))
            {
                parser = default!;
                return false;
            }

            var remap = typeof(Remap<,>).NewParserGeneric(types, value);
            parser = ParseJoin.MaybeJoin(inputAdapter, remap);
            return true;
        }
        
        // Without adapting, the input type must be assignable to the dictionary key.
        if (!inputType.IsAssignableTo(types[0]))
        {
            parser = default!;
            return false;
        }

        // If the key and value type are the same, then allow for "loose" mapping.
        // Where any values not found in the dictionary will be mapped to itself.
        parser = types[0] == types[1] 
            ? typeof(Remap<>).NewParserGeneric([types[0]], value)
            : typeof(Remap<,>).NewParserGeneric(types, value);
        return true;
    }
    
    /// <summary>
    /// Map values using a dictionary.
    /// </summary>
    /// <param name="dictionary"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class Remap<TKey, TValue>(Dictionary<TKey, TValue> dictionary) : IParser<TKey, TValue>
        where TKey : notnull
    {
        public TValue Parse(TKey input) => dictionary[input];
    }

    /// <summary>
    /// Map values using a dicitonary, using the key as its own default value.
    /// </summary>
    /// <param name="dictionary"></param>
    /// <typeparam name="T"></typeparam>
    public class Remap<T>(Dictionary<T, T> dictionary) : IParser<T, T>
        where T : notnull
    {
        public T Parse(T input) => dictionary.GetValueOrDefault(input, input);
    }
}