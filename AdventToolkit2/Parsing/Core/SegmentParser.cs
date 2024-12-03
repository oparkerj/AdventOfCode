using System.Runtime.CompilerServices;
using AdventToolkit2.Debugging;
using AdventToolkit2.Parsing.Builtin;
using AdventToolkit2.Parsing.Disambiguation;
using AdventToolkit2.Parsing.Interface;

namespace AdventToolkit2.Parsing.Core;

/// <summary>
/// Provides the ability to build parsers that operate on sections
/// of a string separated by known anchors in the string.
/// 
/// This class is designed to be constructed using an interpolated string.
///
/// Literal values in the interpolated string are used to define anchors, and interpolated
/// values are passed to a <see cref="ParseBuilder"/> per section.
/// For example, the interpolated string $"{a}{b},{c}{d}" would create two parse builders,
/// passing "a" and "b" to the first, and "c" and "d" to the second.
/// During parsing, the input string is split at the first occurrence of each anchor.
/// In this case, the input string is split at the first comma, and the two strings are passed
/// to each of the parse builders.
/// We currently have an output from each of the parse builders, say T1 and T2.
/// A tuple (T1, T2), would be adapted to the desired output type T to get the result.
///
/// The default mode for the parser uses literal portions of the string to specify anchor
/// points, and interpolated values to transform the string. A literal null value will begin
/// parsing the same input again for multiple transformations.
/// Using a null literal with a format specifier allows to select data in the input
/// for further transformation. After selecting data, the parser will then behave like
/// an <see cref="Adapt{TIn,TOut}"/>.
/// </summary>
/// <typeparam name="T">The result parse type.</typeparam>
[InterpolatedStringHandler]
public class SegmentParser<T> : ParseBase<string, T>
{
    private readonly List<string> _anchors = [];

    private readonly List<ParseBuilder> _sections = [];

    // Current section index
    private int _selected = -1;

    // Current number of empty splits
    private int _empty;
    // Whether the parse starts with a literal
    private bool _firstIsLiteral;
    // Whether the parse ends with a literal
    private bool _lastIsLiteral;

    // Most recent constructed parser section
    private IParser? _input;
    
    // Used by the Parse functions to cache the constructed parser
    private IParser<string, T>? _built;
    
    public SegmentParser(int literalLength, int formattedCount, IParseContext context)
        : base(context)
    { }
    
    public SegmentParser(int literalLength, int formattedCount)
        : this(literalLength, formattedCount, DefaultContext.Instance)
    { }

    /// <summary>
    /// Get the input for the current section.
    /// </summary>
    /// <returns></returns>
    private Type GetEffectiveInputType() => _input is null ? typeof(string) : ParseUtil.GetParserTypesOf(_input).OutputType;

    /// <summary>
    /// Create a parse section.
    /// </summary>
    /// <param name="inputType"></param>
    /// <param name="identity">If this section is an identity parse.</param>
    /// <returns></returns>
    private ParseBuilder CreateBuilder(bool identity = false)
    {
        var builder = new ParseBuilder {InputType = GetEffectiveInputType()};
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
    ///
    /// When <paramref name="adapt"/> is true, the output is non-null and can safely
    /// be cast to IParser&lt;string, T&gt;.
    /// Otherwise, this will do all the construction except for converting the
    /// output to type <typeparamref name="T"/>. In this case the output may be null if no
    /// transformation is performed on the input.
    /// </summary>
    /// <param name="adapt"></param>
    /// <returns></returns>
    private IParser? BuildInternal(bool adapt)
    {
        var endEmpty = FlushEmpty();
        var endSection = endEmpty || !_lastIsLiteral;
        var inputType = GetEffectiveInputType();

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
                if (!adapt) return null;
                return ParseAdapt.Adapt(typeof(string), typeof(T), Context) ?? IdentityAdapter.Create(typeof(T));
            }
            if (!endSection && _anchors.Count == 1 && _anchors[0] != string.Empty)
            {
                // Cannot specify just a literal
                Err.InvalidFormat("Invalid parse format. No sections given.");
            }
            // Here means the format consists of only literals and null splits
            var splitParse = _input is null
                ? AnchorSplit.Create(_anchors, _firstIsLiteral, endSection)
                : SplitParse.Create(inputType, _anchors.Count);
            Parsing.Parse.Verbose($"Raw type of parse is {ParseUtil.GetParserTypesOf(splitParse).OutputType})");
            return adapt ? ParseAdapt.Adapt(splitParse, typeof(T), Context) : splitParse;
        }
        
        // If there is one section, then adapt it to the output type.
        if (_sections.Count == 1)
        {
            var single = adapt ? _sections[0].Build<T>(Context) : _sections[0].Current;
            
            // If there are no literals, then the input does not need to be split
            if (!_firstIsLiteral && !_lastIsLiteral) return single;
            
            // This path is not reachable in secondary input mode as literals cannot be specified
            var split = AnchorSplit.Create(_anchors, _firstIsLiteral, endSection);
            var unwrap = TupleAdapter.UnwrapSingle(split);
            return ParseJoin.Create(unwrap, single);
        }

        // If the output type is a tuple, each section will be adapted to the
        // corresponding output type. If not, the raw output type is a tuple of
        // whatever each section produces, which is adapted to the output type.
        var parsers = new IParser[_sections.Count];
        var outputTypes = new Type[_sections.Count];
        for (var i = 0; i < parsers.Length; i++)
        {
            var groupParser = _sections[i].Current;
            parsers[i] = groupParser;
            outputTypes[i] = ParseUtil.GetParserTypesOf(groupParser).OutputType;
        }
        
