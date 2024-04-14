using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing.Disambiguation;

/// <summary>
/// This will apply disambiguation types so that strings are spread out
/// instead of enumerated further.
/// </summary>
public class StrSpread : IAutoDisambiguation
{
    public static bool Match(Type self, Type type) => true;

    public static Type Apply(Type self) => typeof(And<NoEnter<string>, Collect<Tuples>>);
}