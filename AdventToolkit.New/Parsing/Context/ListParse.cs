using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;

namespace AdventToolkit.New.Parsing.Context;

/// <summary>
/// Parse support for lists.
///
/// This allows a list to be collected.
/// </summary>
public class ListParse : ITypeDescriptor
{
    public bool Match(Type type) => type.Generic() == typeof(List<>);

    public bool PassiveSelect => false;

    public bool TryCollect(Type type, Type inner, IReadOnlyParseContext context, out IParser collector)
    {
        collector = typeof(ListCollector<>).NewParserGeneric([inner]);
        return true;
    }

    public bool TryGetCollectType(Type type, IReadOnlyParseContext context, out Type inner)
    {
        inner = type.GetSingleTypeArgument();
        return true;
    }

    /// <summary>
    /// List collector.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListCollector<T> : IParser<IEnumerable<T>, List<T>>
    {
        public List<T> Parse(IEnumerable<T> input) => input.ToList();
    }
}