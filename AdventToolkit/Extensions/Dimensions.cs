using System.Collections.Generic;

namespace AdventToolkit.Extensions;

public static class Dimensions
{
    public static IEnumerable<(int, int, int)> Around(this (int X, int Y, int Z) p)
    {
        for (var x = -1; x <= 1; x++)
        {
            for (var y = -1; y <= 1; y++)
            {
                for (var z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0) continue;
                    yield return (p.X + x, p.Y + y, p.Z + z);
                }
            }   
        }
    }

    public static IEnumerable<(int, int, int, int)> Around(this (int X, int Y, int Z, int W) p)
    {
        for (var x = -1; x <= 1; x++)
        {
            for (var y = -1; y <= 1; y++)
            {
                for (var z = -1; z <= 1; z++)
                {
                    for (var w = -1; w <= 1; w++)
                    {
                        if (x == 0 && y == 0 && z == 0 && w == 0) continue;
                        yield return (p.X + x, p.Y + y, p.Z + z, p.W + w);
                    }
                }
            }   
        }
    }
}