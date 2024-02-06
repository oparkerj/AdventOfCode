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

    public bool TryCollect(Type inner, IReadOnlyParseContext context, out IParser constructor)
    {
        constructor = typeof(Collect<>).NewParserGeneric([inner]);
        return true;
    }

    public bool TryGetCollectType(Type type, IReadOnlyParseContext context, out Type inner)
    {
        inner = type.GetElementType()!;
        return true;
    }

    public class Collect<T> : IParser<IEnumerable<T>, T[]>
    {
        public T[] Parse(IEnumerable<T> input) => input.ToArray();
    }
}