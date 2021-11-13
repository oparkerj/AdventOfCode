using System.Collections.Generic;
using AdventToolkit.Common;

namespace AdventToolkit.Collections.Space
{
    public static class HexGrid
    {
        public static IEnumerable<string> Directions()
        {
            yield return "n";
            yield return "ne";
            yield return "se";
            yield return "s";
            yield return "sw";
            yield return "nw";
        }
    }
    
    public class HexGrid<T> : SparseSpace<Pos3D, T>
    {
        public static IEnumerable<Pos3D> Surround(Pos3D pos)
        {
            var (x, y, z) = pos;
            yield return new Pos3D(x + 1, y, z - 1);
            yield return new Pos3D(x - 1, y, z + 1);
            yield return new Pos3D(x + 1, y - 1, z);
            yield return new Pos3D(x - 1, y + 1, z);
            yield return new Pos3D(x, y - 1, z + 1);
            yield return new Pos3D(x, y + 1, z - 1);
        }

        public override IEnumerable<Pos3D> GetNeighbors(Pos3D pos) => Surround(pos);
    }
}