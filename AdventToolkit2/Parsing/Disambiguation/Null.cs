using AdventToolkit2.Parsing.Interface;

namespace AdventToolkit2.Parsing.Disambiguation;

/// <summary>
/// A disambiguation type that does nothing.
/// This may be used as a spacer if some tuple types don't need disambiguation.
/// </summary>
public class Null : IDisambiguation
{
    public static bool Match(Type self, Type type) => false;

    public static Type? Apply(Type self) => null;
}