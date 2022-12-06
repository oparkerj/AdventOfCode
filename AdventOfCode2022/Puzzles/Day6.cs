using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2022.Puzzles;

public class Day6 : Puzzle<int>
{
    public int FindMarker(int size)
    {
        return InputLine.Window(size).FirstIndex(list => list.Distinct().Count() == size) + size;
    }
    
    public override int PartOne() => FindMarker(4);

    public override int PartTwo() => FindMarker(14);
}