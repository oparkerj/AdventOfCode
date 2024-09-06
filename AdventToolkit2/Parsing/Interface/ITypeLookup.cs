using AdventToolkit2.Reflect;
using AdventToolkit2.Util;

namespace AdventToolkit2.Parsing.Interface;

/// <summary>
/// This class mirrors the functions of <see cref="ITypeDescriptor"/> but using
/// instance methods.
/// </summary>
public interface ITypeLookup
{
    /// <inheritdoc cref="ITypeDescriptor.Match"/>
    bool Match(Type type);

    /// <inheritdoc cref="ITypeDescriptor.PassiveSelect"/>
    bool PassiveSelect => false;

    /// <inheritdoc cref="ITypeDescriptor.TrySelect"/>
    bool TrySelect(Type type, out Type inner, out IParser selector)
    {
        return Impl.Default(out inner, out selector);
    }

    /// <inheritdoc cref="ITypeDescriptor.TryCollect"/>
    bool TryCollect(Type type, Type inner, IParseContext context, out IParser collector)
    {
        return Impl.Default(out collector);
    }

    /// <inheritdoc cref="ITypeDescriptor.TryGetCollectType"/>
    bool TryGetCollectType(Type type, IParseContext context, out Type inner)
    {
        return Impl.Default(out inner);
    }

    /// <inheritdoc cref="ITypeDescriptor.TryConstruct"/>
    bool TryConstruct(Type type, IParseContext context, TypeSpan types, out IParser constructor)
    {
        return Impl.Default(out constructor);
    }

    /// <inheritdoc cref="ITypeDescriptor.TryUnpack"/>
    bool TryUnpack(Type type, IParseContext context, int amount, out IParser unpack)
    {
        return Impl.Default(out unpack);
    }
}