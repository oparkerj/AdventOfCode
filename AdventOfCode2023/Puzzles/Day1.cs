using System.Buffers;
using System.Collections.Frozen;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day1 : Puzzle<int>
{
    public override int PartOne()
    {
        return Input.Select(Value).Sum();
        
        int Value(string s)
        {
            var first = s.First(char.IsDigit).AsInt();
            var last = s.Last(char.IsDigit).AsInt();
            return first * 10 + last;
        }
    }

    public override int PartTwo()
    {
        var digits = SearchValues.Create("0123456789");
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
        }.ToFrozenDictionary();

        return Input.Select(Value).Sum();

        int Value(string s)
        {
            var (firstWord, firstIndex) = s.FindFirst(numbers.Keys);
            var firstDigit = s.AsSpan().IndexOfAny(digits);
            var first = firstIndex.MinCompare() < firstDigit ? numbers[firstWord] : s[firstDigit].AsInt();

            var (lastWord, lastIndex) = s.FindLast(numbers.Keys);
            var lastDigit = s.AsSpan().LastIndexOfAny(digits);
            var last = lastIndex > lastDigit ? numbers[lastWord] : s[lastDigit].AsInt();
            
            return first * 10 + last;
        }
    }
}