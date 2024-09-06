using AdventToolkit2.Parsing.Interface;
using AdventToolkit2.Reflect;

namespace AdventToolkit2.Parsing.Disambiguation;

/// <summary>
/// This disambiguation specified that the parse adapt should not try to
/// enumerate the given type.
/// </summary>
/// <typeparam name="T"></typeparam>
public class NoEnter<T> : IDisambiguation
{
    public static bool Match(Type self, Type type) => type.Generic() == typeof(NoEnter<>);

    public static Type? Apply(Type self) => null;
}