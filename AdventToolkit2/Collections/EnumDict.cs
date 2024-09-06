using System.Collections;
using System.Diagnostics;
using System.Linq.Expressions;
using AdventToolkit2.Debugging;
using AdventToolkit2.Extensions;

namespace AdventToolkit2.Collections;

/// <summary>
/// Map an enum to values, backed by an array.
/// This assumes that the enum is a continuous range of values starting at 0.
/// </summary>
/// <typeparam name="TEnum"></typeparam>
/// <typeparam name="TVal"></typeparam>
public class EnumDict<TEnum, TVal> : IDictionary<TEnum, TVal>
    where TEnum : struct, Enum
{
    private readonly TEnum[] _indices;
    
    public readonly TVal[] Data;

    public readonly Func<TEnum, int> ToIndex;

    public EnumDict()
    {
        _indices = Enum.GetValues<TEnum>();
        Data = new TVal[_indices.Length];

        // Create a function that will cast the enum to int
        var input = Expression.Parameter(typeof(TEnum));
        var convert = Expression.Convert(input, typeof(int));
        ToIndex = Expression.Lambda<Func<TEnum, int>>(convert, input).Compile();
        
        // Make sure the enum values are incrementing from 0
        Debug.Assert(_indices.All((e, i) => ToIndex(e) == i));
    }

    public int Count => Data.Length;

    public bool IsReadOnly => false;

    public ICollection<TEnum> Keys => _indices;

    public ICollection<TVal> Values => Data;

    public TVal this[TEnum key]
    {
        get => Data[ToIndex(key)];
        set => Data[ToIndex(key)] = value;
    }

    public void Clear() => Array.Clear(Data);

    public void Add(KeyValuePair<TEnum, TVal> item) => Add(item.Key, item.Value);

    public void Add(TEnum key, TVal value) => this[key] = value;

    public bool Remove(KeyValuePair<TEnum, TVal> item) => Remove(item.Key);

    public bool Remove(TEnum key)
    {
        this[key] = default!;
        return true;
    }

    public bool Contains(KeyValuePair<TEnum, TVal> item) => ContainsKey(item.Key);

    public bool ContainsKey(TEnum key) => Enum.IsDefined(key);

    public bool TryGetValue(TEnum key, out TVal value)
    {
        if (!ContainsKey(key))
        {
            value = default!;
            return false;
        }

        value = this[key];
        return true;
    }

    public void CopyTo(KeyValuePair<TEnum, TVal>[] array, int arrayIndex)
    {
        if (array.Length - arrayIndex > Count) Err.Argument("Not enough space.");

        var count = Count;
        for (var i = 0; i < count; i++)
        {
            array[i + arrayIndex] = new KeyValuePair<TEnum, TVal>(_indices[i], Data[i]);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<KeyValuePair<TEnum, TVal>> GetEnumerator()
    {
        var count = Count;
        for (var i = 0; i < count; i++)
        {
            yield return new KeyValuePair<TEnum, TVal>(_indices[i], Data[i]);
        }
    }
}