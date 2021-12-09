using System;
using AdventToolkit.Common;

namespace AdventToolkit.Extensions;

public static class HexExtensions
{
    public static int HexDistance(this Pos3D a, Pos3D b)
    {
        var delta = (a - b).Abs();
        return Math.Max(Math.Max(delta.X, delta.Y), delta.Z);
    }
}