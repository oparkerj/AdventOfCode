using AdventToolkit;
using AdventToolkit.Extensions;
using Z3Helper;

namespace AdventOfCode2023.Puzzles;

public class Day24 : Puzzle<long>
{
    public override long PartOne()
    {
        const long testMin = 200000000000000;
        const long testMax = 400000000000000;
        var stones = Input.Select(ParseHailstone<double>).ToList();
        return stones.Pairs().Count(IntersectsInTestArea);
        
        // TODO Try and do this without double
        bool IntersectsInTestArea(((double[] position, double[] velocity), (double[] position, double[] velocity)) tuple)
        {
            var (left, right) = tuple;
            // Slope
            var (m1, m2) = (left.velocity[1] / left.velocity[0], right.velocity[1] / right.velocity[0]);
            // Intercept
            var (b1, b2) = (left.position[1] - m1 * left.position[0], right.position[1] - m2 * right.position[0]);

            // Intersection of two lines
            var (ix, iy) = ((b2 - b1) / (m1 - m2), m1 * (b2 - b1) / (m1 - m2) + b1);
            if (!(ix is >= testMin and <= testMax && iy is >= testMin and <= testMax)) return false;
            // Make sure intersection is not in the past
            return Math.Sign(ix - left.position[0]) == Math.Sign(left.velocity[0])
                   && Math.Sign(iy - left.position[1]) == Math.Sign(left.velocity[1])
                   && Math.Sign(ix - right.position[0]) == Math.Sign(right.velocity[0])
                   && Math.Sign(iy - right.position[1]) == Math.Sign(right.velocity[1]);
        }
    }
    
    public override long PartTwo()
    {
        var stones = Input.Select(ParseHailstone<long>).ToList();

        var x = "x".RealConst();
        var y = "y".RealConst();
        var z = "z".RealConst();
        var vx = "vx".RealConst();
        var vy = "vy".RealConst();
        var vz = "vz".RealConst();
        
        var solver = Zzz.Solver();
        solver.Add(stones.SelectMany(GetEquations).Z3());
        
        WriteLn(solver.Check());
        return solver.GetAll<long>(x, y, z).Sum();
        
        // Get the equations that set the xyz of the hailstone equal to
        // the xyz of the thrown rock.
        ZbExpr[] GetEquations((long[], long[]) stone, int i)
        {
            var (pos, vel) = stone;
            var t = $"t{i}".RealConst();
            return
            [
                pos[0] + vel[0] * t == x + vx * t,
                pos[1] + vel[1] * t == y + vy * t,
                pos[2] + vel[2] * t == z + vz * t,
            ];
        }
    }

    private (T[] position, T[] velocity) ParseHailstone<T>(string s)
        where T : IParsable<T>
    {
        var (pos, vel) = s.SingleSplit('@');
        var position = pos.Split(',', StringSplitOptions.TrimEntries).Select(part => T.Parse(part, null)).ToArray();
        var velocity = vel.Split(',', StringSplitOptions.TrimEntries).Select(part => T.Parse(part, null)).ToArray();
        return (position, velocity);
    }
}