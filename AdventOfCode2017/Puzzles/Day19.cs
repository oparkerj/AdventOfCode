using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2017.Puzzles
{
    public class Day19 : Puzzle
    {
        public Day19()
        {
            Part = 2;
        }

        public IEnumerable<Pos> GetPath(Grid<char> grid)
        {
            var last = grid.Positions.OrderBy(Pos.ReadingOrder).First(pos => grid[pos] == '|');
            var dir = Pos.Down;
            yield return last;
            while (true)
            {
                last = last.Trace(dir, pos => grid[pos] is ' ' or '+');
                yield return last;
                if (grid[last] == ' ') break;
                dir = grid.GetNeighbors(Pos.Origin)
                    .Without(-dir)
                    .First(pos => grid[last + pos] != ' ');
            }
        }

        public override void PartOne()
        {
            var grid = Input.ToGrid();
            var result = GetPath(grid).ConnectLines().SelectWhere((Pos pos, out char c) =>
            {
                c = grid[pos];
                return char.IsLetter(c);
            }).Str();
            WriteLn(result);
        }

        public override void PartTwo()
        {
            var grid = Input.ToGrid();
            var count = GetPath(grid).ConnectLines().Count();
            WriteLn(count);
        }
    }
}