using System.Numerics;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2022.Puzzles;

public class Day11 : Puzzle<BigInteger>
{
    public override BigInteger PartOne()
    {
        var mod = AllGroups.Select(data => data[3].GetNextInt()).LongProduct();
        var monkeys = AllGroups.Select(data => new Monkey(data) {Mod = mod, Part = Part}).ToList();

        var count = Part == 1 ? 20 : 10000;
        for (var i = 0; i < count; i++)
        {
            foreach (var monkey in monkeys)
            {
                monkey.DoTurn(monkeys);
            }
        }
        
        return monkeys.Select(monkey => monkey.Inspections).TakeMax(2).Product();
    }

    public class Monkey
    {
        public Queue<BigInteger> Items = new();
        public Func<BigInteger, BigInteger> Op;
        public int Divisible;
        public int IfTrue;
        public int IfFalse;
        public long Mod;
        public int Part;

        public BigInteger Inspections;

        public void Add(BigInteger i) => Items.Enqueue(i);

        public Monkey(string[] data)
        {
            foreach (var i in data[1].GetInts())
            {
                Items.Enqueue(i);
            }
            var op = data[2].After("new = ");
            var parts = op.Split();
            Func<BigInteger, BigInteger, BigInteger> func = parts[1][0] == '*' ? (a, b) => a * b : (a, b) => a + b;
            var leftPart = parts[0];
            var rightPart = parts[2];
            Op = old =>
            {
                var l = leftPart.StartsWith('o') ? old : leftPart.AsInt();
                var r = rightPart.StartsWith('o') ? old : rightPart.AsInt();
                return func(l, r);
            };
            Divisible = data[3].GetNextInt();
            IfTrue = data[4].GetNextInt();
            IfFalse = data[5].GetNextInt();
        }

        public void DoTurn(List<Monkey> monkeys)
        {
            while (Items.Count > 0)
            {
                var item = Items.Dequeue();
                item = Op(item) % Mod;
                Inspections++;
                if (Part == 1) item /= 3;
                if (item % Divisible == 0) monkeys[IfTrue].Add(item);
                else monkeys[IfFalse].Add(item);
            }
        }
    }
}