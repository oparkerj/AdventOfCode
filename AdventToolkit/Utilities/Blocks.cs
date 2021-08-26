using System.Collections.Generic;
using AdventToolkit.Extensions;

namespace AdventToolkit.Utilities
{
    public class Blocks<T> : AlignedSpace<Pos3D, T>
    {
        public override IEnumerable<Pos3D> GetNeighbors(Pos3D pos)
        {
            return pos.Around();
        }
    }
}