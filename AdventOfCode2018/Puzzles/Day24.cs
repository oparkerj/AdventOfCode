using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Solvers;
using AdventToolkit.Utilities;
using RegExtract;

namespace AdventOfCode2018.Puzzles;

public class Day24 : Puzzle
{
    public Day24()
    {
        Part = 2;
    }

    public Fight<Group> SetupFight(int boost = 0)
    {
        var groups = Groups.SelectMany(groups =>
        {
            var friendly = groups[0].Contains("Immune");
            return groups.Skip(1)
                .Extract<Group>(@"(?<UnitCount>\d+) units each with (?<UnitHp>\d+) hit points (?:\((?<Mods>.+?)\) )?with an attack that does (?<Damage>\d+) (?<DamageType>.+?) damage at initiative (?<Initiative>\d+)")
                .Peek(group =>
                {
                    group.Friendly = friendly;
                    if (friendly) group.Damage += boost;
                });
        });
        var fight = new ImmuneSystem();
        fight.Units.AddRange(groups);
        return fight;
    }
        
    public override void PartOne()
    {
        var fight = SetupFight();
        fight.RunBattle();
        WriteLn(fight.Units.Select(group => group.UnitCount).Sum());
    }

    public override void PartTwo()
    {
        var boost = 0;
        while (true)
        {
            var fight = SetupFight(boost++);
            fight.RunBattle();
            if (fight.Units.Select(group => group.Friendly).AllEqual(true))
            {
                WriteLn(fight.Units.Select(group => group.UnitCount).Sum());
                return;
            }
        }
    }

    public class Group
    {
        public bool Friendly { get; set; }
        public int UnitCount { get; set; }
        public int UnitHp { get; set; }
        public List<string> Weaknesses { get; set; }
        public List<string> Immunities { get; set; }
        public int Damage { get; set; }
        public string DamageType { get; set; }
        public int Initiative { get; set; }

        public int EffectivePower => UnitCount * Damage;

        public bool Alive => UnitCount > 0;

        public string Mods
        {
            set
            {
                if (value != null)
                {
                    foreach (var list in value.Split("; "))
                    {
                        if (list.StartsWith("weak")) Weaknesses = list[8..].Csv(true).ToList();
                        else if (list.StartsWith("immune")) Immunities = list[10..].Csv(true).ToList();
                    }
                }
                Weaknesses ??= new List<string>();
                Immunities ??= new List<string>();
            }
        }

        public int EffectiveDamage(Group source) => EffectiveDamage(source.EffectivePower, source.DamageType);

        public int EffectiveDamage(int power, string type)
        {
            if (Immunities.Contains(type)) return 0;
            if (Weaknesses.Contains(type)) return power * 2;
            return power;
        }

        public int ComputeLoss(int power, string type) => EffectiveDamage(power, type) / UnitHp;

        public void ApplyDamage(int power, string type) => UnitCount -= ComputeLoss(power, type);

        public override string ToString()
        {
            return $"{Friendly.AsInt()}, {UnitCount}";
        }
    }

    public class ImmuneSystem : Fight<Group>
    {
        public IComparer<Group> SelectionOrder = Comparing<Group>.ByReverse(group => group.EffectivePower)
            .ThenByReverse(group => group.Initiative);

        public IComparer<Group> Initiative = Comparing<Group>.By(group => group.Initiative);

        public override bool Tick()
        {
            var pool = Units.ToList();

            var targets = Units.OrderBy(SelectionOrder).With(group => ChooseTarget(group, pool)).ToArray(Units.Count);
            if (targets.WhereValue(group => group != null).All(pair => pair.Value.ComputeLoss(pair.Key.EffectivePower, pair.Key.DamageType) == 0)) return false;
            targets.OrderByDescending(pair => pair.Key, Initiative)
                .ForEach(Attack);
            Units.RemoveAll(group => !group.Alive);
            return !Units.AllEqual(group => group.Friendly);
        }

        public Group ChooseTarget(Group attacking, List<Group> pool)
        {
            var target = pool.Where(group => group.Friendly != attacking.Friendly)
                .OrderByDescending(group => group.EffectiveDamage(attacking))
                .ThenByDescending(group => group.EffectivePower)
                .ThenByDescending(group => group.Initiative)
                .FirstOrDefault();
            if (target?.EffectiveDamage(attacking.EffectivePower, attacking.DamageType) == 0) return null;
            pool.Remove(target);
            return target;
        }

        public void Attack(Group source, Group target)
        {
            if (!source.Alive) return;
            target?.ApplyDamage(source.EffectivePower, source.DamageType);
        }
    }
}