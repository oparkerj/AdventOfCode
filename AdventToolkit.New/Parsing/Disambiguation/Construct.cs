using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing.Disambiguation;

/// <summary>
/// This disambiguation is used in <see cref="Collect{T}"/> disambiguation.
/// Specifies tha the parse should try construction instead of entering further
/// into an enumerable type.
/// </summary>
public class Construct : IDisambiguation
{
    public static Type? Apply(Type type) => null;
}