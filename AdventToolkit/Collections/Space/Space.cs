using System;
using System.Collections.Generic;

namespace AdventToolkit.Collections.Space;

public class Space<T> : ISpace<T>
{
    public readonly Func<T, IEnumerable<T>> Neighbors;

    public Space(Func<T, IEnumerable<T>> neighbors) => Neighbors = neighbors;

    public IEnumerable<T> GetNeighbors(T t) => Neighbors(t);
}