using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day4 : Puzzle<int>
{
    public int WinCount(int index)
    {
        var card = Input[index];
        var winning = card.Between(':', '|').Spaced().Ints().ToHashSet();
        var have = card.After('|').Spaced().Ints().ToHashSet();
        return have.Count(winning.Contains);
    }
    
    public override int PartOne()
    {
        return Enumerable.Range(0, Input.Length)
            .Select(WinCount)
            .Select(wins => 2.Pow(wins - 1))
            .Sum();
    }

    public override int PartTwo()
    {
        var scores = new List<int>();
        var total = 0;
        for (var i = 0; i < Input.Length; i++)
        {
            var wins = WinCount(Input.Length - i - 1);
            var gain = 1;
            for (var prev = 1; prev <= wins; prev++)
            {
                gain += scores[i - prev];
            }
            scores.Add(gain);
            total += gain;
        }
        return total;
    }
}