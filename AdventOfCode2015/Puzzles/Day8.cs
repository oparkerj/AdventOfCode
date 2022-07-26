using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2015.Puzzles;

public class Day8 : Puzzle
{
    public int MemoryLength(string s)
    {
        var remove = s.IndicesOf("\\", (str, i) => str[i + 1] == 'x' ? 4 : 2)
            .Select(i => s[i + 1] == 'x' ? 3 : 1)
            .Sum();
        return s.Length - remove - 2;
    }
    
    public override void PartOne()
    {
        var code = Input.Select(s => s.Length).Sum();
        var memory = Input.Select(MemoryLength).Sum();
        WriteLn(code - memory);
    }

    public int EncodedLength(string s) => s.Length + s.Count('\\') + s.Count('"') + 2;

    public override void PartTwo()
    {
        var result = Input.Select(EncodedLength).Sum();
        var code = Input.Select(s => s.Length).Sum();
        WriteLn(result - code);
    }
}