namespace AdventToolkit.New.Parsing.Interface;

// TODO see if this would work better as a static abstract method

/// <summary>
/// An object that can lookup a parser to add to the parse pipeline.
/// </summary>
public interface IParserLookup
{
    /// <summary>
    /// Try to find a matching parser.
    /// </summary>
    /// <param name="inputType">Parse input type.</param>
    /// <param name="value">Parse build value.</param>
    /// <param name="extra">Parse build extra data.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="parser">Matching parser.</param>
    /// <typeparam name="T">Build value type.</typeparam>
    /// <returns>True if a parser was found, false otherwise.</returns>
    bool TryLookup<T>(Type inputType, T value, string extra, IParseContext context, out IParser parser);
}

/// <summary>
/// Parser lookup for a specific builder value type.
/// </summary>
/// <typeparam name="TValue">Builder value type.</typeparam>
public interface IParserLookup<in TValue> : IParserLookup
{
    bool IParserLookup.TryLookup<T>(Type inputType, T value, string extra, IParseContext context, out IParser parser)
    {
        if (value is TValue typedValue)
        {
            return TryLookup(inputType, typedValue, extra, context, out parser);
        }

        parser = default!;
        return false;
    }

    /// <inheritdoc cref="IParserLookup.TryLookup{T}"/>
    bool TryLookup(Type inputType, TValue value, string extra, IParseContext context, out IParser parser);
}

/// <summary>
/// Parser lookup for a specific builder value type.
/// </summary>
/// <typeparam name="TInput">Builder value type.</typeparam>
public interface IParserLookupByInput<in TInput> : IParserLookup
{
    bool IParserLookup.TryLookup<T>(Type inputType, T value, string extra, IParseContext context, out IParser parser)
    {
        if (inputType == typeof(TInput))
        {
            return TryLookup(value, extra, context, out parser);
        }

        parser = default!;
        return false;
    }

    /// <inheritdoc cref="IParserLookup.TryLookup{T}"/>
    bool TryLookup<T>(T value, string extra, IParseContext context, out IParser parser);
}