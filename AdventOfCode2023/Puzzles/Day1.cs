using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day1 : Puzzle<int>
{
    public override int PartOne()
    {
        return Input
            .Select(s => $"{s.First(char.IsDigit)}{s.Last(char.IsDigit)}")
            .Ints()
            .Sum();
    }

    // TODO clean up this ugly solution
    public override int PartTwo()
    {
        var digits = new[]
        {
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine"
        };

        return Input.Select(GetValue).Sum();

        int GetValue(string str)
        {
            var first = digits.Select(s => str.IndexOf(s)).Without(-1).Order().FirstOrDefault(int.MaxValue);
            if (str.First(char.IsDigit) is var firstNum && str.IndexOf(firstNum) is var firstIndex && firstIndex < first)
            {
                first = firstIndex;
            }

            var last = digits.Select(s => str.LastIndexOf(s)).Without(-1).OrderDescending().FirstOrDefault(int.MinValue);
            if (str.Last(char.IsDigit) is var lastNum && str.LastIndexOf(lastNum) is var lastIndex &&  lastIndex > last)
            {
                last = lastIndex;
            }

            return $"{NumericValue(str, first)}{NumericValue(str, last)}".AsInt();
        }

        int NumericValue(string str, int index)
        {
            if (char.IsDigit(str[index])) return str[index].AsInt();
            var word = digits.First(s => str.AsSpan(index).StartsWith(s));
            return Array.IndexOf(digits, word) + 1;
        }
    }
}