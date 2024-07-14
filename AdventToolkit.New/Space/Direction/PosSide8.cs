using System.Numerics;
using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space.Direction;

/// <summary>
/// The sides of the 2d pos, with corners.
/// </summary>
/// <typeparam name="TNum"></typeparam>
public class PosSide8<TNum> : IDimStaticSides<PosSide8<TNum>, Pos<TNum>, Side8>
    where TNum : INumber<TNum>
{
    public static IEnumerable<Side8> GetSidesStatic(Pos<TNum> pos)
    {
        // TODO benchmark
        // return [Side.Up, Side.Right, Side.Down, Side.Left];
        yield return Side8.Up;
        yield return Side8.Right;
        yield return Side8.Down;
        yield return Side8.Left;
        yield return Side8.UpRight;
        yield return Side8.UpLeft;
        yield return Side8.DownRight;
        yield return Side8.DownLeft;
    }

    public static Pos<TNum> GetSideStatic(Pos<TNum> pos, Side8 side)
    {
        return side switch
        {
            Side8.Up => pos.Up(),
            Side8.Right => pos.Right(),
            Side8.Left => pos.Left(),
            Side8.Down => pos.Down(),
            Side8.UpRight => new Pos<TNum>(pos.X + TNum.One, pos.Y + TNum.One),
            Side8.UpLeft => new Pos<TNum>(pos.X - TNum.One, pos.Y + TNum.One),
            Side8.DownRight => new Pos<TNum>(pos.X + TNum.One, pos.Y - TNum.One),
            Side8.DownLeft => new Pos<TNum>(pos.X - TNum.One, pos.Y - TNum.One),
            _ => pos,
        };
    }

    public static bool TryLookupSideStatic(Pos<TNum> pos, Pos<TNum> other, out Side8 side)
    {
        var (x, y) = other - pos;

        if (TNum.Abs(x) == TNum.Abs(y))
        {
            if (x > TNum.Zero)
            {
                if (y > TNum.Zero)
                {
                    side = Side8.UpRight;
                    return true;
                }
                if (y < TNum.Zero)
                {
                    side = Side8.DownRight;
                    return true;
                }
            }
            else if (x < TNum.Zero)
            {
                if (y > TNum.Zero)
                {
                    side = Side8.UpLeft;
                    return true;
                }
                if (y < TNum.Zero)
                {
                    side = Side8.DownLeft;
                    return true;
                }
            }
        }
        
        if (x == TNum.Zero)
        {
            if (y > TNum.Zero)
            {
                side = Side8.Up;
                return true;
            }
            if (y < TNum.Zero)
            {
                side = Side8.Down;
                return true;
            }
        }
        else if (y == TNum.Zero)
        {
            if (x > TNum.Zero)
            {
                side = Side8.Right;
                return true;
            }
            if (x < TNum.Zero)
            {
                side = Side8.Left;
                return true;
            }
        }
        
        side = default;
        return false;
    }

    public static bool TryLookupSideExactStatic(Pos<TNum> pos, Pos<TNum> other, out Side8 side)
    {
        var (x, y) = other - pos;
        
        if (x == TNum.One)
        {
            if (y == TNum.One)
            {
                side = Side8.UpRight;
                return true;
            }
            if (y == -TNum.One)
            {
                side = Side8.DownRight;
                return true;
            }
        }
        else if (x == -TNum.One)
        {
            if (y == TNum.One)
            {
                side = Side8.UpLeft;
                return true;
            }
            if (y == -TNum.One)
            {
                side = Side8.DownLeft;
                return true;
            }
        }
        
        if (x == TNum.Zero)
        {
            if (y == TNum.One)
            {
                side = Side8.Up;
                return true;
            }
            if (y == -TNum.One)
            {
                side = Side8.Down;
                return true;
            }
        }
        else if (y == TNum.Zero)
        {
            if (x == TNum.One)
            {
                side = Side8.Right;
                return true;
            }
            if (x == -TNum.One)
            {
                side = Side8.Left;
                return true;
            }
        }
        
        side = default;
        return false;
    }
}