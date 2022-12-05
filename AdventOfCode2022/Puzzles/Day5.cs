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
        Stacks = new Stack<char>[9];
        Stacks.Init();

        foreach (var chars in AllGroups[0].Reverse().Skip(1).Select(s => s.TakeEvery(4, 1)))
        {
            foreach (var (i, c) in chars.Index())
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
            for (var i = 0; i < amount; i++)
            {
                to.Push(from.Pop());
            }
        }

        return Stacks.Select(stack => stack.Peek()).Str();
    }

    public override string PartTwo()
    {
        foreach (var (amount, fromIndex, toIndex) in AllGroups[1].Extract<Int3>(Patterns.UInt3))
        {
            var from = Stacks[fromIndex - 1];
            var to = Stacks[toIndex - 1];
            to.PushAll(from.Pop(amount).Reverse());
        }

        return Stacks.Select(stack => stack.Peek()).Str();
    }
}