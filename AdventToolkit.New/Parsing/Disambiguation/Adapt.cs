using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing.Disambiguation;

/// <summary>
/// This disambiguation specifies that the type should be directly adapted.
/// </summary>
public class Adapt : IDisambiguation
{
    public static bool Match(Type self, Type type) => type == typeof(Adapt);

    public static Type? Apply(Type self) => null;
}