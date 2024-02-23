using AdventToolkit.New.Parsing.Builtin;
using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;

namespace AdventToolkit.New.Parsing.Core;

/// <summary>
/// Looks up input stages and joins the resulting parsers together
/// to create a single parsing pipeline.
/// </summary>
public class ParseBuilder
{
    /// <summary>
    /// This will be true when at least one value has been added to the builder.
    /// If this value is false, then most fields are invalid.
    /// </summary>
    public bool HasValue { get; private set; }
    
    /// <summary>
    /// Current parser.
    /// </summary>
    public IParser Current { get; private set; } = default!;

    /// <summary>
    /// Builder initial input type.
    /// </summary>
    public Type InputType { get; init; } = default!;

    /// <summary>
    /// Builder current raw output type.
    /// </summary>
    public Type OutputType { get; private set; } = default!;

    /// <summary>
    /// Builder current working type.
    /// </summary>
    public Type CurrentType { get; private set; } = default!;
    
    /// <summary>
    /// How far are we nested into the current output type.
    /// </summary>
    public int EnumerableLevel { get; private set; }

    /// <summary>
    /// When true, descends into enumerable values during parsing.
    /// </summary>
    public bool AutoSelect { get; set; } = true;

    /// <summary>
    /// Try to descent one level deeper into the current value.
    /// </summary>
    /// <param name="context">Parse context.</param>
    /// <param name="usePassive">If false, then the <see cref="ITypeDescriptor.PassiveSelect"/>
    /// value of the type descriptor is ignored.</param>
    /// <returns>True if we nested into the current value, false otherwise.</returns>
    private bool TryEnterEnumerable(IReadOnlyParseContext context, bool usePassive)
    {
        if (context.TryLookupType(CurrentType, out var descriptor))
        {
            if (!usePassive || !descriptor.PassiveSelect)
            {
                if (descriptor.TryGetInnerType(CurrentType, out var inner, out var enumerator))
                {
                    if (enumerator != null)
                    {
                        Current = ParseJoin.InnerJoin(Current, enumerator, EnumerableLevel, context);
                    }
                    CurrentType = inner;
                    EnumerableLevel++;
                    return true;
                }
            }
        }
        else if (CurrentType.TryGetTypeArguments(typeof(IEnumerable<>), out var innerTypes))
        {
            CurrentType = innerTypes[0];
            EnumerableLevel++;
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// Enter as far as we can into the current value.
    /// </summary>
    /// <param name="context">Parse context.</param>
    private void EnterNested(IReadOnlyParseContext context)
    {
        while (TryEnterEnumerable(context, true)) { }
    }

    /// <summary>
    /// Join the parser to the builder.
    /// This will handle inserting the next parser at the correct location
    /// in the current parser.
    /// </summary>
    /// <param name="parser">Next part.</param>
    /// <param name="context">Parse context.</param>
    private void Append(IParser parser, IReadOnlyParseContext context)
    {
        if (!HasValue)
        {
            HasValue = true;
            Current = parser;
        }
        else
        {
            Current = ParseJoin.InnerJoin(Current, parser, EnumerableLevel, context);
        }

        var (_, output) = ParseUtil.GetParserTypesOf(Current);
        OutputType = output;
        CurrentType = output;

        if (AutoSelect)
        {
            EnumerableLevel = 0;
            EnterNested(context);
        }
    }

    /// <summary>
    /// Try to nest one level deeper into the current value.
    /// This ignores the <see cref="ITypeDescriptor.PassiveSelect"/> setting.
    /// </summary>
    /// <param name="context">Parse context.</param>
    /// <returns>True if successfully nested into the current value, false otherwise.</returns>
    public bool TryEnterEnumerable(IReadOnlyParseContext context) => TryEnterEnumerable(context, false);

    /// <summary>
    /// Try to exit a nested level.
    /// </summary>
    /// <param name="context">Parse context.</param>
    /// <param name="container">Container to use when exiting the nested level.
    /// The current type will be collected into this container. If null, then the
    /// new type will be <see cref="IEnumerable{T}"/> of the current type.</param>
    /// <returns>True if the current level was exited, false otherwise.</returns>
    public bool TryExitEnumerable(IReadOnlyParseContext context, Type? container = null)
    {
        if (EnumerableLevel == 0) return false;
        
        if (container is null)
        {
            EnumerableLevel--;
            CurrentType = typeof(IEnumerable<>).MakeGenericType(CurrentType);
            return true;
        }
        
        if (context.TryCollect(container, CurrentType, out var constructor))
        {
            Append(constructor, context);
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// Look up the parser for a value and add it to the builder.
    ///
    /// First tries to look up a parser which matches the builder value,
    /// and then tries to look up a modifier for the builder value.
    /// </summary>
    /// <param name="value">Builder value.</param>
    /// <param name="extra">Builder extra data.</param>
    /// <param name="context">Parser context.</param>
    /// <typeparam name="T">Builder value type.</typeparam>
    /// <exception cref="ArgumentException">The builder value is invalid.</exception>
    public void AddStage<T>(T value, string extra, IReadOnlyParseContext context)
    {
        var inputType = HasValue ? CurrentType : InputType;

        if (context.TryLookupParser(inputType, value, extra, out var parser))
        {
            Append(parser, context);
        }
        else if (!context.TryApplyModifier(this, inputType, value, extra))
        {
            throw new ArgumentException($"Could not add parser or modifier. (Value = {Types.SimpleValueString(value)}, Extra = \"{extra}\")");
        }
    }

    /// <summary>
    /// Initialize this builder as an identity parser.
    /// </summary>
    public void SetupIdentity()
    {
        Current = IdentityAdapter.Create(InputType);
        CurrentType = InputType;
        OutputType = InputType;
        EnumerableLevel = 0;
    }

    /// <summary>
    /// Adapt the builder to the given type.
    /// </summary>
    /// <param name="context">Parse context.</param>
    /// <param name="outputType">Output type.</param>
    /// <returns>Built parser.</returns>
    public IParser Build(IReadOnlyParseContext context, Type outputType)
    {
        return ParseUtil.Adapt(Current, outputType, context);
    }

    /// <summary>
    /// Build the parser for a specific type and get a strongly typed result.
    /// </summary>
    /// <seealso cref="Build"/>
    /// <param name="context">Parse context.</param>
    /// <typeparam name="T">Output value.</typeparam>
    /// <returns>Build parser.</returns>
    public IParser<string, T> Build<T>(IReadOnlyParseContext context)
    {
        return (IParser<string, T>) Build(context, typeof(T));
    }
}