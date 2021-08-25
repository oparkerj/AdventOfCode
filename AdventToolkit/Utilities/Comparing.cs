using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Utilities
{
    public class Comparing<T> : IComparer<T>
    {
        private readonly List<Func<T, T, int>> _comparisons = new();

        public static Comparing<T> Reversed()
        {
            return By((a, b) => -Comparer<T>.Default.Compare(a, b));
        }

        public static Comparing<T> By(Func<T, T, int> func)
        {
            var c = new Comparing<T>();
            c._comparisons.Add(func);
            return c;
        }

        public static Comparing<T> By<TC>(Func<T, TC> func)
            where TC : IComparable<TC>
        {
            return By((a, b) => func(a).CompareTo(func(b)));
        }
        
        public static Comparing<T> ByReverse<TC>(Func<T, TC> func)
            where TC : IComparable<TC>
        {
            return By((a, b) => func(b).CompareTo(func(a)));
        }

        public int Compare(T x, T y)
        {
            if (x is null) return y is null ? 0 : -1;
            if (y is null) return 1;
            return _comparisons.Select(func => func(x, y)).FirstOrDefault(i => i != 0);
        }

        public Comparing<T> ThenBy(Func<T, T, int> func)
        {
            _comparisons.Add(func);
            return this;
        }

        public Comparing<T> ThenBy<TC>(Func<T, TC> func)
            where TC : IComparable<TC>
        {
            return ThenBy((a, b) => func(a).CompareTo(func(b)));
        }
        
        public Comparing<T> ThenByReverse<TC>(Func<T, TC> func)
            where TC : IComparable<TC>
        {
            return ThenBy((a, b) => func(b).CompareTo(func(a)));
        }
    }
}