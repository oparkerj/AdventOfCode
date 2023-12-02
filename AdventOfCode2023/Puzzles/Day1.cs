using System.Buffers;
using System.Collections.Frozen;
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
        var numbers = new[]
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
            var first = numbers.Select(s => str.IndexOf(s)).Without(-1).Order().FirstOrDefault(int.MaxValue);
            if (str.First(char.IsDigit) is var firstNum && str.IndexOf(firstNum) is var firstIndex && firstIndex < first)
            {
                first = firstIndex;
            }

            var last = numbers.Select(s => str.LastIndexOf(s)).Without(-1).OrderDescending().FirstOrDefault(int.MinValue);
            if (str.Last(char.IsDigit) is var lastNum && str.LastIndexOf(lastNum) is var lastIndex &&  lastIndex > last)
            {
                last = lastIndex;
            }

            return $"{NumericValue(str, first)}{NumericValue(str, last)}".AsInt();
        }

        int NumericValue(string str, int index)
        {
            if (char.IsDigit(str[index])) return str[index].AsInt();
            var word = numbers.First(s => str.AsSpan(index).StartsWith(s));
            return Array.IndexOf(numbers, word) + 1;
        }
    }
}

public class Day1Better : Puzzle<int>
{
    public Day1Better()
    {
        InputName = CopyInput<Day1>();
        // Part = 1;
    }

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
            var firstIndex = s.IndexOfFirst(numbers.Keys).MinCompare();
            var firstDigit = s.AsSpan().IndexOfAny(digits);
            var first = firstIndex < firstDigit ? numbers[s.StartsWith(numbers.Keys, firstIndex)] : s[firstDigit].AsInt();

            var lastIndex = s.IndexOfLast(numbers.Keys);
            var lastDigit = s.AsSpan().LastIndexOfAny(digits);
            var last = lastIndex > lastDigit ? numbers[s.StartsWith(numbers.Keys, lastIndex)] : s[lastDigit].AsInt();
            
            return first * 10 + last;
        }
    }
}