using System.Linq;
using AdventToolkit.Collections.Space;

namespace AdventToolkit.Extensions
{
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