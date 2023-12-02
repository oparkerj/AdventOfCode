using System.Collections.Frozen;
using AdventToolkit;
using AdventToolkit.Collections.Tree;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day1 : Puzzle<int>
{
    public override int PartOne()
    {
        return Input.Select(s =>
        {
            var first = s.First(char.IsDigit).AsInt();
            var last = s.Last(char.IsDigit).AsInt();
            return first * 10 + last;
        }).Sum();
    }

    public override int PartTwo()
    {
        var numbers = new Dictionary<string, int>
        {
            ["one"] = 1,
            ["two"] = 2,
            ["three"] = 3,
            ["four"] = 4,
            ["five"] = 5,
            ["six"] = 6,
            ["seven"] = 7,
            ["eight"] = 8,
            ["nine"] = 9,
            ["0"] = 0,
            ["1"] = 1,
            ["2"] = 2,
            ["3"] = 3,
            ["4"] = 4,
            ["5"] = 5,
            ["6"] = 6,
            ["7"] = 7,
            ["8"] = 8,
            ["9"] = 9,
        }.ToFrozenDictionary();

        var trie = new Trie<string, char>();
        foreach (var word in numbers.Keys)
        {
            trie.Add(word);
            trie.AddReverse(word);
        }

        return Input.Select(s =>
        {
            trie.TryFindValue(s, out var first);
            trie.TryFindValueLast(s, out var last);
            return numbers[first] * 10 + numbers[last];
        }).Sum();
    }
}