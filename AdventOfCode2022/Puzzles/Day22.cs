using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2022.Puzzles;

public class Day22 : Puzzle<int>
{
    public const char Nothing = ' ';
    public const char Open = '.';
    public const char Wall = '#';
    
    public override int PartOne()
    {
        var map = AllGroups[0].ToGrid();
        var current = map.Positions.Order(Pos.ReadingOrder)
            .First(pos => map[pos] == Open);
        var dir = Pos.Right;
        
        foreach (var s in AllGroups[1][0].TakeRegex(@"\D+|\d+"))
        {
            if (char.IsNumber(s[0]))
            {
                foreach (var _ in Enumerable.Range(0, s.AsInt()))
                {
                    var target = current + dir;
                    if (!map.Has(target) || map[target] == Nothing)
                    {
                        target = current.Trace(-dir, pos => !map.Has(pos) || map[pos] == Nothing) + dir;
                        if (map[target] == Open) current = target;
                    }
                    else if (map[target] != Wall) current = target;
                }
            }
            else {
                dir = s[0] == 'R' ? dir.Clockwise() : dir.CounterClockwise();
            }
        }

        var (x, y) = (current.X - map.Bounds.MinX + 1, map.Bounds.MaxY - current.Y + 1);
        var facing = (dir.GetDirection() - 1).CircularMod(4);

        return y * 1000 + x * 4 + facing;
    }
    
    Pos ComingFrom(Side side)
    {
        return side switch
        {
            Side.Bottom => Pos.Up,
            Side.Left => Pos.Right,
            Side.Right => Pos.Left,
            Side.Top => Pos.Down,
        };
    }

    public override int PartTwo()
    {
        var map = new PortalGrid<char, Pos>();
        var original = AllGroups[0].ToGrid();
        var transformer = SimpleGridTransformer<char>.ToFirstQuadrant();
        foreach (var (pos, value) in original)
        {
            map[transformer.Transform(pos, original)] = value;
        }

        void Connect(IEnumerable<(Pos, Pos)> positions, Side first, Side second)
        {
            foreach (var (a, b) in positions)
            {
                // The tags are swapped when assigned
                map.Connect(a, first, ComingFrom(second), b, second, ComingFrom(first));
            }
        }

        var a = new Rect(50, 150, 50, 50);
        var b = new Rect(100, 150, 50, 50);
        var c = new Rect(50, 100, 50, 50);
        var d = new Rect(0, 50, 50, 50);
        var e = new Rect(50, 50, 50, 50);
        var f = new Rect(0, 0, 50, 50);
        
        Connect(a.GetSidePositions(Side.Top).Zip(f.GetSidePositions(Side.Left).Reverse()), Side.Top, Side.Left);
        Connect(b.GetSidePositions(Side.Top).Zip(f.GetSidePositions(Side.Bottom)), Side.Top, Side.Bottom);
        Connect(a.GetSidePositions(Side.Left).Zip(d.GetSidePositions(Side.Left).Reverse()), Side.Left, Side.Left);
        Connect(b.GetSidePositions(Side.Right).Zip(e.GetSidePositions(Side.Right).Reverse()), Side.Right, Side.Right);
        Connect(b.GetSidePositions(Side.Bottom).Zip(c.GetSidePositions(Side.Right).Reverse()), Side.Bottom, Side.Right);
        Connect(c.GetSidePositions(Side.Left).Zip(d.GetSidePositions(Side.Top).Reverse()), Side.Left, Side.Top);
        Connect(e.GetSidePositions(Side.Bottom).Zip(f.GetSidePositions(Side.Right).Reverse()), Side.Bottom, Side.Right);

        var current = map.Positions.Select(tuple => tuple.Pos)
            .Order(Pos.ReadingOrder)
            .First(pos => map.GetAny(pos) == Open);
        var dir = Pos.Right;
        
        foreach (var s in AllGroups[1][0].TakeRegex(@"\D+|\d+"))
        {
            if (char.IsNumber(s[0]))
            {
                foreach (var _ in Enumerable.Range(0, s.AsInt()))
                {
                    var (target, side) = map.GetNeighborSide(current, dir.ToSide());
                    if (map.GetAny(target) != Wall)
                    {
                        if (side != Pos.Zero) dir = side;
                        current = target;
                    }
                }
            }
            else {
                dir = s[0] == 'R' ? dir.Clockwise() : dir.CounterClockwise();
            }
        }

        var (x, y) = (current.X + 1, 200 - current.Y);
        var facing = (dir.GetDirection() - 1).CircularMod(4);
        
        return y * 1000 + x * 4 + facing;
    }
}