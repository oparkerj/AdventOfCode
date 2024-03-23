namespace AdventToolkit.New.Parsing.Interface;

/// <summary>
/// A type that can try to lookup an adapter between two types.
/// </summary>
public interface IAdapterLookup
{
    /// <summary>
    /// Try to find an adapter to convert between types.
    /// </summary>
    /// <param name="from">Input type.</param>
    /// <param name="to">Output type.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="parser">Adapter.</param>
    /// <returns>True if a relevant adapter was found, false otherwise.</returns>
    bool TryLookup(Type from, Type to, IParseContext context, out IParser parser);
}

/// <summary>
/// Adapter lookup for a specific 'from' type.
/// </summary>
/// <typeparam name="TFrom">From type.</typeparam>
public interface IAdapterLookup<TFrom> : IAdapterLookup
{
    bool IAdapterLookup.TryLookup(Type from, Type to, IParseContext context, out IParser parser)
    {
        if (from == typeof(TFrom)) return TryLookup(to, context, out parser);
        
        parser = default!;
        return false;
    }

    /// <summary>
    /// Try to find an adapter to convert to the target type.
    /// </summary>
    /// <param name="to">Output type.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="parser">Adapter.</param>
    /// <returns>True if a relevant adapter was found, false otherwise.</returns>
    bool TryLookup(Type to, IParseContext context, out IParser parser);
}

/// <summary>
/// Adapter lookup where the 'from' and 'to' types are known.
/// </summary>
/// <typeparam name="TFrom">From type.</typeparam>
/// <typeparam name="TTo">To Type.</typeparam>
public interface IAdapterLookup<TFrom, TTo> : IAdapterLookup<TFrom>
{
    bool IAdapterLookup<TFrom>.TryLookup(Type to, IParseContext context, out IParser parser)
    {
        if (to == typeof(TTo)) return TryLookup(context, out parser);

        parser = default!;
        return false;
    }

    /// <summary>
    /// Try to find an adapter to perform the conversion.
    /// </summary>
    /// <param name="context">Parse context.</param>
    /// <param name="parser">Adapter.</param>
    /// <returns>True if a relevant adapter was found, false otherwise.</returns>
    bool TryLookup(IParseContext context, out IParser parser);
}