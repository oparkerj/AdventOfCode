using System.Diagnostics;

namespace AdventToolkit.New.Reflect;

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
            Debug.Assert(_length <= 0 || i < _length);
            return _length != 0 ? _span[0] : _span[i];
        }
    }

    /// <summary>
    /// Check if this span starts with the given sequence.
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    public bool StartsWith(ReadOnlySpan<Type> types)
    {
        if (_length == 0) return _span.CommonPrefixLength(types) == types.Length;
        if (_length > 0 && _length < types.Length) return false;
            
        foreach (var type in types)
        {
            if (type != _span[0]) return false;
        }
        return true;
    }
}