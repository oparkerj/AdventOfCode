using AdventToolkit2.Parsing.Interface;
using AdventToolkit2.Reflect;

namespace AdventToolkit2.Parsing.Disambiguation;

/// <summary>
/// Disambiguation used to specify that a particular conversion should not occur.
/// </summary>
/// <typeparam name="T"></typeparam>
public class No<T> : IDisambiguation
{
    public static bool Match(Type self, Type type) => self.GetSingleTypeArgument() == type;

    public static Type? Apply(Type self) => null;
}