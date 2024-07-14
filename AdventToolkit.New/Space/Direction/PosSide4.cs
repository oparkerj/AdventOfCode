using System.Numerics;
using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space.Direction;

/// <summary>
/// The sides of the 2d pos, without corners.
/// </summary>
/// <typeparam name="TNum"></typeparam>
public class PosSide4<TNum> : IDimStaticSides<PosSide4<TNum>, Pos<TNum>, Side>
    where TNum : INumber<TNum>
{
    public static IEnumerable<Side> GetSidesStatic(Pos<TNum> pos)
    {
        // TODO benchmark
        // return [Side.Up, Side.Right, Side.Down, Side.Left];
        yield return Side.Up;
        yield return Side.Right;
        yield return Side.Down;
        yield return Side.Left;
    }

    public static Pos<TNum> GetSideStatic(Pos<TNum> pos, Side side)
    {
        return side switch
        {
            Side.Up => pos.Up(),
            Side.Right => pos.Right(),
            Side.Left => pos.Left(),
            Side.Down => pos.Down(),
            _ => pos,
        };
    }

    public static bool TryLookupSideStatic(Pos<TNum> pos, Pos<TNum> other, out Side side)
    {
        var (x, y) = other - pos;
        
        if (x == TNum.Zero)
        {
            if (y > TNum.Zero)
            {
                side = Side.Up;
                return true;
            }
            if (y < TNum.Zero)
            {
                side = Side.Down;
                return true;
            }
        }
        else if (y == TNum.Zero)
        {
            if (x > TNum.Zero)
            {
                side = Side.Right;
                return true;
            }
            if (x < TNum.Zero)
            {
                side = Side.Left;
                return true;
            }
        }
        
        side = default;
        return false;
    }

    public static bool TryLookupSideExactStatic(Pos<TNum> pos, Pos<TNum> other, out Side side)
    {
        var (x, y) = other - pos;
        
        if (x == TNum.Zero)
        {
            if (y == TNum.One)
            {
                side = Side.Up;
                return true;
            }
            if (y == -TNum.One)
            {
                side = Side.Down;
                return true;
            }
        }
        else if (y == TNum.Zero)
        {
            if (x == TNum.One)
            {
                side = Side.Right;
                return true;
            }
            if (x == -TNum.One)
            {
                side = Side.Left;
                return true;
            }
        }
        
        side = default;
        return false;
    }
}