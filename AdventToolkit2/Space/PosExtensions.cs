using System.Numerics;

namespace AdventToolkit2.Space;

public static class PosExtensions
{
    public static Pos<T> Up<T>(this Pos<T> pos)
        where T : INumber<T> =>
        pos with {Y = pos.Y + T.One};
    
    public static Pos<T> Down<T>(this Pos<T> pos)
        where T : INumber<T> =>
        pos with {Y = pos.Y - T.One};
    
    public static Pos<T> Right<T>(this Pos<T> pos)
        where T : INumber<T> =>
        pos with {X = pos.X + T.One};
    
    public static Pos<T> Left<T>(this Pos<T> pos)
        where T : INumber<T> =>
        pos with {X = pos.X - T.One};
    
    public static Pos3<T> Up<T>(this Pos3<T> pos)
        where T : INumber<T> =>
        pos with {Y = pos.Y + T.One};
    
    public static Pos3<T> Down<T>(this Pos3<T> pos)
        where T : INumber<T> =>
        pos with {Y = pos.Y - T.One};
    
    public static Pos3<T> Right<T>(this Pos3<T> pos)
        where T : INumber<T> =>
        pos with {X = pos.X + T.One};
    
    public static Pos3<T> Left<T>(this Pos3<T> pos)
        where T : INumber<T> =>
        pos with {X = pos.X - T.One};
    
    public static Pos3<T> Forward<T>(this Pos3<T> pos)
        where T : INumber<T> =>
        pos with {Z = pos.Z + T.One};
    
    public static Pos3<T> Back<T>(this Pos3<T> pos)
        where T : INumber<T> =>
        pos with {Z = pos.Z - T.One};
}