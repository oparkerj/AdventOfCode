using System.Diagnostics;
using System.Numerics;
using AdventToolkit.New.Interface;

namespace AdventToolkit.New.Data.Space;

/// <summary>
/// A space where the positions are a range of sequential values.
/// The position may be any number type, however the underlying data
/// is an array, so the position will be converted to int.
/// </summary>
/// <typeparam name="TNum"></typeparam>
/// <typeparam name="T"></typeparam>
public class PackedLine<TNum, T> : ISpace<TNum, T>
    where TNum : INumber<TNum>
{
    /// <summary>
    /// Underlying position data.
    /// </summary>
    public T[] Data { get; private set; }
    
    /// <summary>
    /// Range of values represented by this line.
    /// </summary>
    public Interval<int> Interval { get; private set; }
    
    /// <summary>
    /// Get <see cref="Interval"/> but with the line number type.
    /// </summary>
    public Interval<TNum> TypeInterval { get; private set; }

    private PackedLine(Interval<int> interval, Interval<TNum> typeInterval)
    {
        Debug.Assert(interval.Length == int.CreateTruncating(typeInterval.Length));
        Data = new T[interval.Length];
        Interval = interval;
        TypeInterval = typeInterval;
    }

    /// <summary>
    /// Create a line over the given range of values.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="length"></param>
    public PackedLine(TNum start, TNum length) : this(
        new Interval<int>(int.CreateTruncating(start), int.CreateTruncating(length)),
        new Interval<TNum>(start, length))
    { }
    
    /// <summary>
    /// Create a line over the given range of values.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="length"></param>
    public PackedLine(int start, int length) : this(
        new Interval<int>(start, length),
        new Interval<TNum>(TNum.CreateTruncating(start), TNum.CreateTruncating(length)))
    { }

    /// <summary>
    /// Create a line over the given range of values.
    /// </summary>
    /// <param name="interval"></param>
    public PackedLine(Interval<int> interval) : this(interval, interval.As<TNum>())
    { }

    /// <summary>
    /// Create a line over the given range of values.
    /// </summary>
    /// <param name="interval"></param>
    public PackedLine(Interval<TNum> interval) : this(interval.As<int>(), interval)
    { }

    /// <summary>
    /// Starting index of the line.
    /// </summary>
    public int Start
    {
        get => Interval.Start;
        set
        {
            Interval = Interval with {Start = value};
            TypeInterval = TypeInterval with {Start = TNum.CreateTruncating(value)};
        }
    }

    /// <summary>
    /// <see cref="Start"/> as <see cref="TNum"/>
    /// </summary>
    public TNum TypeStart
    {
        get => TypeInterval.Start;
        set
        {
            Interval = Interval with {Start = int.CreateTruncating(value)};
            TypeInterval = TypeInterval with {Start = value};
        }
    }

    /// <summary>
    /// Length of the line.
    /// </summary>
    public int Length
    {
        get => Interval.Length;
        set
        {
            Interval = Interval with {Length = value};
            TypeInterval = TypeInterval with {Length = TNum.CreateTruncating(value)};
            var data = Data;
            Array.Resize(ref data, value);
            Data = data;
        }
    }
    
    /// <summary>
    /// <see cref="Length"/> as <see cref="TNum"/>
    /// </summary>
    public TNum TypeLength
    {
        get => TypeInterval.Length;
        set
        {
            var length = int.CreateTruncating(value);
            Interval = Interval with {Length = length};
            TypeInterval = TypeInterval with {Length = value};
            var data = Data;
            Array.Resize(ref data, length);
            Data = data;
        }
    }

    public int Count => Interval.Length;

    public T Default { get; set; } = default!;

    public IEnumerable<TNum> GetNeighbors(TNum pos)
    {
        if (pos < TypeInterval.Last)
        {
            yield return pos + TNum.One;
        }
        if (pos > TypeStart)
        {
            yield return pos - TNum.One;
        }
    }
    
    public void Add(TNum pos, T val) => this[pos] = val;

    public void Add(int pos, T val) => this[pos] = val;

    public bool Remove(TNum pos)
    {
        if (!TypeInterval.Contains(pos)) return false;
        this[pos] = default!;
        return true;
    }

    public bool Remove(int pos)
    {
        if (!Interval.Contains(pos)) return false;
        this[pos] = default!;
        return true;
    }

    public bool TryGet(TNum pos, out T val)
    {
        if (TypeInterval.Contains(pos))
        {
            val = this[pos];
            return true;
        }
        val = default!;
        return false;
    }

    public bool TryGet(int pos, out T val)
    {
        if (Interval.Contains(pos))
        {
            val = this[pos];
            return true;
        }
        val = default!;
        return false;
    }

    public T Get(TNum pos) => this[pos];

    public T Get(int pos) => this[pos];

    public T GetStrict(TNum pos) => Data[int.CreateTruncating(pos - TypeStart)];

    public T GetString(int pos) => Data[pos - Start];

    public T this[TNum pos]
    {
        get => TypeInterval.Contains(pos) ? Data[int.CreateTruncating(pos - TypeStart)] : Default;
        set
        {
            if (TypeInterval.Contains(pos))
            {
                Data[int.CreateTruncating(pos - TypeStart)] = value;
            }
        }
    }

    /// <summary>
    /// Overload of the indexer for int indexing.
    /// </summary>
    /// <param name="pos"></param>
    public T this[int pos]
    {
        get => Interval.Contains(pos) ? Data[pos - Start] : Default;
        set
        {
            if (Interval.Contains(pos))
            {
                Data[pos - Start] = value;
            }
        }
    }

    /// <summary>
    /// Overload of the indexer for long indexing.
    /// </summary>
    /// <param name="pos"></param>
    public T this[long pos]
    {
        get => this[(int) pos];
        set => this[(int) pos] = value;
    }

    public bool Contains(TNum pos) => TypeInterval.Contains(pos);

    public bool Contains(int pos) => Interval.Contains(pos);

    public bool ContainsValue(T val) => Array.IndexOf(Data, val) > -1;

    public void Clear() => Array.Clear(Data);

    public IEnumerable<TNum> Positions => TypeInterval;

    public IEnumerable<T> Values => Data;
}