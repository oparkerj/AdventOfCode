using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections;

namespace AdventOfCode2021.Puzzles;

public class Day21 : Puzzle
{
    public Day21()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var p1 = 0;
        var p2 = 0;
        var p1Pos = Input[0][^1].AsInt();
        var p2Pos = Input[1][^1].AsInt();
        var first = true;
        var rolls = 0L;

        var map = new Interval(1, 10);

        foreach (var roll in new Interval(1, 100).Endless().Chunk(3))
        {
            rolls += 3;
            if (first)
            {
                p1Pos = (p1Pos + roll.Sum()).ModRange(map);
                p1 += p1Pos;
            }
            else
            {
                p2Pos = (p2Pos + roll.Sum()).ModRange(map);
                p2 += p2Pos;
            }
            first = !first;
            if (p1 >= 1000 || p2 >= 1000) break;
        }

        var result = Math.Min(p1, p2) * rolls;
        WriteLn(result);
    }

    public record State(int P1, int P2, int P1Score, int P2Score);

    public (long P1Win, long P2Win) WinsFrom(State state, Dictionary<State, (long, long)> known)
    {
        if (state.P1Score >= 21) return (1, 0);
        if (state.P2Score >= 21) return (0, 1);
        if (known.TryGetValue(state, out var wins)) return wins;
        var (a, b) = Algorithms.Sequences(3, 3).Aggregate((0L, 0L), (scores, rolls) =>
        {
            var pos = (state.P1 + rolls.Sum()).ModRange(new Interval(1, 10));
            var next = new State(state.P2, pos, state.P2Score, state.P1Score + pos);
            var (p2Won, p1Won) = WinsFrom(next, known);
            return (scores.Item1 + p1Won, scores.Item2 + p2Won);
        });
        known[state] = (a, b);
        return (a, b);
    }

    public override void PartTwo()
    {
        var p1Pos = Input[0][^1].AsInt();
        var p2Pos = Input[1][^1].AsInt();
        var state = new State(p1Pos, p2Pos, 0, 0);
        var (a, b) = WinsFrom(state, new Dictionary<State, (long, long)>());
        WriteLn(Math.Max(a, b));
    }
}