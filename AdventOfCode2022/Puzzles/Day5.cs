using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using MoreLinq;
using RegExtract;

namespace AdventOfCode2022.Puzzles;

public class Day5 : Puzzle<string>
{
    public override string PartOne()
    {
        var stacks = AllGroups[0].Reverse()
            .Skip(1)
            .Transpose()
            .TakeEvery(4, 1)
            .Select(stack => new Stack<char>(stack.Str().Trim()))
            .ToArray();
        
        foreach (var (amount, fromIndex, toIndex) in AllGroups[1].Extract<Int3>(Patterns.UInt3))
        {
            var from = stacks[fromIndex - 1];
            var to = stacks[toIndex - 1];
            to.PushAll(Part == 1 ? from.Pop(amount) : from.Pop(amount).Reverse());
        }

        return stacks.Select(stack => stack.Peek()).Str();
    }
}