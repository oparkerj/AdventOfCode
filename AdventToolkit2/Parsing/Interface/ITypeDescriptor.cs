using AdventToolkit2.Reflect;

namespace AdventToolkit2.Parsing.Interface;

/// <summary>
/// Describes attributes and behaviors of a type that cannot be detected automatically.
///
/// This can be used to define how to enumerate a type which is not <see cref="IEnumerable{T}"/>
/// </summary>
public interface ITypeDescriptor
{
    /// <summary>
    /// Test if this descriptor applies to a type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>True if this descriptor applies to the type, false otherwise.</returns>
    static abstract bool Match(Type type);

    /// <summary>
    /// When true, a parse builder will not automatically descend into the elements
    /// of this type, if it is enumerable.
    ///
    /// For example, if the current type is a string, you typically want to stop there
    /// and operate on the string rather than descending and parsing each character.
    /// </summary>
    static abstract bool PassiveSelect { get; }

    /// <summary>
    /// Get the element type and selector from a type which can be enumerated.
    /// 
    /// This will not be called for types that implement <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="type">Current type. This type must also be true for <see cref="Match"/></param>
    /// <param name="inner">Element type.</param>
    /// <param name="selector">Parser that turns the type into an enumerable.</param>
    /// <returns>True if the given type is enumerable, false otherwise.</returns>
    static abstract bool TrySelect(Type type, out Type inner, out IParser selector);

    /// <summary>
    /// Get the collector that turns a sequence of elements into an instance of this type.
    ///
    /// This is where you turn an <see cref="IEnumerable{T}"/> into an instance of the type.
    /// </summary>
    /// <param name="type">Current type.</param>
    /// <param name="inner">Element type.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="collector">Container constructor. This takes an enumerable
    ///     of the element type and produces the container.</param>
    /// <returns>True if the type from this type descriptor can can be constructed
    /// from a sequence of the element type, false otherwise.</returns>
    static abstract bool TryCollect(Type type, Type inner, IParseContext context, out IParser collector);

    /// <summary>
    /// Get the element type for a sequence that could be used to construct this type.
    ///
    /// This is typically used on a target type to figure out what element type is needed
    /// to construct the target type.
    /// </summary>
    /// <param name="type">Current type.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="inner">Element type.</param>
    /// <returns>True if the type can be constructed from a sequence, false otherwise.</returns>
    static abstract bool TryGetCollectType(Type type, IParseContext context, out Type inner);

    /// <summary>
    /// Get a parser that constructs this type.
    /// The input to the constructor can either be a single value, or
    /// a tuple.
    /// </summary>
    /// <param name="type">Current type.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="types">Input types that can be used to try construction.</param>
    /// <param name="constructor">Type constructor.</param>
    /// <returns></returns>
    static abstract bool TryConstruct(Type type, IParseContext context, TypeSpan types, out IParser constructor);

    /// <summary>
    /// Get a parser that can unpack the type into a tuple.
    /// The output type of unpacking must be a tuple type.
    /// </summary>
    /// <param name="type">Current type.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="amount">Number of items to unpack.</param>
    /// <param name="unpack">Unpack parser.</param>
    /// <returns></returns>
    static abstract bool TryUnpack(Type type, IParseContext context, int amount, out IParser unpack);

    /// <summary>
    /// Get a parser that can convert the current type to string.
    /// This may be used to override or disable an existing type's ToString
    /// method for parsing.
    /// This is intended to be used in adapters to disable automatic conversions
    /// to string while still leaving the option to convert manually.
    /// </summary>
    /// <param name="type">Current type.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="toString">ToString parser, or null to use the object's ToString method.</param>
    /// <returns>True if the parser may convert this type to string, false otherwise.</returns>
    static abstract bool TryToString(Type type, IParseContext context, out IParser? toString);
}