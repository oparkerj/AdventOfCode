using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing.Context;

/// <summary>
/// Parse support for arrays.
///
/// This allows an array to be collected.
/// </summary>
public class ArrayParse : ITypeDescriptor
{
    /// <summary>
    /// Check whether every element in a span is the same type.
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
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
    
    public bool Match(Type type) => type.IsArray && type.GetArrayRank() == 1;

    public bool PassiveSelect => false;

    public bool TryCollect(Type type, Type inner, IReadOnlyParseContext context, out IParser collector)
    {
        collector = typeof(Collect<>).NewParserGeneric([inner]);
        return true;
    }

    public bool TryGetCollectType(Type type, IReadOnlyParseContext context, out Type inner)
    {
        inner = type.GetElementType()!;
        return true;
    }

    /// <summary>
    /// Array collector
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Collect<T> : IParser<IEnumerable<T>, T[]>
    {
        public T[] Parse(IEnumerable<T> input) => input.ToArray();
    }
}