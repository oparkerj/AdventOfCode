using AdventToolkit.New.Extensions;

namespace AdventToolkit.New.Data;

public class CircularArray<T>
{
    public readonly T[] Array;
    
    public int Offset { get; set; }
    public int Current { get; set; }

    public CircularArray(T[] array) => Array = array;
    
    public CircularArray(int length) : this(new T[length]) { }

    public int Length => Array.Length;

    public void Add(T t)
    {
        this[Current] = t;
        if (++Current == Array.Length)
        {
            Current = 0;
        }
    }

    public T this[int i]
    {
        get => Array[(Offset + i).Mod(Array.Length)];
        set => Array[(Offset + i).Mod(Array.Length)] = value;
    }
}