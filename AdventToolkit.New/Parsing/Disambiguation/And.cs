using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;

namespace AdventToolkit.New.Parsing.Disambiguation;

/// <summary>
/// Allows multiple disambiguations to be applied within the same tuple component.
/// This will match and apply the first disambiguation. When the first disambiguation
/// completes, the result is the second disambiguation.
/// </summary>
/// <typeparam name="TFirst">First disambiguation.</typeparam>
/// <typeparam name="TSecond">Second disambiguation.</typeparam>
public class And<TFirst, TSecond> : IDisambiguation
{
    public static bool Match(Type self, Type type) => IDisambiguation.Matches(self.GetSingleTypeArgument(), type);

    public static Type Apply(Type self)
    {
        var generic = self.GetGenericArguments();
        var next = IDisambiguation.ApplyTo(generic[0]);
        return next is null ? generic[1] : typeof(And<,>).MakeGenericType(next, generic[1]);
    }
}