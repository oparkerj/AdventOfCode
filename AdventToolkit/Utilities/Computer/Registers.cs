using System;
using System.Collections.Generic;

namespace AdventToolkit.Utilities.Computer;

// Represents array-backed memory.
public class Registers<T> : IMemory<T>
{
    public T[] Storage;

    public Registers(int size) => Storage = new T[size];

    public Registers(T[] buffer) => Storage = buffer;

    public void Reset() => Array.Fill(Storage, default);

    public void CopyIn(ICollection<T> collection)
    {
        collection.CopyTo(Storage, 0);
    }

    public virtual T this[T t]
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public virtual T this[int i]
    {
        get => Storage[i];
        set => Storage[i] = value;
    }

    public virtual T this[char c]
    {
        get
        {
            if (char.IsLower(c)) return Storage[c - 'a'];
            if (char.IsUpper(c)) return Storage[c - 'A'];
            if (char.IsDigit(c)) return Storage[c - '0'];
            throw new ArgumentException($"Unknown register '{c}'.");
        }
        set
        {
            if (char.IsLower(c)) Storage[c - 'a'] = value;
            else if (char.IsUpper(c)) Storage[c - 'A'] = value;
            else if (char.IsDigit(c)) Storage[c - '0'] = value;
            else throw new ArgumentException($"Unknown register '{c}'.");
        }
    }

    public virtual T this[long l]
    {
        get => Storage[(int) l];
        set => Storage[(int) l] = value;
    }

    public virtual T this[string s]
    {
        get => this[s[0]];
        set => this[s[0]] = value;
    }
}