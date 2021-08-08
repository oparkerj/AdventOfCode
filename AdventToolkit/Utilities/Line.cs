using System.Collections.Generic;

namespace AdventToolkit.Utilities
{
    public class Line<T> : AlignedSpace<int, T>
    {
        public override IEnumerable<int> GetNeighbors(int pos)
        {
            yield return pos + 1;
            yield return pos - 1;
        }
    }
}