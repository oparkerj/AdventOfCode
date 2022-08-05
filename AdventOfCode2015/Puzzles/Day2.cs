using AdventToolkit;
using AdventToolkit.Common;
using RegExtract;

namespace AdventOfCode2015.Puzzles;

public class Day2 : Puzzle<int>
{
    public record Box(int X, int Y, int Z)
    {
        public int Paper => 2 * X * Y + 2 * Y * Z + 2 * X * Z + Math.Min(X * Y, Math.Min(X * Z, Y * Z));

        public int Ribbon => Math.Min(X + Y, Math.Min(X + Z, Y + Z)) * 2 + X * Y * Z;
    }

    public override int PartOne()
    {
        return Input.Extract<Box>(Patterns.Int3).Select(box => box.Paper).Sum();
    }

    public override int PartTwo()
    {
        return Input.Extract<Box>(Patterns.Int3).Select(box => box.Ribbon).Sum();
    }
}