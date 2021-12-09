using System.Collections.Generic;

namespace AdventToolkit.Collections.Space;

public class Line<T> : SparseSpace<int, T>
{
    public override IEnumerable<int> GetNeighbors(int pos)
    {
        yield return pos + 1;
        yield return pos - 1;
    }
}