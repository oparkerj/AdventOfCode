using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2022.Puzzles;

public class Day24 : Puzzle<int>
{
    public const char Open = '.';
    public const char Wall = '#';

    public override int PartOne()
    {
        var map = Input.ToGrid();
        var blizzards = new Dictionary<Pos, List<Pos>>();
        foreach (var (pos, c) in map.ToList())
        {
            if (map[pos] is not Wall and not Open)
            {
                blizzards.GetOrNew(pos).Add(Pos.RelativeDirection(c));
                map[pos] = Open;
            }
        }

        // Find the openings
        var start = map.Bounds.GetSidePositions(Side.Top).Single(pos => map[pos] == Open);
        var originalTarget = map.Bounds.GetSidePositions(Side.Bottom).Single(pos => map[pos] == Open);
        var currentTarget = originalTarget;

        // Fill in the walls
        map[start] = Wall;
        map[originalTarget] = Wall;

        // Advance the blizzard by one minute
        void AdvanceBlizzards()
        {
            var list = blizzards.ToList();
            blizzards.Clear();
            foreach (var (pos, dirs) in list)
            {
                foreach (var dir in dirs)
                {
                    var target = pos + dir;
                    if (map[target] == Wall)
                    {
                        target = pos.Trace(-dir, p => map[p] == Wall) + dir;
                    }
                    blizzards.GetOrNew(target).Add(dir);
                }
            }
        }

        // Number of minutes before the states repeat
        var cycle = (map.Bounds.Width - 1).Lcm(map.Bounds.Height - 1);
        var states = new CircularBuffer<Dictionary<Pos, List<Pos>>>(cycle);

        // Pre-calculate the blizzard states
        foreach (var i in Enumerable.Range(0, cycle))
        {
            states[i] = new Dictionary<Pos, List<Pos>>(blizzards);
            AdvanceBlizzards();
        }

        IEnumerable<(Pos, int)> GetMoves((Pos Pos, int Time) current)
        {
            var p = current.Pos;
            var nextState = states[current.Time + 1];
            // ReSharper disable once AccessToModifiedClosure
            var next = p.Adjacent()
                .Where(pos => (map[pos] == Open || pos == currentTarget) && !nextState.ContainsKey(pos))
                .Select(pos => (pos, current.Time + 1))
                .AppendIf((p, current.Time + 1), !nextState.ContainsKey(p));
            return next;
        }

        var initial = (start, 0);
        var dijkstra = new Dijkstra<(Pos Pos, int Time)>(GetMoves);
        
        var (steps, targetState) = dijkstra.ComputeFind(initial, tuple => tuple.Pos == originalTarget);
        if (Part == 1) return steps;
        
        currentTarget = start;
        var (steps2, targetState2) = dijkstra.ComputeFind(targetState, tuple => tuple.Pos == start);
        currentTarget = originalTarget;
        var (steps3, _) = dijkstra.ComputeFind(targetState2, tuple => tuple.Pos == currentTarget);
        return steps + steps2 + steps3;
    }
}