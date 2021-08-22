using System;
using System.Linq;
using System.Numerics;
using AdventToolkit;

namespace AdventOfCode2019.Puzzles
{
    public class Day22 : Puzzle
    {
        public const int Size = 10007;

        public int[] Deck = Enumerable.Range(0, Size).ToArray();
        public int[] Temp = new int[Size];
        public int Top = 0;
        public int Dir = 1;

        public Day22()
        {
            Part = 2;
        }

        public int Wrap(long val, int size) => (int) ((val % size + size) % size);

        public void IncrementAndWrap(int offset)
        {
            Top += offset * Dir;
            Top = Wrap(Top, Size);
        }

        public void DealIntoNewStack()
        {
            IncrementAndWrap(-1);
            Dir *= -1;
        }

        public void Cut(int n)
        {
            IncrementAndWrap(n);
        }

        public void DealWithIncrement(long n)
        {
            for (var i = 0; i < Size; i++)
            {
                Temp[Wrap(i * n, Size)] = Deck[Wrap(Top + i * Dir, Size)];
            }
            var t = Temp;
            Temp = Deck;
            Deck = t;
            Top = 0;
            Dir = 1;
        }

        public override void PartOne()
        {
            foreach (var s in Input)
            {
                if (s == "deal into new stack") DealIntoNewStack();
                else if (s.StartsWith("deal with increment")) DealWithIncrement(int.Parse(s[20..]));
                else if (s.StartsWith("cut")) Cut(int.Parse(s[4..]));
            }
            DealWithIncrement(1);
            WriteLn(Array.IndexOf(Deck, 2019));
        }

        public BigInteger Mod(BigInteger a, BigInteger b)
        {
            return (a % b + b) % b;
        }

        public BigInteger Inverse(BigInteger a, BigInteger n)
        {
            return BigInteger.ModPow(a, n - 2, n);
        }

        public override void PartTwo()
        {
            BigInteger size = 119315717514047;
            BigInteger count = 101741582076661;
            BigInteger offset = 0;
            BigInteger increment = 1;
            
            foreach (var s in Input)
            {
                if (s == "deal into new stack")
                {
                    increment = -increment;
                    offset = Mod(offset + increment, size);
                }
                else if (s.StartsWith("deal with increment"))
                {
                    increment *= Inverse(int.Parse(s[20..]), size);
                    increment %= size;
                }
                else if (s.StartsWith("cut"))
                {
                    offset += increment * int.Parse(s[4..]);
                    offset = Mod(offset, size);
                }
            }

            var diff = offset;
            var mul = increment;
            increment = BigInteger.ModPow(mul, count, size);
            offset = diff * (1 - BigInteger.ModPow(mul, count, size)) * Inverse(1 - mul, size);
            offset = Mod(offset, size);
            WriteLn(Mod(offset + increment * 2020, size));
        }
    }
}