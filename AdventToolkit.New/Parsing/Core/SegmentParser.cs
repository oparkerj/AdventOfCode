using System.Runtime.CompilerServices;
using AdventToolkit.New.Algorithms;
using AdventToolkit.New.Parsing.Builtin;
using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing.Core;

/// <summary>
/// Provides the ability to build parsers that operate on sections
/// of a string separated by known anchors in the string.
///
/// This class is designed to be constructed using an interpolated string.
/// </summary>
/// <typeparam name="T">The result parse type.</typeparam>
[InterpolatedStringHandler]
public class SegmentParser<T> : ParseBase<string, T>
{
    private List<string> _anchors = [];

    private List<ParseBuilder> _sections = [];

    private int _selected = -1;

    private IParser<string, T>? _built;
    
    public SegmentParser(int literalLength, int formattedCount, IReadOnlyParseContext context)
        : base(context)
    { }
    
    public SegmentParser(int literalLength, int formattedCount)
        : this(literalLength, formattedCount, DefaultContext.Instance)
    { }

    /// <summary>
    /// Get the parse builder for the selected slot.
    /// </summary>
    /// <returns>Current builder.</returns>
    private ParseBuilder GetCurrentSlot()
    {
        // If the first thing being added is a parse section, then insert an empty anchor.
        if (_selected < 0)
        {
            _anchors.Add(string.Empty);
            _selected = 0;
        }

        // Create missing builders
        while (_sections.Count <= _selected)
        {
            _sections.Add(new ParseBuilder {InputType = typeof(string)});
        }

        return _sections[_selected];
    }

    /// <summary>
    /// Create the parser that will split a string using the anchors and return
    /// a tuple with the result.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private IParser<string, T> Build()
    {
        // If there are only anchors, then the raw result is a tuple of strings.
        if (_sections.Count == 0)
        {
            if (_anchors.Count == 0)
            {
                throw new ArgumentException("No sections were given.");
            }
            return (IParser<string, T>) ParseUtil.Adapt(AnchorSplit.Create(_anchors), typeof(T), Context);
        }
        
        // If there is one section, then adapt it to the output type.
        if (_sections.Count == 1)
        {
            return _sections[0].Build<T>(Context);
        }
        
        var parsers = new IParser[_sections.Count];
        
        // If the output type is a tuple, each section will be adapted to the
        // corresponding output type. If not, the raw output type is a tuple of
        // whatever each section produces, and the resulting tuple is adapted
        // to the output type.
        if (typeof(T).TryGetTupleTypes(out var outputTypes))
        {
            if (outputTypes.Length != _sections.Count)
            {
                throw new ArgumentException("Output tuple does not match number of groups.");
            }
            
            // Output types are known
            for (var i = 0; i < parsers.Length; i++)
            {
                parsers[i] = _sections[i].Build(Context, outputTypes[i]);
            }
        }
        else
        {
            outputTypes = new Type[_sections.Count];
            
            // Output types are whatever each section produces.
            for (var i = 0; i < parsers.Length; i++)
            {
                var groupParser = _sections[i].Current;
                var (_, output) = ParseUtil.GetParserTypesOf(groupParser);
                parsers[i] = groupParser;
                outputTypes[i] = output;
            }
        }
        
        var segmentTypes = new Type[_sections.Count];
        Array.Fill(segmentTypes, typeof(string));
        
        // Adapt the result tuple to the output type
        var tupleParser = ParseJoin.Create(AnchorSplit.Create(_anchors), TupleAdapter.Create(segmentTypes, outputTypes, parsers));
        return (IParser<string, T>) ParseUtil.Adapt(tupleParser, typeof(T), Context);
    }

    /// <summary>
    /// Execute the parse on a string.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public override T Parse(string input)
    {
        _built ??= Build();
        return _built.Parse(input);
    }

    /// <summary>
    /// Execute the parse on a sequence of strings.
    /// </summary>
    /// <param name="strings"></param>
    /// <returns></returns>
    public IEnumerable<T> ParseMany(IEnumerable<string> strings)
    {
        _built ??= Build();
        return strings.Select(_built.Parse);
    }

    /// <summary>
    /// This method is called for the raw string portions of the interpolated string.
    /// This adds an anchor to the parse and moves on to the next section.
    /// </summary>
    /// <param name="s"></param>
    public void AppendLiteral(string s)
    {
        _anchors.Add(s);
        _selected++;
    }
    
    public void AppendFormatted<TItem>(TItem item) => AppendFormatted(item, string.Empty);

    /// <summary>
    /// This method is called for each interpolated value in the interpolated string.
    /// This adds the value to the parse builder in the current slot.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="format"></param>
    /// <typeparam name="TItem"></typeparam>
    public void AppendFormatted<TItem>(TItem item, string format)
    {
        GetCurrentSlot().AddStage(item, format, Context);
    }

    // TODO alignment parameter

    public override IEnumerable<IParser> GetChildren()
    {
        yield return _built ??= Build();
    }
}