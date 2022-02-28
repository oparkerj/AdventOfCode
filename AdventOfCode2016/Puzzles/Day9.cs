using System.Text;
using System.Text.RegularExpressions;
using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2016.Puzzles;

public class Day9 : Puzzle
{
    private Regex _marker = new(@"\((\d+)x(\d+)\)", RegexOptions.Compiled);
    
    public string Decompress()
    {
        var b = new StringBuilder();
        var pos = 0;
        Match m;
        while ((m = _marker.Match(InputLine, pos)).Success)
        {
            if (m.Index > pos) b.Append(InputLine[pos..m.Index]);
            var (length, count) = Pos.Parse(m.Value);
            var end = m.Index + m.Length;
            var part = InputLine.Substring(end, length).Repeat(count);
            b.Append(part);
            pos = end + length;
        }
        return b.ToString();
    }
    
    public override void PartOne()
    {
        var result = Decompress();
        WriteLn(result.Length);
    }

    public long DecompressedLength(string input)
    {
        var result = 0L;
        var pos = 0;
        Match m;
        while ((m = _marker.Match(input, pos)).Success)
        {
            result += m.Index - pos;
            var (length, count) = Pos.Parse(m.Value);
            var end = m.Index + m.Length;
            var part = input.Substring(end, length);
            result += DecompressedLength(part) * count;
            pos = end + length;
        }
        result += input.Length - pos;
        return result;
    }

    public override void PartTwo()
    {
        var result = DecompressedLength(InputLine);
        WriteLn(result);
    }
}