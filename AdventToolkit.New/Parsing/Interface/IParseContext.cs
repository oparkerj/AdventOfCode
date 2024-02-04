namespace AdventToolkit.New.Parsing.Interface;

/// <summary>
/// Provides the ability to look up the types needed for a parse.
/// </summary>
public interface IReadOnlyParseContext
{
    /// <summary>
    /// All known type descriptors.
    /// </summary>
    IEnumerable<ITypeDescriptor> Types { get; }
    
    /// <summary>
    /// All known parser lookups.
    /// </summary>
    IEnumerable<IParserLookup> ParserLookups { get; }
    
    /// <summary>
    /// All known modifiers.
    /// </summary>
    IEnumerable<IModifier> Modifiers { get; }
    
    /// <summary>
    /// All known adapter lookups.
    /// </summary>
    IEnumerable<IAdapterLookup> AdapterLookups { get; }
    
    /// <summary>
    /// Try to find a matching type descriptor.
    /// </summary>
    /// <param name="type">Search type.</param>
    /// <param name="descriptor">Matching type descriptor.</param>
    /// <returns>True if a type descriptor was found, false otherwise.</returns>
    bool TryLookupType(Type type, out ITypeDescriptor descriptor);
    
    /// <summary>
    /// Try to find a matching parser lookup.
    /// </summary>
    /// <param name="inputType">Parse input type.</param>
    /// <param name="value">Parse builder value.</param>
    /// <param name="extra">Parse builder extra data.</param>
    /// <param name="parser">Matching parser.</param>
    /// <typeparam name="T">Builder value type.</typeparam>
    /// <returns>True if a parser was found, false otherwise.</returns>
    bool TryLookupParser<T>(Type inputType, T value, string extra, out IParser parser);

    /// <summary>
    /// Try to find a matching adapter.
    /// </summary>
    /// <param name="from">Input type.</param>
    /// <param name="to">Output type.</param>
    /// <param name="parser">Matching adapter.</param>
    /// <returns>True if an adapter was found, false otherwise.</returns>
    bool TryLookupAdapter(Type from, Type to, out IParser parser);

    /// <summary>
    /// Try to apply a matching modifier.
    /// </summary>
    /// <param name="pipeline">Current parse builder.</param>
    /// <param name="inputType">Parse input type.</param>
    /// <param name="value">Parse builder value.</param>
    /// <param name="extra">Parse builder extra value.</param>
    /// <typeparam name="TPipeline">Current parse builder type.</typeparam>
    /// <typeparam name="T">Builder value type.</typeparam>
    /// <returns>True if a modifier was applied, false otherwise.</returns>
    bool TryApplyModifier<TPipeline, T>(TPipeline pipeline, Type inputType, T value, string extra);

    /// <summary>
    /// Try to contain a value type using a container type.
    /// </summary>
    /// <param name="container">Container type.</param>
    /// <param name="inner">Element type.</param>
    /// <param name="constructor">Constructor which takes an enumerable of the
    /// element type and produces the container type.</param>
    /// <returns>True if a container constructor is found, false otherwise.</returns>
    bool TryCollect(Type container, Type inner, out IParser constructor);
}

/// <summary>
/// A parse context where context objects can be added.
/// </summary>
public interface IParseContext : IReadOnlyParseContext
{
    /// <summary>
    /// Add a type descriptor to the context.
    /// </summary>
    /// <param name="descriptor"></param>
    void AddType(ITypeDescriptor descriptor);

    /// <summary>
    /// Add a parser lookup to the context.
    /// </summary>
    /// <param name="parserLookup"></param>
    void AddParserLookup(IParserLookup parserLookup);

    /// <summary>
    /// Add a modifier to the context.
    /// </summary>
    /// <param name="modifier"></param>
    void AddModifier(IModifier modifier);

    /// <summary>
    /// Add an adapter to the context.
    /// </summary>
    /// <param name="adapterLookup"></param>
    void AddAdapter(IAdapterLookup adapterLookup);
}