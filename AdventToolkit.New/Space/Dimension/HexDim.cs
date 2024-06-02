using System.Numerics;
using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space.Dimension;

/// <summary>
/// Defines a hexagonal dimension.
/// </summary>
/// <typeparam name="T"></typeparam>
public class HexDim<T> : IStaticDimension<HexDim<T>, Pos3<T>>
    where T : INumber<T>
{
    public static IEnumerable<Pos3<T>> GetNeighborsStatic(Pos3<T> pos)
    {
        yield return pos with {X = pos.X + T.One, Y = pos.Y - T.One};
        yield return pos with {X = pos.X - T.One, Y = pos.Y + T.One};
        yield return pos with {X = pos.X + T.One, Z = pos.Z - T.One};
        yield return pos with {X = pos.X - T.One, Z = pos.Z + T.One};
        yield return pos with {Y = pos.Y + T.One, Z = pos.Z - T.One};
        yield return pos with {Y = pos.Y - T.One, Z = pos.Z + T.One};
    }
}