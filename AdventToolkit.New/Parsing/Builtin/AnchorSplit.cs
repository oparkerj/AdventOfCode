using System.Diagnostics;
using AdventToolkit.New.Data;
using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;

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
    /// <param name="anchorFirst">True if the split starts with an anchor, false if a section is first.</param>
    /// <param name="includeLast">Whether there is an element after the last split</param>
    /// <returns></returns>
    public static IParser Create(List<string> anchors, bool anchorFirst, bool includeLast)
    {
        var size = includeLast ? anchors.Count : anchors.Count - 1;
        Debug.Assert(size >= 1);
        var tupleType = Types.CreateTupleType(typeof(string), size);
        return typeof(AnchorSplit<>).NewParserGeneric([tupleType], size, anchors, anchorFirst);
    }
}

/// <summary>
/// Splits a string using multiple split points.
/// </summary>
/// <typeparam name="T"></typeparam>
public class AnchorSplit<T> : IStringParser<T>
{
    private readonly int _size;
    private readonly string[] _splits;
    private readonly bool _anchorFirst;

    /// <summary>
    /// Create an anchor split using a list of anchors.
    /// 
    /// One split is represented by a sequence of empty strings followed
    /// by a split value.
    /// </summary>
    /// <param name="size"></param>
    /// <param name="anchors"></param>
    /// <param name="anchorFirst">Whether the first section appears after the first anchor.</param>
    public AnchorSplit(int size, List<string> anchors, bool anchorFirst)
    {
        Debug.Assert(typeof(T).IsTupleType());
        
        _size = size;
        _anchorFirst = anchorFirst;

        // If every string is empty, then every section will receive the entire input string
        if (anchors.All(s => s == string.Empty))
        {
            _splits = Array.Empty<string>();
            return;
        }
        
        // Reverse every split section. Every sequence of empty strings followed
        // by a split value will be reversed. This is so splits can be processed
        // in order during parsing.
        // If the split starts with a section, drop the first anchor (which should be empty anyway)
        var i = anchorFirst ? 0 : 1;
        var last = i;
        var current = 0;
        _splits = new string[anchors.Count - i];
        for (; i < anchors.Count; i++)
        {
            if (anchors[i] == string.Empty) continue;
            
            // Add the anchor
            _splits[current] = anchors[i];
            // Add the empty sections
            var repeat = i - last;
            _splits.AsSpan(current + 1, repeat).Fill(string.Empty);
            current += repeat + 1;
            last = i + 1;
        }
        
        // Fill the remainder with empty splits
        if (last < _splits.Length)
        {
            _splits.AsSpan(last).Fill(string.Empty);
        }
    }

    public T Parse(ReadOnlySpan<char> span)
    {
        using Arr<object?> parts = new(_size);
        
        if (_splits.Length == 0)
        {
            // There are no splits, every section is the input string
            parts.Span.Fill(span.ToString());
        }
        else
        {
            var offset = 0;
            
            // Shift the input if needed
            if (_anchorFirst)
            {
                var at = span.IndexOf(_splits[0]);
                span = span[(at + 1)..];
                offset++;
            }
            
            var last = span.ToString();
            for (var i = 0; i < parts.Length; i++)
            {
                var effective = i + offset;
                if (effective < _splits.Length)
                {
                    var split = _splits[effective];
                    // If the split is empty, repeat the last section
                    if (split.Length > 0)
                    {
                        var at = span.IndexOf(split);
                        last = span[..at].ToString();
                        span = span[(at + 1)..];
                    }
                }
                else
                {
                    // If there are no more split points, use the remainder of the input
                    last = span.ToString();
                }
                
                parts[i] = last;
            }
        }

        return (T) Types.CreateTuple(typeof(string), parts);
    }
}