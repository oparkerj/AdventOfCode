using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing.Disambiguation;

/// <summary>
/// A disambiguation type that does nothing.
/// This may be used as a spacer if some tuple types don't need disambiguation.
/// </summary>
public class Null : IDisambiguation
{
    public static Type? Apply(Type type) => null;
}