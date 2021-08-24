using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;
using MoreLinq.Extensions;

namespace AdventOfCode2018.Puzzles
{
    public class Day6 : Puzzle
    {
        public Day6()
        {
            Part = 2;
        }

        public IEnumerable<Pos> GetPoints()
        {
            return Input.Select(Pos.Parse);
        }

        public override void PartOne()
        {
            var points = GetPoints().ToList();
            var area = new Grid<int> { Default = -1 };
            foreach (var (i, point) in points.Index())
            {
                area[point] = i;
            }
            // Get closest point for every position
            foreach (var pos in area.Bounds.Positions())
            {
                area[pos] = points.Index().MinBy(pair => pos.MDist(pair.Value)).Keys().SingleOrDefault(-1);
            }
            // All regions on the edges of the bounding box of the positions extend infinitely
            var infinite = Enumerable.ToHashSet(area.Bounds.GetAllSides().Select(pos => area[pos]).Distinct());
            var result = area.Values.Where(i => !infinite.Contains(i)).Frequencies().Max(pair => pair.Value);
            WriteLn(result);
        }

        public override void PartTwo()
        {
            var points = GetPoints().ToList();
            var area = new Grid<int> { Default = -1 };
            foreach (var (i, point) in points.Index())
            {
                area[point] = i;
            }
            var result = area.Bounds.Positions().Count(pos => points.Select(pos.MDist).Sum() < 10000);
            WriteLn(result);
        }
    }
}