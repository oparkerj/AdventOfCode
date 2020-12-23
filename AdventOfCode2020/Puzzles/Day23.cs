using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2020.Puzzles
{
    public class Day23 : Puzzle
    {
        public int Min = -1;
        public int Max = -1;
        
        public int Current;
        public readonly Dictionary<int, Cup> Cups = new();

        public Day23()
        {
            ReadInput();
            Part = 2;
        }

        public void ReadInput()
        {
            var cups = Input[0].Select(c => c.ToString()).Ints().ToArray();
            Cup prev = null;
            foreach (var cup in cups)
            {
                if (cup < Min || Min == -1) Min = cup;
                if (cup > Max || Max == -1) Max = cup;
                var c = new Cup(cup);
                Cups[cup] = c;
                if (prev != null) prev.Next = c;
                c.Prev = prev;
                prev = c;
            }
            Current = cups[0];
            Cups[Current].Prev = Cups[cups[^1]];
            Cups[cups[^1]].Next = Cups[Current];
        }

        public class Cup
        {
            public readonly int Id;
            public Cup Prev, Next;

            public Cup(int id)
            {
                Id = id;
            }

            public Cup this[int i]
            {
                get
                {
                    var c = this;
                    if (i > 0)
                    {
                        while (i-- > 0) c = c.Next;
                    }
                    else if (i < 0)
                    {
                        while (i++ < 0) c = c.Prev;
                    }
                    return c;
                }
            }
        }

        public Cup Remove(int cup)
        {
            var c = Cups[cup];
            c.Prev.Next = c.Next;
            c.Next.Prev = c.Prev;
            c.Prev = c.Next = null;
            return c;
        }

        public void Insert(Cup cup, Cup at)
        {
            at.Prev.Next = cup;
            cup.Prev = at.Prev;
            at.Prev = cup;
            cup.Next = at;
        }

        public void Move()
        {
            var c = Cups[Current];
            var remove = new[] {c[1], c[2], c[3]}.Select(cup => cup.Id).ToArray();
            var cups = remove.Select(Remove).ToArray();
            var dest = c.Id - 1;
            if (dest < Min) dest = Max;
            while (remove.Contains(dest))
            {
                dest--;
                if (dest < Min) dest = Max;
            }
            var insert = Cups[dest];
            for (var i = 0; i < 3; i++)
            {
                Insert(cups[i], insert[i + 1]);
            }
            Current = Cups[Current][1].Id;
        }
        
        public override void PartOne()
        {
            foreach (var _ in ..100)
            {
                Move();
            }
            var start = Cups[1];
            var cups = Enumerable.Range(0, Max - 1).Select(i => start[i + 1]).Select(cup => cup.Id.ToString());
            WriteLn(string.Concat(cups));
        }

        public override void PartTwo()
        {
            var start = Cups[Current];
            for (var i = Max + 1; i <= 1000000; i++)
            {
                var c = new Cup(i);
                Cups[i] = c;
                Insert(c, start);
            }
            Max = 1000000;
            foreach (var _ in ..10_000_000)
            {
                Move();
            }
            var result = (long) Cups[1][1].Id * Cups[1][2].Id;
            WriteLn(result);
        }
    }
}