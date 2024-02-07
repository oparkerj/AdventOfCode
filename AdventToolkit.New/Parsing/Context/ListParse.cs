using AdventToolkit.New.Algorithms;
using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing.Context;

public class ListParse : ITypeDescriptor
{
    public bool Match(Type type) => type.Generic() == typeof(List<>);

    public bool PassiveSelect => false;

    public bool TryCollect(Type type, Type inner, IReadOnlyParseContext context, out IParser constructor)
    {
        constructor = typeof(ListCollector<>).NewParserGeneric([inner]);
        return true;
    }

    public bool TryGetCollectType(Type type, IReadOnlyParseContext context, out Type inner)
    {
        inner = type.GetSingleTypeArgument();
        return true;
    }

    public class ListCollector<T> : IParser<IEnumerable<T>, List<T>>
    {
        public List<T> Parse(IEnumerable<T> input) => input.ToList();
    }
}