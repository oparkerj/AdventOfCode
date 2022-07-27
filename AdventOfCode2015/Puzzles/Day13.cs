using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;
using RegExtract;

namespace AdventOfCode2015.Puzzles;

public class Day13 : Puzzle
{
    public Dictionary<(string, string), int> Delta = new();

    public Day13() => ReadInput();

    public void ReadInput()
    {
        var input = Input.Extract<(string, string, int, string)>(@"(\w+) would (\w+) (\d+) happiness units? by sitting next to (\w+)");
        foreach (var (who, what, amount, other) in input)
        {
            Delta[(who, other)] = what == "gain" ? amount : -amount;
        }
    }

    public int Change(string a, string b) => Delta[(a, b)] + Delta[(b, a)];

    public override void PartOne()
    {
        var people = Delta.Keys.UnpackAll().Distinct().ToList();
        var result = people.Permutations()
            .Select(order => order.Pairwise(Change).Then(Change(order[0], order[^1])).Sum())
            .Max();
        WriteLn(result);
    }

    public override void PartTwo()
    {
        var people = Delta.Keys.UnpackAll().Distinct().ToList();
        foreach (var person in people)
        {
            Delta[(person, "You")] = 0;
            Delta[("You", person)] = 0;
        }
        PartOne();
    }
}