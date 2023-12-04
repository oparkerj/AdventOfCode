using AdventToolkit;
using AdventToolkit.Attributes;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

[CopyResult]
public class Day4 : Puzzle<int>
{
    public int WinCount(int index)
    {
        var card = Input[index];
        var winning = card.Between(':', '|').Spaced().Ints().ToList();
        var have = card.After('|').Spaced().Ints().ToList();
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
        var wins = Data.Memoize<int, int>(WinCount);
        
        var queue = new Queue<int>(Enumerable.Range(0, Input.Length));
        var total = 0;
        while (queue.Count > 0)
        {
            total++;
            var index = queue.Dequeue();
            var count = wins(index);
            foreach (var next in Enumerable.Range(index + 1, count))
            {
                queue.Enqueue(next);
            }
        }
        return total;
    }
}