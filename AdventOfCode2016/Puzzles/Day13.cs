using System.Numerics;
using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2016.Puzzles;

public class Day13 : Puzzle
{
    private int _num;

    public Day13() => _num = InputLine.AsInt();

    public bool HasWall(Pos pos)
    {
        var (x, y) = pos;
        var v = x * x + 3 * x + 2 * x * y + y + y * y;
        return BitOperations.PopCount((uint) (v + _num)) % 2 != 0;
    }

    public bool IsValid(Pos pos) => pos.X >= 0 && pos.Y >= 0 && !HasWall(pos);

    public Dijkstra<Pos> GetPathfinder()
    {
        return new Dijkstra<Pos>(PosExtensions.Adjacent);
    }

    public override void PartOne()
    {
        var dijkstra = GetPathfinder();

        var steps = dijkstra.ComputeFind(new Pos(1, 1), new Pos(31, 39), IsValid);
        WriteLn(steps);
    }

    public override void PartTwo()
    {
        var dijkstra = GetPathfinder();

        var map = dijkstra.ComputeFrom(new Pos(1, 1), (pos, i) => i <= 50 && IsValid(pos));
        WriteLn(map.Count);
    }
}