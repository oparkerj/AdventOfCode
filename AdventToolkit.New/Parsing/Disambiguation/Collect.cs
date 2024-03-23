using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;

namespace AdventToolkit.New.Parsing.Disambiguation;

/// <summary>
/// This disambiguation specifies which method to use when collecting an enumerable.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Collect<T> : IDisambiguation
{
    public static Type Apply(Type type) => type.GetSingleTypeArgument();
}