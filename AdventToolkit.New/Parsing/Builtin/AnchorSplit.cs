using AdventToolkit.New.Algorithms;
using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing.Builtin;

/// <summary>
/// Anchor split helper methods.
/// </summary>
public static class AnchorSplit
{
    /// <summary>
    /// Create an anchor split from a list of anchors.
    /// </summary>
    /// <param name="anchors"></param>
    /// <returns></returns>
    public static IParser Create(List<string> anchors)
    {
        var tupleType = Types.CreateTupleType(typeof(string), anchors.Count);
        return typeof(AnchorSplit<>).NewParserGeneric([tupleType], anchors);
    }
}

/// <summary>
/// Splits a string using multiple split points.
/// </summary>
/// <typeparam name="T"></typeparam>
public class AnchorSplit<T> : IStringParser<T>
{
    private readonly int _size;
    private readonly Type[] _outputType;
    private readonly List<string> _splits;

    /// <summary>
    /// Create an anchor split using a list of anchors.
    ///
    /// One split is represented by a sequence of empty strings followed
    /// by a split value.
    /// </summary>
    /// <param name="anchors"></param>
    public AnchorSplit(List<string> anchors)
    {
        _size = typeof(T).GetTupleSize();
        _outputType = new Type[_size];
        Array.Fill(_outputType, typeof(string));

        // If every string is empty, then every section will receive the entire input string
        if (anchors.All(s => s == string.Empty))
        {
            _splits = new List<string>();
            return;
        }
        
        // Reverse every split section. Every sequence of empty strings followed
        // by a split value will be reversed. This is so splits can be processed
        // in order during parsing.
        _splits = new List<string>(anchors.Count);
        var current = 0;
        for (var i = 0; i < anchors.Count; i++)
        {
            if (anchors[i] == string.Empty) continue;
            
            _splits.Add(anchors[i]);
            
            var repeat = Math.Max(0, i - current - 1);
            for (var j = 0; j < repeat; j++)
            {
                _splits.Add(string.Empty);
            }
            
            current = i + 1;
        }
    }

    public T Parse(ReadOnlySpan<char> span)
    {
        var parts = new object[_size];
        
        if (_splits.Count == 0)
        {
            Array.Fill(parts, span.ToString());
        }
        else
        {
            var last = string.Empty;
            for (var i = 0; i < parts.Length; i++)
            {
                if (i < _splits.Count)
                {
                    var split = _splits[i];
                    if (split.Length > 0)
                    {
                        var at = span.IndexOf(split);
                        last = span[..at].ToString();
                        span = span[(at + 1)..];
                    }
                }
                else
                {
                    last = span.ToString();
                }
                
                parts[i] = last;
            }
        }
        
        return (T) Types.CreateTuple(_outputType, parts);
    }
}