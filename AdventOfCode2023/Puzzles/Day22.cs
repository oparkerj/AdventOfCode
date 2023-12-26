using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day22 : Puzzle<int>
{
    public Cube Parse(string s)
    {
        var (first, second) = s.SingleSplit('~');
        return new Cube(Pos3D.Parse(first), Pos3D.Parse(second));
    }

    public Dictionary<Cube, (List<Cube> On, List<Cube> Support)> SettleBricks(List<Cube> bricks)
    {
        var settled = new Dictionary<Cube, (List<Cube> On, List<Cube> Support)>();
        foreach (var cube in bricks.OrderBy(cube => cube.Z.Start))
        {
            var support = settled.Keys.Where(c => c.WouldSupport(cube)).MaxsBy(c => c.Z.End).ToList();
            if (support.Count == 0)
            {
                settled[cube.AtZ(1)] = ([], []);
            }
            else
            {
                var moved = cube.AtZ(support[0].Z.End);
                settled[moved] = ([..support], []);
                foreach (var c in support)
                {
                    settled[c].Support.Add(moved);
                }
            }
        }

        return settled;
    }
    
    public override int PartOne()
    {
        var bricks = Input.Select(Parse).ToList();
        var settled = SettleBricks(bricks);
        // A brick is safe to disintegrate if every brick it supports is on more than one brick.
        return settled.Values.Count(tuple => tuple.Support.All(c => settled[c].On.Count > 1));
    }

    public override int PartTwo()
    {
        var bricks = Input.Select(Parse).ToList();
        var settled = SettleBricks(bricks);
        
        return settled.Keys.Select(WouldFall).Sum();

        int WouldFall(Cube start)
        {
            var copy = new Dictionary<Cube, (List<Cube> On, List<Cube> Support)>(settled);

            var startSupport = copy[start].Support;
            HashSet<Cube> fallen = [start]; 
            HashSet<Cube> check = [..startSupport];
            
            var queue = new PriorityQueue<Cube, int>();
            foreach (var cube in startSupport)
            {
                queue.Enqueue(cube, cube.Z.Start);
            }
            
            while (queue.Count > 0)
            {
                var cube = queue.Dequeue();
                var (on, support) = copy[cube];
                if (!on.Except(fallen).Any())
                {
                    fallen.Add(cube);
                    foreach (var c in support)
                    {
                        if (check.Add(c))
                        {
                            queue.Enqueue(c, c.Z.Start);
                        }
                    }
                }
            }

            return fallen.Count - 1;
        }
    }

    public record Cube(Interval X, Interval Y, Interval Z)
    {
        public Cube(Pos3D a, Pos3D b) : this(
            Interval.RangeInclusive(a.X, b.X),
            Interval.RangeInclusive(a.Y, b.Y),
            Interval.RangeInclusive(a.Z, b.Z))
        { }

        public Cube AtZ(int z) => this with {Z = new Interval(z, Z.Length)};

        public bool WouldSupport(Cube other) => X.Overlaps(other.X) && Y.Overlaps(other.Y);
    }
}