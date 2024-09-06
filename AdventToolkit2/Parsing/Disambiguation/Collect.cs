using AdventToolkit2.Parsing.Interface;
using AdventToolkit2.Reflect;

namespace AdventToolkit2.Parsing.Disambiguation;

/// <summary>
/// Generic disambiguation type that can be used in other containers.
/// </summary>
public class Collect : IDisambiguation
{
    public static bool Match(Type self, Type type) => type == typeof(Collect);

    public static Type? Apply(Type self) => null;
}

/// <summary>
/// This disambiguation specifies which method to use when collecting an enumerable.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Collect<T> : IDisambiguation
{
    public static bool Match(Type self, Type type) => type.Generic() == typeof(Collect<>);

    public static Type? Apply(Type self) => self.GetSingleTypeArgument();
}