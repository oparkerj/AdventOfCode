namespace AdventToolkit.New.Parsing.Interface;

/// <summary>
/// Specifies a disambiguation type.
/// </summary>
public interface IDisambiguation
{
    /// <summary>
    /// Get the next disambiguation state.
    ///
    /// For example: A type with generics may return one of the generic types as the next state.
    /// </summary>
    /// <param name="type">This will always be a constructed version of the containing class.</param>
    /// <returns>Next state, or null if there is no further state.</returns>
    static abstract Type? Apply(Type type);
}