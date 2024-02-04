namespace AdventToolkit.New.Parsing.Interface;

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
    bool Match(Type type);

    /// <summary>
    /// Get the element type and selector from a type which can be enumerated.
    /// 
    /// This will not be called for types that implement <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="type">Current type. This type must also be true for <see cref="Match"/></param>
    /// <param name="inner">Element type.</param>
    /// <param name="selector">Parser that turns the type into an enumerable.</param>
    /// <returns>True if the given type is enumerable, false otherwise.</returns>
    bool TrySelect(Type type, out Type inner, out IParser selector)
    {
        inner = default!;
        selector = default!;
        return false;
    }

    /// <summary>
    /// Get the collector that turns a sequence of elements into an instance of this type.
    /// </summary>
    /// <param name="inner">Element type.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="constructor">Container constructor. This takes an enumerable
    /// of the element type and produces the container.</param>
    /// <returns>True if the type from this type descriptor can can be constructed
    /// from a sequence of the element type, false otherwise.</returns>
    bool TryCollect(Type inner, IReadOnlyParseContext context, out IParser constructor)
    {
        constructor = default!;
        return false;
    }

    /// <summary>
    /// Get the element type for a sequence that could be used to construct this type.
    /// </summary>
    /// <param name="type">Current type.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="inner">Element type.</param>
    /// <returns>True if the type can be constructed from a sequence, false otherwise.</returns>
    bool TryGetCollectType(Type type, IReadOnlyParseContext context, out Type inner)
    {
        inner = default!;
        return true;
    }
    
    /// <summary>
    /// When true, a parse builder will not automatically descend into the elements
    /// of this type, if it is enumerable.
    ///
    /// For example, if the current type is a string, you typically want to stop there
    /// and operate on the string rather than descending and parsing each character.
    /// </summary>
    bool PassiveSelect { get; }
}