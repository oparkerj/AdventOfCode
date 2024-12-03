using AdventToolkit2.Parsing.Interface;

namespace AdventToolkit2.Parsing.Builtin;

/// <summary>
/// Parser which constructs a dictionary from a sequence of 2-tuples.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class DictParse<TKey, TValue> : IParser<IEnumerable<(TKey, TValue)>, Dictionary<TKey, TValue>>
    where TKey : notnull
{
    /// <summary>
    /// This function specifies how to merge/overwrite a key when a duplicate key already exists.
    /// </summary>
    public readonly Func<TKey, TValue, TValue, TValue> MergeFunc;

    public DictParse() => MergeFunc = (_, _, next) => next;

    public DictParse(Func<TKey, TValue, TValue, TValue> mergeFunc) => MergeFunc = mergeFunc;
    
    public DictParse(Func<TValue, TValue, TValue> mergeFunc) : this((_, current, next) => mergeFunc(current, next)) 
    { }

    public Dictionary<TKey, TValue> Parse(IEnumerable<(TKey, TValue)> input)
    {
        var dict = new Dictionary<TKey, TValue>();

        foreach (var (key, next) in input)
        {
            if (dict.TryGetValue(key, out var current))
            {
                dict[key] = MergeFunc(key, current, next);
            }
            else
            {
                dict[key] = next;
            }
        }
        
        return dict;
    }
}

/// <summary>
/// This class is the same as <see cref="DictParse{TKey,TValue}"/> but swaps the components of
/// the input tuple.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class DictParseSwap<TKey, TValue> : IParser<IEnumerable<(TValue, TKey)>, Dictionary<TKey, TValue>>
    where TKey : notnull
{
    public readonly Func<TKey, TValue, TValue, TValue> MergeFunc;

    public DictParseSwap() => MergeFunc = (_, _, next) => next;

    public DictParseSwap(Func<TKey, TValue, TValue, TValue> mergeFunc) => MergeFunc = mergeFunc;
    
    public DictParseSwap(Func<TValue, TValue, TValue> mergeFunc) : this((_, current, next) => mergeFunc(current, next)) 
    { }

    public Dictionary<TKey, TValue> Parse(IEnumerable<(TValue, TKey)> input)
    {
        var dict = new Dictionary<TKey, TValue>();

        foreach (var (next, key) in input)
        {
            if (dict.TryGetValue(key, out var current))
            {
                dict[key] = MergeFunc(key, current, next);
            }
            else
            {
                dict[key] = next;
            }
        }
        
        return dict;
    }
}