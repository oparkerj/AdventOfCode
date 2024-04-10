using System.Numerics;
using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space.Dimension;

public class HexDim<T> : IDimension<Pos3<T>>
    where T : INumber<T>
{
    public static IEnumerable<Pos3<T>> GetNeighbors(Pos3<T> pos)
    {
        yield return pos with {X = pos.X + T.One, Y = pos.Y - T.One};
        yield return pos with {X = pos.X - T.One, Y = pos.Y + T.One};
        yield return pos with {X = pos.X + T.One, Z = pos.Z - T.One};
        yield return pos with {X = pos.X - T.One, Z = pos.Z + T.One};
        yield return pos with {Y = pos.Y + T.One, Z = pos.Z - T.One};
        yield return pos with {Y = pos.Y - T.One, Z = pos.Z + T.One};
    }
}