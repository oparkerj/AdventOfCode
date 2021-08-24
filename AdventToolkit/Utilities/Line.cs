using System.Collections.Generic;
using System.Linq;

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

    public static class LineExtensions
    {
        public static void Increment(this Line<int> line, int start, int length)
        {
            foreach (var i in Enumerable.Range(start, length))
            {
                line[i]++;
            }
        }
    }
}