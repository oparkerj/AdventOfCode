using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day15 : Puzzle<int>
{
    public static int Hash(string s)
    {
        var current = 0;
        foreach (var c in s)
        {
            current += c;
            current *= 17;
            current %= 256;
        }
        return current;
    }
    
    public override int PartOne() => InputLine.Csv().Select(Hash).Sum();

    public record Lens(string Label, int Value);

    public override int PartTwo()
    {
        var boxes = new List<Lens>[256];
        for (var i = 0; i < boxes.Length; i++)
        {
            boxes[i] = [];
        }
        
        foreach (var inst in InputLine.Csv())
        {
            if (inst.Contains('='))
            {
                var (label, value) = inst.SingleSplit('=');
                var index = Hash(label);
                var found = boxes[index].FindIndex(l => l.Label == label);
                if (found > -1)
                {
                    // If the lens already exists, update the focal length
                    boxes[index][found] = new Lens(label, value.AsInt());
                }
                else
                {
                    // Otherwise add a new one
                    boxes[index].Add(new Lens(label, value.AsInt()));
                }
            }
            else
            {
                // Remove the lens
                var label = inst[..^1];
                var index = Hash(label);
                boxes[index].RemoveAll(l => l.Label == label);
            }
        }
        
        return boxes.SelectMany((box, num) => box.Select((label, slot) => (num + 1) * (slot + 1) * label.Value)).Sum();
    }
}