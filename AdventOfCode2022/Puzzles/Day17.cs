using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2022.Puzzles;

public class Day17 : Puzzle<int, long>
{
    public IEnumerable<Pos> Horizontal()
    {
        return Enumerable.Range(2, 4).Select(i => new Pos(i, 1));
    }

    public IEnumerable<Pos> Plus()
    {
        var center = new Pos(3, 2);
        return center.Adjacent().Append(center);
    }

    public IEnumerable<Pos> L()
    {
        yield return new Pos(2, 1);
        yield return new Pos(3, 1);
        yield return new Pos(4, 1);
        yield return new Pos(4, 2);
        yield return new Pos(4, 3);
    }

    public IEnumerable<Pos> Vertical()
    {
        return Enumerable.Range(1, 4).Select(i => new Pos(2, i));
    }

    public IEnumerable<Pos> Block()
    {
        yield return new Pos(2, 1);
        yield return new Pos(3, 1);
        yield return new Pos(2, 2);
        yield return new Pos(3, 2);
    }

    public HashSet<Pos> Pieces;
    public Func<IEnumerable<Pos>>[] Shapes;
    public int CurrentShape = -1;
    public int NextJet;
    public int Max;

    public Day17()
    {
        Pieces = new HashSet<Pos>();
        Shapes = new[] {Horizontal, Plus, L, Vertical, Block};
    }

    public void NextRock()
    {
        CurrentShape = (CurrentShape + 1) % Shapes.Length;
        var current = new Pos(0, Max + 3);
        
        IEnumerable<Pos> Shape(Pos shift = default) => Shapes[CurrentShape]().Shift(current + shift);
        
        while (true)
        {
            var dir = Pos.RelativeDirection(InputLine[NextJet++ % InputLine.Length]);
            // Push
            if (!Shape(dir).Any(pos => pos.X is < 0 or > 6 || Pieces.Contains(pos)))
            {
                current += dir;
            }
            // Fall
            if (!Shape(Pos.Down).Any(pos => pos.Y == 0 || Pieces.Contains(pos)))
            {
                current += Pos.Down;
            }
            else
            {
                foreach (var pos in Shape())
                {
                    Pieces.Add(pos);
                    Max = Math.Max(Max, pos.Y);
                }
                break;
            }
        }
    }

    public override int PartOne()
    {
        var rocks = 0;
        while (rocks < 2022)
        {
            NextRock();
            rocks++;
        }
        return Max;
    }

    public override long PartTwo()
    {
        IEnumerable<Pos> RowData(int r = default) => Enumerable.Range(0, 7).Select(i => new Pos(i, r == default ? Max : r));
        string Row(int r = default) => RowData(r).Select(p => Pieces.Contains(p) ? '1' : '0').Str();
        
        var cache = new Dictionary<int, (string Key, int Max)>();
        var rock = 0;
        const int signatureSize = 30;

        // Simulate a good number of rocks before trying to find the cycle.
        var rockCount = Shapes.Length.Lcm(InputLine.Length);
        rockCount.Times(() =>
        {
            NextRock();
            var key = Enumerable.Range(0, signatureSize).Select(i => Row(Max - i)).Str();
            cache[++rock] = (key, Max);
        });

        var bottom = cache.OrderByKeyDescending()
            .PullOne(out var top)
            .First(pair => pair.Value.Key == top.Value.Key);
        var size = top.Key - bottom.Key;
        var heightGain = top.Value.Max - bottom.Value.Max;

        var target = 1_000_000_000_000;
        var setup = bottom.Key;
        var fullCycles = (target - setup) / size;
        var extra = (target - setup) % size;
        var extraHeightGain = cache[(int) (rock - size + extra)].Max - bottom.Value.Max;
        
        return bottom.Value.Max + fullCycles * heightGain + extraHeightGain;
    }
}