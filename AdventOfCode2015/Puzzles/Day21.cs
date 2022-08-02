using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Extensions;

namespace AdventOfCode2015.Puzzles;

public class Day21 : Puzzle
{
    public List<Item> Weapons = new()
    {
        new Item(8, 4, 0),
        new Item(10, 5, 0),
        new Item(25, 6, 0),
        new Item(40, 7, 0),
        new Item(74, 8, 0),
    };

    public List<Item> Armor = new()
    {
        new Item(0, 0, 0),
        new Item(13, 0, 1),
        new Item(31, 0, 2),
        new Item(53, 0, 3),
        new Item(75, 0, 4),
        new Item(102, 0, 5),
    };

    public List<Item> Rings = new()
    {
        new Item(0, 0, 0),
        new Item(25, 1, 0),
        new Item(50, 2, 0),
        new Item(100, 3, 0),
        new Item(20, 0, 1),
        new Item(40, 0, 2),
        new Item(80, 0, 3),
    };

    public Dictionary<string, int> Info;

    public record Item(int Cost, int Damage, int Armor);

    public Day21()
    {
        Info = Input.ReadKeys(s => s, int.Parse);
    }

    public IEnumerable<IList<Item>> GetCombinations()
    {
        var first = Algorithms.Sequences(new Interval(Weapons.Count), new Interval(Armor.Count)).MakeUnique().ToList();
        var second = Algorithms.SequencesIncreasing(2, Rings.Count, true).MakeUnique().Prepend(new int[2]).ToList();
        return Algorithms.Sequences(new Interval(first.Count), new Interval(second.Count))
            .Select(chosen =>
            {
                var f = first[chosen[0]];
                return Weapons[f[0]].EnumerateSingle()
                    .Then(Armor[f[1]])
                    .Concat(Rings.Get(second[chosen[1]]));
            })
            .Select(items => items.ToList());
    }

    public bool CheckWin(IList<Item> items)
    {
        var myDamage = Math.Max(1, items.Sum(item => item.Damage) - Info["Armor"]);
        var bossDamage = Math.Max(1, Info["Damage"] - items.Sum(item => item.Armor));
        return myDamage >= bossDamage;
    }

    public override void PartOne()
    {
        var result = GetCombinations()
            .Where(CheckWin)
            .Select(list => list.Sum(item => item.Cost))
            .Min();
        WriteLn(result);
    }

    public override void PartTwo()
    {
        var result = GetCombinations()
            .Where(items => !CheckWin(items))
            .Select(list => list.Sum(item => item.Cost))
            .Max();
        WriteLn(result);
    }
}