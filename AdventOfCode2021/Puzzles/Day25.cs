using AdventToolkit.Collections;
using AdventToolkit.Collections.Space;

namespace AdventOfCode2021.Puzzles;

public class Day25 : Puzzle
{
    public enum Dir
    {
        Nothing,
        East,
        South
    }

    public override void PartOne()
    {
        var width = InputLine.Length;
        var height = Input.Length;
        
        // Set up special grid
        var grid = new PortalGrid<Dir>();
        grid.WrapEdges(new Rect(width, height));
        var temp = PortalGrid<Dir>.CreateFrom(grid);

        int Step()
        {
            var count = 0;
            var next = temp;
            temp.Clear();
            foreach (var (key, _) in grid.WhereValue(Dir.East))
            {
                var target = grid.GetNeighbor(key, Side.Right);
                if (grid[target] == Dir.Nothing)
                {
                    next[target] = Dir.East;
                    count++;
                }
                else next[key] = Dir.East;
            }
            foreach (var (key, _) in grid.WhereValue(Dir.South))
            {
                var target = grid.GetNeighbor(key, Side.Bottom);
                if (grid[target] != Dir.South && next[target] == Dir.Nothing)
                {
                    next[target] = Dir.South;
                    count++;
                }
                else next[key] = Dir.South;
            }
            Data.Swap(ref grid, ref temp);
            return count;
        }
        
        foreach (var (key, value) in Input.ToGridQ1())
        {
            grid[key] = value == '>' ? Dir.East : value == 'v' ? Dir.South : Dir.Nothing;
        }

        var steps = 0;
        while (true)
        {
            steps++;
            if (Step() == 0) break;
        }
        
        WriteLn(steps);
    }
}