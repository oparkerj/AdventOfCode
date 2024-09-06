using System.Runtime.CompilerServices;
using AdventToolkit2.Collections;
using AdventToolkit2.Parsing.Builtin;
using AdventToolkit2.Parsing.Disambiguation;
using AdventToolkit2.Parsing.Interface;

namespace AdventToolkit2.Parsing.Core;

/// <summary>
/// Similar to <see cref="SegmentParser{T}"/> but for adapting
/// an arbitrary input type to another type.
///
/// This class can be constructed from an interpolated string.
/// This class does not use anchors, so literal text is ignored.
///
/// A format specifier of "null" will start a new section that adapts
/// the input value again. An implicit null is inserted at the beginning
/// of the format, but it may be specified explicitly.
/// E.g. To create an adapter that makes a copy of the input: T => (T, T)
/// The format specifier is $"{null}{null}"
/// </summary>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
[InterpolatedStringHandler]
public class Adapt<TIn, TOut> : ParseBase<TIn, TOut>
{
    private readonly List<ParseBuilder> _builders = [];

    private IParser<TIn, TOut>? _built;

    public Adapt(int literalLength, int formattedCount, IParseContext context) 
        : base(context)
    { }
    
    public Adapt(int literalLength, int formattedCount)
        : this(literalLength, formattedCount, DefaultContext.Instance)
    { }

    /// <summary>
    /// Add a new builder to the internal list.
    /// </summary>
    private void CreateBuilder()
    {
        _builders.Add(new ParseBuilder {InputType = typeof(TIn)});
    }

    /// <summary>
    /// If no conversions were given, just adapt the input type to the output type.
    /// </summary>
    /// <returns></returns>
    private IParser<TIn, TOut> EmptyAdapt()
    {
        return (IParser<TIn, TOut>) (ParseAdapt.Adapt(typeof(TIn), typeof(TOut), Context) ?? IdentityAdapter.Create(typeof(TOut)));
    }

    /// <summary>
    /// Build the parser that will perform the conversions for this adapt.
    /// </summary>
    /// <returns></returns>
    public IParser<TIn, TOut> Build()
    {
        if (_builders.Count == 0) return EmptyAdapt();
        
        // If there is only a single section, then just build it.
        if (_builders.Count == 1) return !_builders[0].HasValue ? EmptyAdapt() : _builders[0].Build<TIn, TOut>(Context);

        using var inputs = Arr<Type>.Of(typeof(TIn), _builders.Count);
        using var outputs = Arr<Type>.Get(_builders.Count);
        using var parsers = Arr<IParser>.Get(_builders.Count);

        // Get the parsers for each section
        for (var i = 0; i < outputs.Length; i++)
        {
            var builder = _builders[i];
            if (builder.HasValue)
            {
                outputs[i] = builder.OutputType;
                parsers[i] = builder.Current;
            }
            else
            {
                outputs[i] = typeof(TIn);
                parsers[i] = IdentityAdapter.Create(typeof(TIn));
            }
        }
        
        // Split the input, convert each section, then adapt the result
        var split = SplitParse.Create(typeof(TIn), _builders.Count);
        var parseEach = TupleAdapter.Create(inputs, outputs, parsers);
        var joined = ParseJoin.Create(split, parseEach);
        return (IParser<TIn, TOut>) ParseAdapt.Adapt(joined, typeof(TOut), Context);
    }

    public override TOut Parse(TIn input)
    {
        _built ??= Build();
        return _built.Parse(input);
    }

    /// <summary>
    /// Execute the adapt on a sequence of input elements.
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public IEnumerable<TOut> ParseMany(IEnumerable<TIn> inputs)
    {
        var parser = _built ??= Build();
        foreach (var input in inputs)
        {
            yield return parser.Parse(input);
        }
    }
    
    /// <summary>
    /// Literal text on adapts does nothing.
    /// Effectively literal text is an inline comment.
    /// </summary>
    /// <param name="s"></param>
    public void AppendLiteral(string s) { }

    /// <summary>
    /// Null format begins a new section.
    /// </summary>
    /// <param name="null"></param>
    public void AppendFormatted(Null? @null)
    {
        CreateBuilder();
    }

    /// <summary>
    /// Adds a parse stage with an empty format string.
    /// </summary>
    /// <param name="t"></param>
    /// <typeparam name="T"></typeparam>
    public void AppendFormatted<T>(T t) => AppendFormatted(t, string.Empty);

    /// <summary>
    /// Adds a parse stage to the current section.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="format"></param>
    /// <typeparam name="T"></typeparam>
    public void AppendFormatted<T>(T t, string format)
    {
        if (_builders.Count == 0)
        {
            CreateBuilder();
        }
        
        _builders[^1].AddStage(t, format, Context);
    }

    public override IEnumerable<IParser> GetChildren()
    {
        yield return _built ??= Build();
    }
}