using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2018.Puzzles;

public class Day17 : Puzzle
{
    public readonly Pos Spring = new(500, 0);

    public const int Empty = 0;
    public const int Wall = 1;
    public const int Water = 2;
    public const int Settled = 3;
    public readonly Grid<int> Map = new();

    public void BuildMap()
    {
        var lines = Input.Extract<(char, int, int, int)>(@"(.)=(\d+), .=(\d+)..(\d+)");
        foreach (var (fix, at, start, end) in lines)
        {
            foreach (var i in Interval.RangeInclusive(start, end))
            {
                var p = fix == 'x' ? new Pos(at, i) : new Pos(i, at);
                Map[p] = Wall;
            }
        }
    }

    public override void PartOne()
    {
        BuildMap();
            
        var range = Interval.RangeInclusive(Map.Bounds.MinY, Map.Bounds.MaxY);
        var backtrack = new Dictionary<Pos, Pos>();
        var fall = new Stack<Pos>();
        fall.Push(Spring);
        Map[Spring] = Water;
        while (fall.Count > 0)
        {
            var p = fall.Pop();

            while (true)
            {
                var up = p + Pos.Up;
                if (up.Y > range.End) goto Done;
                if (Map[up] is Wall or Settled) break;
                Map[up] = Water;
                p = up;
            }

            while (true)
            {
                var left = p.Trace(Pos.Left, pos => Map[pos] == Wall || Map[pos + Pos.Up] is Water or Empty);
                var right = p.Trace(Pos.Right, pos => Map[pos] == Wall || Map[pos + Pos.Up] is Water or Empty);
                if (Map[left] == Wall && Map[right] == Wall)
                {
                    foreach (var pos in left.EachBetween(right))
                    {
                        Map[pos] = Settled;
                    }
                    p = backtrack.GetValueOrDefault(p, p + Pos.Down);
                }
                else
                {
                    foreach (var pos in left.EachBetween(right))
                    {
                        Map[pos] = Water;
                    }
                    if (!Map.Has(left))
                    {
                        Map[left] = Water;
                        fall.Push(left);
                        backtrack[left] = p + Pos.Down;
                    }
                    if (!Map.Has(right))
                    {
                        Map[right] = Water;
                        fall.Push(right);
                        backtrack[right] = p + Pos.Down;
                    }
                    break;
                }
            }
            Done: ;
        }

        var settled = Map.WhereValue(Settled).WhereKey(pos => range.Contains(pos.Y)).Count();
        var water = Map.WhereValue(Water).WhereKey(pos => range.Contains(pos.Y)).Count();
        WriteLn(water + settled);
        WriteLn(settled); // Part 2
    }
}