        var segmentTypes = new Type[_sections.Count];
        Array.Fill(segmentTypes, inputType);
        
        // Adapt the result tuple to the output type
        var inputSplit = _input is null
            ? AnchorSplit.Create(_anchors, _firstIsLiteral, endSection)
            : SplitParse.Create(inputType, _anchors.Count);
        var tupleParser = ParseJoin.Create(inputSplit, TupleAdapter.Create(segmentTypes, outputTypes, parsers));
        Parsing.Parse.Verbose($"Raw type of parse is {ParseUtil.GetParserTypesOf(tupleParser).OutputType}");
        return adapt ? ParseAdapt.Adapt(tupleParser, typeof(T), Context) : tupleParser;
    }

    /// <summary>
    /// Construct the full parser given the current sequence of anchors
    /// and sections.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public IParser<string, T> Build()
    {
        return (IParser<string, T>) ParseJoin.MaybeJoin(_input, BuildInternal(true)!);
    }

    /// <summary>
    /// Execute the parse on a string.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public override T Parse(string input)
    {
        Parsing.Parse.Verbose($"========== Begin parse ==========");
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
    /// Try to parse a special section out of a string.
    /// A special section is a substring at the beginning of the input
    /// surrounded by the separator char.
    /// For example, with '@' as a separator, and input "@special@rest",
    /// the return value would be "special" and the input string would become "rest".
    /// The special section is only searched at the beginning of the string.
    /// If the opening separator is found but no closing separator is found,
    /// The entire string is assumed to be part of the special section.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    private ReadOnlySpan<char> GetSpecialFormat(ref string format, char separator)
    {
        if (!format.StartsWith(separator)) return ReadOnlySpan<char>.Empty;

        var end = format.IndexOf(separator, 1);

        if (end == -1)
        {
            var result = format.AsSpan(1);
            format = string.Empty;
            return result;
        }
        else
        {
            var result = format[1..end];
            format = format[(end + 1)..];
            return result;
        }
    }

    /// <summary>
    /// This method is called for the literal string portions of the interpolated string.
    /// This adds an anchor to the parse and moves on to the next section.
    /// </summary>
    /// <param name="s"></param>
    public void AppendLiteral(string s)
    {
        if (_input is not null) return;

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
    /// This overload is for the case when "null" is passed into the string interpolation.
    /// Effectively this will begin a new parse section that operates on the same portion
    /// of string as the previous section.
    /// </summary>
    /// <param name="null"></param>
    public void AppendFormatted(Null? @null) => _empty++;

    /// <summary>
    /// This overload is the case when "null" is passed along with a format string.
    /// The format string specifies how to select data to begin transforming all 
    /// the previous sections.
    /// </summary>
    /// <param name="null"></param>
    /// <param name="format"></param>
    public void AppendFormatted(Null? @null, string format)
    {
        // TODO experiment with more granular selection
        _input = ParseJoin.MaybeJoin(_input, BuildInternal(false));
        
        _anchors.Clear();
        _sections.Clear();
        _selected = -1;
        _lastIsLiteral = false;
        _firstIsLiteral = false;
    }

    /// <inheritdoc cref="AppendFormatted(Range, string)"/>
    public void AppendFormatted(Range range) => AppendFormatted(range, string.Empty);

    /// <summary>
    /// Append a special format specifier that affects the parser.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="format"></param>
    public void AppendFormatted(Range range, string format)
    {
        if (range.Equals(..))
        {
            AppendSpecial(format);
        }
        else
        {
            AppendFormatted<Range>(range, format);
        }
    }

    public void AppendSpecial(ReadOnlySpan<char> format)
    {
        while (!format.IsEmpty)
        {
            var end = format.IndexOf(';');
            ReadOnlySpan<char> cmd;
            if (end > -1)
            {
                cmd = format[..end];
                format = format[(end + 1)..];
            }
            else
            {
                cmd = format;
                format = ReadOnlySpan<char>.Empty;
            }
            
            if (cmd is ['+', .. var plus])
            {
                if (!int.TryParse(plus, out var count))
                {
                    count = 1;
                }
                for (var i = 0; i < count; i++)
                {
                    if (!GetCurrentSlot().TryEnterEnumerable(Context))
                    {
                        Err.InvalidFormat();
                    }
                }

                continue;
            }
            
            if (cmd is ['-', .. var minus])
            {
                if (!int.TryParse(minus, out var count))
                {
                    count = 1;
                }
                for (var i = 0; i < count; i++)
                {
                    if (!GetCurrentSlot().TryExitEnumerable(Context))
                    {
                        Err.InvalidFormat();
                    }
                }

                continue;
            }
            
            AppendFormatted<Range>(.., cmd.ToString());
        }
    }

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

        var specialBefore = GetSpecialFormat(ref format, '@');
        var specialAfter = GetSpecialFormat(ref format, '!');

        if (!specialBefore.IsEmpty)
        {
            AppendSpecial(specialBefore);
        }
        
        GetCurrentSlot().AddStage(item, format, Context);

        if (!specialAfter.IsEmpty)
        {
            AppendSpecial(specialAfter);
        }
    }

    public override IEnumerable<IParser> GetChildren()
    {
        yield return _built ??= Build();
    }
}