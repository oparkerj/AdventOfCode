using System.Runtime.CompilerServices;
using AdventToolkit.New.Parsing.Builtin;
using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;

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

    // Current number of empty splits
    private int _empty;
    // Whether the parse starts with a literal
    private bool _firstIsLiteral;
    // Whether the parse ends with a literal
    private bool _lastIsLiteral;
    
    public SegmentParser(int literalLength, int formattedCount, IReadOnlyParseContext context)
        : base(context)
    { }
    
    public SegmentParser(int literalLength, int formattedCount)
        : this(literalLength, formattedCount, DefaultContext.Instance)
    { }

    /// <summary>
    /// Create a parse section.
    /// </summary>
    /// <param name="identity">If this section is an identity parse.</param>
    /// <returns></returns>
    private ParseBuilder CreateBuilder(bool identity = false)
    {
        var builder = new ParseBuilder {InputType = typeof(string)};
        if (identity)
        {
            builder.SetupIdentity();
        }
        return builder;
    }

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
            // New sections before the current slot will be identity sections
            _sections.Add(CreateBuilder(_sections.Count < _selected));
        }

        return _sections[_selected];
    }

    /// <summary>
    /// Apply empty splits that have been added since
    /// the last literal or section.
    /// </summary>
    /// <returns>True if there were any empty splits, false otherwise.</returns>
    private bool FlushEmpty()
    {
        if (_empty == 0) return false;
        
        if (_lastIsLiteral)
        {
            _lastIsLiteral = false;
            _empty--;
        }
        
        for (var i = 0; i < _empty; i++)
        {
            _anchors.Add(string.Empty);
        }
        _selected += _empty;

        _empty = 0;
        return true;
    }

    /// <summary>
    /// Create the parser that will split a string using the anchors and return
    /// a tuple with the result.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public IParser<string, T> Build()
    {
        var endEmpty = FlushEmpty();
        var endSection = endEmpty || !_lastIsLiteral;

        // Add missing identity parsers if needed
        if (_sections.Count > 0)
        {
            var expected = endSection ? _anchors.Count : _anchors.Count - 1;
            while (_sections.Count < expected)
            {
                _sections.Add(CreateBuilder(true));
            }
        }
        
        // If there are only anchors, then the raw result is a tuple of strings.
        if (_sections.Count == 0)
        {
            if (_anchors.Count == 0)
            {
                // This means the parse format was empty, so just adapt string to the output type
                return (IParser<string, T>) (ParseAdapt.Adapt(typeof(string), typeof(T), Context) ?? IdentityAdapter.Create(typeof(T)));
            }
            if (!endSection && _anchors.Count == 1 && _anchors[0] != string.Empty)
            {
                // Cannot specify just a literal
                throw new ArgumentException("Invalid parse format. No sections given.");
            }
            // Here means the format consists of only literals and null splits
            return (IParser<string, T>) ParseAdapt.Adapt(AnchorSplit.Create(_anchors, _firstIsLiteral, endSection), typeof(T), Context);
        }
        
        // If there is one section, then adapt it to the output type.
        if (_sections.Count == 1)
        {
            var single = _sections[0].Build<T>(Context);
            
            // If there are no literals, then the input does not need to be split
            if (!_firstIsLiteral && !_lastIsLiteral) return single;
            
            var split = AnchorSplit.Create(_anchors, _firstIsLiteral, endSection);
            var unwrap = TupleAdapter.UnwrapSingle(split);
            return (IParser<string, T>) ParseJoin.Create(unwrap, single);
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
                parsers[i] = groupParser;
                outputTypes[i] = ParseUtil.GetParserTypesOf(groupParser).OutputType;
            }
        }
        
        var segmentTypes = new Type[_sections.Count];
        Array.Fill(segmentTypes, typeof(string));
        
        // Adapt the result tuple to the output type
        var tupleParser = ParseJoin.Create(AnchorSplit.Create(_anchors, _firstIsLiteral, endSection), TupleAdapter.Create(segmentTypes, outputTypes, parsers));
        return (IParser<string, T>) ParseAdapt.Adapt(tupleParser, typeof(T), Context);
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
        var parser = _built ??= Build();
        foreach (var s in strings)
        {
            yield return parser.Parse(s);
        }
    }

    /// <summary>
    /// This method is called for the raw string portions of the interpolated string.
    /// This adds an anchor to the parse and moves on to the next section.
    /// </summary>
    /// <param name="s"></param>
    public void AppendLiteral(string s)
    {
        FlushEmpty();
        if (_anchors.Count == 0)
        {
            _firstIsLiteral = true;
        }
        _anchors.Add(s);
        _selected++;
        _lastIsLiteral = true;
    }

    /// <summary>
    /// Empty type that prevents the "null" overload of <see cref="SegmentParser{T}.AppendFormatted"/>
    /// from colliding with other types.
    /// </summary>
    public record struct Null;

    /// <summary>
    /// This overload is for the case when "null" is passed into the string interpolation.
    /// This will call <see cref="AppendLiteral"/> with an empty string.
    /// Effectively this will begin a new parse section that operates on the same portion
    /// of string as the previous section.
    /// </summary>
    /// <param name="null"></param>
    public void AppendFormatted(Null? @null) => _empty++;

    /// <inheritdoc cref="AppendFormatted{TItem}(TItem, string)"/>
    public void AppendFormatted<TItem>(TItem item) => AppendFormatted(item, string.Empty);

    /// <summary>
    /// This method is called for each interpolated value in the interpolated string.
    /// This adds the value to the parse builder in the current slot.
    /// </summary>
    /// <param name="item">Builder value.</param>
    /// <param name="format">Builder value extra data.</param>
    /// <typeparam name="TItem">Builder value type.</typeparam>
    public void AppendFormatted<TItem>(TItem item, string format)
    {
        FlushEmpty();
        _lastIsLiteral = false;
        GetCurrentSlot().AddStage(item, format, Context);
    }

    public override IEnumerable<IParser> GetChildren()
    {
        yield return _built ??= Build();
    }
}