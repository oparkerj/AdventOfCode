using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using MoreLinq;
using RegExtract;

namespace AdventOfCode2022.Puzzles;

public class Day5 : Puzzle<string>
{
    public Stack<char>[] Stacks;

    public Day5()
    {
        Stacks = new Stack<char>[AllGroups[0][^1].Count(char.IsDigit)];
        Stacks.Init();

        foreach (var line in AllGroups[0][..^1].Reverse())
        {
            foreach (var (i, c) in line.TakeEvery(4, 1).Index())
            {
                if (c == ' ') continue;
                Stacks[i].Push(c);
            }
        }
    }

    public override string PartOne()
    {
        foreach (var (amount, fromIndex, toIndex) in AllGroups[1].Extract<Int3>(Patterns.UInt3))
        {
            var from = Stacks[fromIndex - 1];
            var to = Stacks[toIndex - 1];
            to.PushAll(Part == 1 ? from.Pop(amount) : from.Pop(amount).Reverse());
        }

        return Stacks.Select(stack => stack.Peek()).Str();
    }
}