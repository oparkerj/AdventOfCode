using System;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2019.Puzzles;

public class Day1 : Puzzle
{
    public int Fuel(int mass) => mass / 3 - 2;

    public override void PartOne()
    {
        var result = Input.Ints().Select(Fuel).Sum();
        WriteLn(result);
    }

    public int FuelRecursive(int mass)
    {
        var fuel = Math.Max(0, Fuel(mass));
        if (fuel > 0) fuel += FuelRecursive(fuel);
        return fuel;
    }

    public override void PartTwo()
    {
        var result = Input.Ints().Select(FuelRecursive).Sum();
        WriteLn(result);
    }
}