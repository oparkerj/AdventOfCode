using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2020.Puzzles;

public class Day2 : Puzzle
{
    public Day2()
    {
        Part = 2;
    }

    public record Policy(int Lower, int Upper, char Letter, string Password);

    public IEnumerable<Policy> Policies()
    {
        return Input.Extract<Policy>(@"^(\d+)-(\d+) (.): (.+)$");
    }

    public bool IsValid(Policy policy)
    {
        return Interval.RangeInclusive(policy.Lower, policy.Upper).Contains(policy.Password.Count(policy.Letter));
    }
        
    public override void PartOne()
    {
        WriteLn(Policies().Count(IsValid));
    }
        
    public bool IsValid2(Policy policy)
    {
        return policy.Password[policy.Lower - 1] == policy.Letter ^ policy.Password[policy.Upper - 1] == policy.Letter;
    }

    public override void PartTwo()
    {
        WriteLn(Policies().Count(IsValid2));
    }
}