using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2019.Puzzles
{
    public class Day10 : Puzzle
    {
        public Grid<bool> Grid;

        public Day10()
        {
            Grid = Input.QuickMap('#', true, false).ToGrid(false);
            Part = 2;
        }

        // Visible if tracing to the target equals the target
        public bool IsVisible(Pos from, Pos p)
        {
            return from.Trace(from.Towards(p), pos => Grid[pos]) == p;
        }

        public IEnumerable<Pos> Visible(Pos p)
        {
            return Grid.WhereValue(true)
                .Keys()
                .Without(p)
                .Where(pos => IsVisible(p, pos));
        }

        public int VisibleCount(Pos p)
        {
            return Visible(p).Count();
        }

        public override void PartOne()
        {
            var max = Grid.WhereValue(true).Keys().Max(VisibleCount);
            WriteLn(max);
        }

        public override void PartTwo()
        {
            var station = Grid.WhereValue(true).Keys().OrderByDescending(VisibleCount).First();
            var count = 200;
            while (count > 0)
            {
                var visible = Visible(station).ToList();
                if (visible.Count >= count)
                {
                    // Sort field by laser order.
                    // Sign of cross product is the same as
                    // the angle between the two.
                    visible.Sort((a, b) => b.Cross(a));
                    var (x, y) = visible[count - 1];
                    WriteLn(x * 100 + y);
                    return;
                }
                foreach (var pos in visible)
                {
                    Grid[pos] = false;
                }
                count -= visible.Count;
            }
        }
    }
}