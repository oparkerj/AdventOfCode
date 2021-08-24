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
        public IEnumerable<Pos> GetPoints()
        {
            return Input.Select(Pos.Parse);
        }

        public KeyValuePair<int, Pos> Closest(Pos pos, IList<Pos> points)
        {
            return points.Index().SelectMin(pair => pos.MDist(pair.Value));
        }

        public override void PartOne()
        {
            var points = GetPoints().ToList();
            var area = new Grid<int>();
            foreach (var (i, point) in points.Index())
            {
                area[point] = i;
            }
            foreach (var pos in area.Bounds.Positions())
            {
                area[pos] = Closest(pos, points).Key;
            }
            var infinite = Enumerable.ToHashSet(area.Bounds.GetAllSides().Select(pos => area[pos]).Distinct());
            var result = area.Values.Where(i => !infinite.Contains(i)).Frequencies().Max(pair => pair.Value);
            WriteLn(result);
            // TODO fix bug with this
        }
    }
}