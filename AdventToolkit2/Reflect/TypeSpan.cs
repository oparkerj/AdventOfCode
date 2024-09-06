using System.Diagnostics;
using AdventToolkit2.Parsing;
using AdventToolkit2.Parsing.Interface;

namespace AdventToolkit2.Reflect;

/// <summary>
/// This struct wraps a <see cref="ReadOnlySpan{T}"/> of <see cref="Type"/>.
///
/// This can represent a sequence of types of fixed or arbitrary length.
/// 
/// When representing something like an <see cref="IEnumerable{T}"/> of known
/// or unknown length, this class prevents the need to create and fill an
/// array of types. 
/// </summary>
public readonly ref struct TypeSpan
{
    /// <summary>
    /// Types represented in this span. For single-type sequences, this span
    /// has length 1 and <see cref="_length"/> will be nonzero.
    /// </summary>
    private readonly ReadOnlySpan<Type> _span;

    /// <summary>
    /// 0 if only the span is used. Nonzero if the span contains one element
    /// and this value is used for the length.
    /// </summary>
    private readonly int _length;

    private TypeSpan(ReadOnlySpan<Type> span, int length)
    {
        _span = span;
        _length = length;
    }

    /// <summary>
    /// Create a type span from an existing span.
    /// </summary>
    /// <param name="types"></param>
    public TypeSpan(ReadOnlySpan<Type> types) => _span = types;

    /// <summary>
    /// Create a span of arbitrary length from an existing type variable.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="length">Length of the span. A negative value indicates
    /// unknown length, which is the default.</param>
    public TypeSpan(ref readonly Type type, int length = -1)
    {
        _span = new ReadOnlySpan<Type>(in type);
        _length = length;
    }

    /// <summary>
    /// Implicitly convert from a span to a type span.
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    public static implicit operator TypeSpan(ReadOnlySpan<Type> types) => new(types);

    /// <summary>
    /// Implicitly convert from an array to a type span.
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    public static implicit operator TypeSpan(Type[] types) => new(types);

    /// <summary>
    /// The length of this span. This value is negative if the span represents
    /// an unknown length.
    /// </summary>
    public int Length => _length != 0 ? _length : _span.Length;

    /// <summary>
    /// Index the span.
    /// </summary>
    /// <param name="i"></param>
    public Type this[int i]
    {
        get
        {
            Debug.Assert(_length < 0 || i < Length);
            return _length != 0 ? _span[0] : _span[i];
        }
    }

    /// <summary>
    /// Slice the type span.
    /// If the span has unknown length, this has no effect.
    /// </summary>
    /// <param name="start">Slice start.</param>
    /// <param name="length">Slice count.</param>
    /// <returns></returns>
    public TypeSpan Slice(int start, int length)
    {
        // If this span represents an unknown length, slicing does not change anything
        if (_length < 0) return this;
        
        if (_length > 0)
        {
            Debug.Assert(start >= 0 && start <= _length);
            Debug.Assert(length >= 0 && length <= _length - start);
            return new TypeSpan(length == 0 ? default : _span, length);
        }
        return new TypeSpan(_span.Slice(start, length));
    }

    /// <summary>
    /// Check if this span starts with the given sequence.
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    public bool StartsWith(ReadOnlySpan<Type> types)
    {
        if (_length == 0) return _span.CommonPrefixLength(types) == types.Length;
        // If the prefix is longer than this span, it can't be contained within the span
        if (_length > 0 && _length < types.Length) return false;

        var value = _span[0];
        foreach (var type in types)
        {
            if (type != value) return false;
        }
        return true;
    }

    /// <summary>
    /// Create an adapter that will convert the types in this span as a tuple to the requested
    /// tuple type.
    /// If no conversion is needed to convert the tuple, null is returned.
    /// </summary>
    /// <param name="type">Target tuple type.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="adapter">Possible adapter.</param>
    /// <returns></returns>
    public bool TryAdaptTuple(Type type, IParseContext context, out IParser? adapter)
    {
        Debug.Assert(type.IsTupleType());
        var size = type.GetTupleSize();
        
        // If we have a known length and the tuple has more elements, we can't adapt
        if ((_length == 0 && _span.Length < size) || (_length > 0 && _length < size))
        {
            adapter = default;
            return false;
        }
        
        // Take the same number of elements as in the tuple 
        var tuple = _length == 0 ? Types.CreateTupleType(_span[..size]) : Types.CreateTupleType(_span[0], size);
        return ParseAdapt.TryAdapt(tuple, type, context, out adapter);
    }
}