using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing.Context;

public class ArrayParse : ITypeDescriptor
{
    public static bool IsSingleType(ReadOnlySpan<Type> types)
    {
        if (types.Length == 0) return true;
        
        var first = types[0];
        foreach (var type in types[1..])
        {
            if (type != first) return false;
        }
        return true;
    }
    
    public bool Match(Type type) => type.IsArray;

    public bool PassiveSelect => false;

    public bool TryCollect(Type type, Type inner, IReadOnlyParseContext context, out IParser constructor)
    {
        if (type.GetArrayRank() == 1)
        {
            constructor = typeof(Collect<>).NewParserGeneric([inner]);
            return true;
        }

        constructor = default!;
        return false;
    }

    public bool TryGetCollectType(Type type, IReadOnlyParseContext context, out Type inner)
    {
        if (type.GetArrayRank() == 1)
        {
            inner = type.GetElementType()!;
            return true;
        }

        inner = default!;
        return false;
    }

    public class Collect<T> : IParser<IEnumerable<T>, T[]>
    {
        public T[] Parse(IEnumerable<T> input) => input.ToArray();
    }
}