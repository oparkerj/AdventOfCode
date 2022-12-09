using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2022.Puzzles;

public class Day9 : Puzzle<int>
{
    public int SimulateKnots(int count)
    {
        var knots = new Pos[count];
        var seen = new HashSet<Pos> {knots[^1]};
        
        void MoveHead(Pos dir)
        {
            knots[0] += dir;
            for (var i = 0; i < knots.Length - 1; i++)
            {
                var delta = knots[i] - knots[i + 1];
                if (delta.Abs().Max() > 1) knots[i + 1] += delta.Normalize();
            }
            seen.Add(knots[^1]);
        }

        foreach (var (dir, length) in Input.Extract<(char, int)>(@"(.) (\d+)"))
        {
            var delta = Pos.RelativeDirection(dir);
            length.Times(() => MoveHead(delta));
        }
        return seen.Count;
    }
    
    public override int PartOne() => SimulateKnots(2);

    public override int PartTwo() => SimulateKnots(10);
}