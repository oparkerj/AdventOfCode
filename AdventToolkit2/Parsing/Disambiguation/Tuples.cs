using AdventToolkit2.Parsing.Interface;
using AdventToolkit2.Reflect;

namespace AdventToolkit2.Parsing.Disambiguation;

/// <summary>
/// This disambiguation is used in <see cref="Collect{T}"/> disambiguation.
/// Specifies that the parse should try to collect a container of tuples instead of
/// entering further into an enumerable type.
/// </summary>
public class Tuples : IDisambiguation
{
    public static bool Match(Type self, Type type) => type == typeof(Tuples);

    public static Type? Apply(Type self) => null;
}

/// <summary>
/// This disambiguation specifies which method to use when adapting
/// an enumerable to a tuple.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Tuples<T> : IDisambiguation
{
    public static bool Match(Type self, Type type) => type.Generic() == typeof(Tuples<>);

    public static Type? Apply(Type self) => self.GetSingleTypeArgument();
}