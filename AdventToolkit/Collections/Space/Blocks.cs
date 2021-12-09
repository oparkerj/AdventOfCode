using System.Collections.Generic;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventToolkit.Collections.Space;

public class Blocks<T> : SparseSpace<Pos3D, T>
{
    public override IEnumerable<Pos3D> GetNeighbors(Pos3D pos)
    {
        return pos.Around();
    }
}