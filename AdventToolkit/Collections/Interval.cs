using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Collections
{
    public readonly struct Interval : IEnumerable<int>
    {
        public readonly int Start;
        public readonly int Length;

        public Interval(int start, int length)
        {
            Start = start;
            Length = length;
        }

        public static Interval Range(int start, int end) => new(start, end - start);
        
        public static Interval RangeInclusive(int start, int end) => new(start, end - start + 1);

        public int End => Start + Length;

        public bool Contains(int i) => i >= Start && i < End;

        public bool ContainsInclusive(int i) => i >= Start && i <= End;

        public Interval Overlap(Interval other)
        {
            var (a, b) = (this, other);
            if (b.Start < a.Start) (a, b) = (b, a);
            if (b.Start < a.End) return new Interval(b.Start, Math.Min(a.End - b.Start, b.Length));
            return new Interval();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<int> GetEnumerator() => Enumerable.Range(Start, Length).GetEnumerator();
    }
}