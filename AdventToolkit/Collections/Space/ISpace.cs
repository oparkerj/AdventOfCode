using System.Collections.Generic;

namespace AdventToolkit.Collections.Space;

public interface ISpace<T>
{
    IEnumerable<T> GetNeighbors(T t);
}