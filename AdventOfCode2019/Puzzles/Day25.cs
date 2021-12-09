using AdventOfCode2019.IntCode;
using AdventToolkit;
using MoreLinq;

namespace AdventOfCode2019.Puzzles;

public class Day25 : Puzzle
{
    public override void PartOne()
    {
        var c = Computer.From(InputLine);
        var data = new DataLink(c);
        c.LineOut = Computer.AsciiOutput();

        var setup =
            @"south
take fixed point
north
west
west
west
take hologram
east
east
east
north
take candy cane
west
take antenna
south
take whirled peas
north
west
take shell
east
east
north
north
take polygon
south
west
take fuel cell
west
";
        var options = new[] {"fixed point", "hologram", "candy cane", "antenna", "whirled peas", "shell", "polygon", "fuel cell"};
            
        data.InsertAscii(setup);
        options.ForEach(s => data.InsertAscii($"drop {s}\n"));
            
        foreach (var subset in options.Subsets())
        {
            foreach (var item in subset)
            {
                data.InsertAscii($"take {item}\n");
            }
            data.InsertAscii("west\n");
            foreach (var item in subset)
            {
                data.InsertAscii($"drop {item}\n");
            }
        }
            
        c.Execute();
    }
}