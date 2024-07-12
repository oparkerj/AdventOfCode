using System.Numerics;
using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space.Side;

/// <summary>
/// The sides of the 2d pos, with corners.
/// </summary>
/// <typeparam name="TNum"></typeparam>
public class PosSide8<TNum> : IDimStaticSides<PosSide8<TNum>, Pos<TNum>, Side>
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
        yield return Side.UpRight;
        yield return Side.UpLeft;
        yield return Side.DownRight;
        yield return Side.DownLeft;
    }

    public static Pos<TNum> GetSideStatic(Pos<TNum> pos, Side side)
    {
        return side switch
        {
            Side.Up => pos.Up(),
            Side.Right => pos.Right(),
            Side.Left => pos.Left(),
            Side.Down => pos.Down(),
            Side.UpRight => new Pos<TNum>(pos.X + TNum.One, pos.Y + TNum.One),
            Side.UpLeft => new Pos<TNum>(pos.X - TNum.One, pos.Y + TNum.One),
            Side.DownRight => new Pos<TNum>(pos.X + TNum.One, pos.Y - TNum.One),
            Side.DownLeft => new Pos<TNum>(pos.X - TNum.One, pos.Y - TNum.One),
            _ => pos,
        };
    }

    public static bool TryLookupSideStatic(Pos<TNum> pos, Pos<TNum> other, out Side side)
    {
        var (x, y) = other - pos;

        if (TNum.Abs(x) == TNum.Abs(y))
        {
            if (x > TNum.Zero)
            {
                if (y > TNum.Zero)
                {
                    side = Side.UpRight;
                    return true;
                }
                if (y < TNum.Zero)
                {
                    side = Side.DownRight;
                    return true;
                }
            }
            else if (x < TNum.Zero)
            {
                if (y > TNum.Zero)
                {
                    side = Side.UpLeft;
                    return true;
                }
                if (y < TNum.Zero)
                {
                    side = Side.DownLeft;
                    return true;
                }
            }
        }
        
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
        
        if (x == TNum.One)
        {
            if (y == TNum.One)
            {
                side = Side.UpRight;
                return true;
            }
            if (y == -TNum.One)
            {
                side = Side.DownRight;
                return true;
            }
        }
        else if (x == -TNum.One)
        {
            if (y == TNum.One)
            {
                side = Side.UpLeft;
                return true;
            }
            if (y == -TNum.One)
            {
                side = Side.DownLeft;
                return true;
            }
        }
        
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