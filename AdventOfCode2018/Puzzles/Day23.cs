using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using RegExtract;
using Z3Helper;
// using MoreLinq;

namespace AdventOfCode2018.Puzzles
{
    public class Day23 : Puzzle
    {
        public Day23()
        {
            Part = 2;
        }

        public IEnumerable<Nanobot> ReadBots()
        {
            return Input.Extract<Nanobot>(@"pos=(<.+?>), r=(\d+)");
        }

        public override void PartOne()
        {
            var bots = ReadBots().ToArray(Input.Length);
            var (position, radius) = bots.MaxBy(bot => bot.Radius)!;
            var count = bots.Count(bot => bot.Position.MDist(position) <= radius);
            WriteLn(count);
        }

        public override void PartTwo()
        {
            var bots = ReadBots().ToArray(Input.Length);

            using var _ = Zzz.Context;

            var x = "x".IntConst();
            var y = "y".IntConst();
            var z = "z".IntConst();

            var inRange = bots.Select(bot => (
                    ((x - bot.Position.X).Abs() +
                     (y - bot.Position.Y).Abs() +
                     (z - bot.Position.Z).Abs()) <= bot.Radius).Condition(1, 0))
                .ToArray();

            var sum = "sum".IntConst();
            var dist = "dist".IntConst();

            var o = Zzz.Optimize();
            // sum = Number in range at x, y, z
            o.Add(sum == inRange.Sum());
            // dist = Manhattan distance to 0, 0, 0.
            o.Add(dist == x.Abs() + y.Abs() + z.Abs());

            var maxSum = o.MkMaximize(sum);
            var minDist = o.MkMinimize(dist);

            WriteLn(o.Check());
            WriteLn(maxSum.Value);
            WriteLn(minDist.Value);
        }

        public record Nanobot(Pos3D Position, int Radius);
    }
}