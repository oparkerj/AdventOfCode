using AdventToolkit2.Parsing.Interface;

namespace AdventToolkit2.Parsing.Disambiguation;

/// <summary>
/// This disambiguation is used in <see cref="Collect{T}"/> disambiguation.
/// Specifies that the parse should try construction instead of entering further
/// into an enumerable type.
/// </summary>
public class Construct : IDisambiguation
{
    public static bool Match(Type self, Type type) => type == typeof(Construct);

    public static Type? Apply(Type self) => null;
}