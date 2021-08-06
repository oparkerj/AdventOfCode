using System.Collections.Generic;

namespace AdventToolkit.Utilities
{
    public class HexGrid<T> : AlignedSpace<Pos3D, T>
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