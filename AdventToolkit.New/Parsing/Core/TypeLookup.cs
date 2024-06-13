using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;

namespace AdventToolkit.New.Parsing.Core;

/// <summary>
/// Wrapper class for an <see cref="ITypeDescriptor"/>.
/// This provides access to static descriptors through a class instance.
/// </summary>
/// <typeparam name="T"></typeparam>
public class TypeLookup<T> : ITypeLookup
    where T : ITypeDescriptor
{
    public bool Match(Type type) => T.Match(type);

    public bool PassiveSelect => T.PassiveSelect;
    
    public bool TrySelect(Type type, out Type inner, out IParser selector)
    {
        return T.TrySelect(type, out inner, out selector);
    }

    public bool TryCollect(Type type, Type inner, IParseContext context, out IParser collector)
    {
        return T.TryCollect(type, inner, context, out collector);
    }

    public bool TryGetCollectType(Type type, IParseContext context, out Type inner)
    {
        return T.TryGetCollectType(type, context, out inner);
    }

    public bool TryConstruct(Type type, IParseContext context, TypeSpan types, out IParser constructor)
    {
        return T.TryConstruct(type, context, types, out constructor);
    }

    public bool TryUnpack(Type type, IParseContext context, int amount, out IParser unpack)
    {
        return T.TryUnpack(type, context, amount, out unpack);
    }
}