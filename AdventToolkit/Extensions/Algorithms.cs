using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections.Space;
using AdventToolkit.Utilities;
using AdventToolkit.Utilities.Arithmetic;
using MoreLinq;

namespace AdventToolkit.Extensions
{
    public static class Algorithms
    {
        public static int SumRange(int min, int max)
        {
            return (max - min + 1) * (max + min) / 2;
        }

        public static int Sum(this IEnumerable<int> ints, out int min, out int max)
        {
            bool start = false, end = false;
            var sum = 0;
            min = 0;
            max = 0;
            foreach (var i in ints)
            {
                sum += i;
                if (!start || i < min)
                {
                    start = true;
                    min = i;
                }
                if (!end || i > max)
                {
                    end = true;
                    max = i;
                }
            }
            return sum;
        }

        public static T Sum<T>(this IEnumerable<T> source)
            where T : IAdd<T>
        {
            return source.Aggregate((a, b) => a.Add(b));
        }

        public static T Min<T>(this T a, T b)
            where T : IComparable<T>
        {
            return a.CompareTo(b) < 0 ? a : b;
        }

        public static T Max<T>(this T a, T b)
            where T : IComparable<T>
        {
            return a.CompareTo(b) > 0 ? a : b;
        }
        
        public static int ExtendedEuclidean(this (int a, int m) p, out int x, out int y)
        {
            var (oldR, r) = p;
            var (oldS, s) = (1, 0);
            var (oldT, t) = (0, 1);
            while (r != 0)
            {
                var q = oldR / r;
                (oldR, r) = (r, oldR - q * r);
                (oldS, s) = (s, oldS - q * s);
                (oldT, t) = (t, oldT - q * t);
            }
            x = oldS;
            y = oldT;
            return oldR;
        }

        public static int ModularInverse(this int i, int mod)
        {
            var g = (i, mod).ExtendedEuclidean(out var x, out _);
            if (g != 1) throw new ArithmeticException($"No modular inverse for {i}, {mod}");
            return (x % mod + mod) % mod;
        }
        
        public static long ExtendedEuclidean(this (long a, long m) p, out long x, out long y)
        {
            var (oldR, r) = p;
            var (oldS, s) = (1L, 0L);
            var (oldT, t) = (0L, 1L);
            while (r != 0)
            {
                var q = oldR / r;
                (oldR, r) = (r, oldR - q * r);
                (oldS, s) = (s, oldS - q * s);
                (oldT, t) = (t, oldT - q * t);
            }
            x = oldS;
            y = oldT;
            return oldR;
        }

        public static long ModularInverse(this long i, long mod)
        {
            var g = (i, mod).ExtendedEuclidean(out var x, out _);
            if (g != 1) throw new ArithmeticException($"No modular inverse for {i}, {mod}");
            return (x % mod + mod) % mod;
        }

        /// <summary>
        /// Get the indices of N integers that sum to the given value.
        /// </summary>
        /// <param name="ints">Array of integers.</param>
        /// <param name="n">Number of indices to sum.</param>
        /// <param name="target">Target sum.</param>
        /// <returns>Indices of N integers that sum to the target value.</returns>
        public static int[] NSum(this int[] ints, int n, int target)
        {
            return SequencesIncreasing(n, ints.Length, true)
                .FirstOrDefault(indices => ints.Get(indices).Sum() == target);
        }

        public static T Trace<T>(this T pos, T delta, Func<T, bool> hit)
            where T : IAdd<T>
        {
            while (true)
            {
                pos = pos.Add(delta);
                if (hit(pos)) break;
            }
            return pos;
        }

        // Returns all sequences of a given length from {1,1,...,1} to {n,n,...,n}
        // The array yielded by this method is the same array each time.
        // index = true makes the sequence start from 0 and n is exclusive
        public static IEnumerable<int[]> Sequences(int length, int n, bool index = false)
        {
            if (n < (index ? 0 : 1)) yield break;
            var arr = new int[length];
            Array.Fill(arr, index ? 0 : 1);
            Main:
            yield return arr;
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i] == (index ? n - 1 : n)) arr[i] = index ? 0 : 1;
                else
                {
                    arr[i]++;
                    goto Main;
                }
            }
        }

        // Generates all sequences of a given length
        // from {1, 2, 3, ..., length} to {..., n - 2, n - 1, n}
        // The array yielded by this method is the same array each time.
        // index = true makes the sequence start from 0 and n is exclusive
        public static IEnumerable<int[]> SequencesIncreasing(int length, int n, bool index = false)
        {
            if (n < (index ? length - 1 : length)) yield break;
            var arr = Enumerable.Range(index ? 0 : 1, length).ToArray();
            while (true)
            {
                yield return arr;
                // Increase last
                var i = length - 1;
                arr[i]++;
                if (arr[i] <= (index ? n - 1 : n)) continue;
                // On overflow, find next index from end to increase
                var j = i;
                do
                {
                    j--;
                    if (j < 0) yield break;
                }
                while (arr[j] >= (index ? n - 1 : n) - (length - j - 1));
                // Increment current index and set indexes
                // after to increasing numbers.
                var v = ++arr[j];
                for (var m = 1; m < length - j; m++)
                {
                    arr[j + m] = v + m;
                }
            }
        }

        public static IEnumerable<int[]> ExclusivePairs(int n, bool index = false)
        {
            return Sequences(2, n, index).Where(ints => ints[0] != ints[1]);
        }

        // Find the cycle time for individual components of an object and calculate how often
        // all cycles align.
        public static long CommonCycle<T, TC>(T data, Func<T, IEnumerable<TC>> cycles, Action<T> step)
        {
            var initial = new Dictionary<int, long>();
            var state = new List<TC>();
            foreach (var (i, cycle) in cycles(data).Index())
            {
                initial[i] = -1;
                state.Add(cycle);
            }
            var steps = 0L;
            while (true)
            {
                if (initial.Values.All(cycle => cycle > -1)) break;
                step(data);
                steps++;
                foreach (var (i, cycle) in cycles(data).Index())
                {
                    if (initial[i] == -1 && Equals(cycle, state[i]))
                    {
                        initial[i] = steps;
                    }
                }
            }
            return initial.Values.Aggregate((a, b) => a.Lcm(b));
        }

        // Find the cycle period for something that might not immediately start in the cycle.
        public static (long Offset, long Cycle) FindCyclePeriod<T, TState>(T data, Func<T, TState> state, Action<T> step)
        {
            var states = new Dictionary<TState, long> {[state(data)] = 0};
            var count = 0L;
            while (true)
            {
                step(data);
                count++;
                var s = state(data);
                if (states.TryGetValue(s, out var i)) return (i, count - i);
                states[s] = count;
            }
        }

        // Given an offset and cycle period, find the offset needed to reach i iterations
        public static int CycleOffset(this int i, int offset, int cycle) => i > offset ? offset + (i - offset) % cycle : i;

        public static long CycleOffset(this long l, long offset, long cycle) => l > offset ? offset + (l - offset) % cycle : l;

        public static int ExploreAll<TPos, TVal>(this IExploreDrone<TPos, TVal> drone, Func<TPos, bool> seen)
            where TPos : ISub<TPos>, INegate<TPos>
        {
            var max = 0;
            var path = new Stack<TPos>();
            while (true)
            {
                var neighbors = drone.GetNeighbors().Where(pos => !seen(pos)).Select(pos => pos.Sub(drone.Position)).ToList();
                if (neighbors.Count == 0 || !neighbors.First(drone.TryMove, out var offset))
                {
                    if (path.Count == 0) return max;
                    drone.TryMove(path.Pop().Negate());
                    continue;
                }
                path.Push(offset);
                max = Math.Max(max, path.Count);
            }
        }

        public static IEnumerable<TPos> Bfs<TPos, TVal>(this AlignedSpace<TPos, TVal> space, TPos from, Func<TPos, bool> valid, Func<TPos, bool> notify, bool includeInvalid = false)
        {
            var visited = new HashSet<TPos>();
            var queue = new Queue<TPos>();
            queue.Enqueue(from);
            visited.Add(from);
            while (queue.Count > 0)
            {
                var pos = queue.Dequeue();
                var near = space.GetNeighbors(pos).Where(p => !visited.Contains(p));
                foreach (var next in near)
                {
                    var v = valid(next);
                    if ((v || includeInvalid) && notify(next)) yield return next;
                    if (!v) continue;
                    queue.Enqueue(next);
                    visited.Add(next);
                }
            }
        }

        public static int ShortestPathBfs<TPos, TVal>(this AlignedSpace<TPos, TVal> space, TPos from, TPos to, Func<TPos, bool> valid)
        {
            var visited = new HashSet<TPos>();
            var queue = new Queue<(TPos pos, int dist)>();
            queue.Enqueue((from, 0));
            visited.Add(from);
            while (queue.Count > 0)
            {
                var (pos, dist) = queue.Dequeue();
                if (Equals(pos, to)) return dist;
                var near = space.GetNeighbors(pos)
                    .Where(valid)
                    .Where(p => !visited.Contains(p))
                    .Select(p => (p, dist + 1));
                foreach (var next in near)
                {
                    queue.Enqueue(next);
                    visited.Add(next.p);
                }
            }
            return -1;
        }

        public static int LongestPathBfs<TPos, TVal>(this AlignedSpace<TPos, TVal> space, TPos from, Func<TPos, bool> valid)
        {
            var visited = new HashSet<TPos>();
            var queue = new Queue<(TPos pos, int dist)>();
            queue.Enqueue((from, 0));
            visited.Add(from);
            var max = 0;
            while (queue.Count > 0)
            {
                var (pos, dist) = queue.Dequeue();
                max = Math.Max(max, dist);
                var near = space.GetNeighbors(pos)
                    .Where(valid)
                    .Where(p => !visited.Contains(p))
                    .Select(p => (p, dist + 1));
                foreach (var next in near)
                {
                    queue.Enqueue(next);
                    visited.Add(next.p);
                }
            }
            return max;
        }

        public static bool IsReachable<TPos, TVal>(this AlignedSpace<TPos, TVal> space, TPos from, TPos target, Func<TPos, bool> valid)
        {
            return space.ShortestPathBfs(from, target, valid) > -1;
        }

        public static IEnumerable<T> LongestRepeatedSequence<T>(this IEnumerable<T> source, int max = -1)
        {
            var list = source.ToList();
            var table = new Grid<int>();
            for (var i = 1; i <= list.Count; i++)
            {
                for (var j = i + 1; j < list.Count; j++)
                {
                    if (Equals(list[i - 1], list[j - 1]) && table[i - 1, j - 1] < (j - i))
                    {
                        table[i, j] = table[i - 1, j - 1] + 1;
                    }
                    else table[i, j] = 0;
                }
            }
            
            var ((a, _), len) = table.Where(pair => max <= 0 || pair.Value <= max).OrderByDescending(pair => pair.Value).FirstOrDefault();
            if (len > 0) return list.GetRange(a - len, len);
            return Enumerable.Empty<T>();
        }
        
        public static IEnumerable<T> LongestRepeatFromStart<T>(this IEnumerable<T> source, int max = -1)
        {
            var list = source.ToList();
            var table = new Grid<int>();
            for (var i = 1; i <= list.Count; i++)
            {
                for (var j = i + 1; j < list.Count; j++)
                {
                    if (Equals(list[i - 1], list[j - 1]) && table[i - 1, j - 1] < (j - i))
                    {
                        table[i, j] = table[i - 1, j - 1] + 1;
                    }
                    else table[i, j] = 0;
                }
            }
            
            var (_, len) = table.Where(pair => max <= 0 || pair.Value <= max)
                .Where(pair => pair.Key.X - pair.Value == 0)
                .OrderByDescending(pair => pair.Value)
                .FirstOrDefault();
            if (len > 0) return list.GetRange(0, len);
            return Enumerable.Empty<T>();
        }

        public static IEnumerable<TO> GetSequenceOrder<T, TO>(this IEnumerable<T> source, List<T>[] sequences, IEnumerable<TO> symbols)
        {
            var outputs = symbols.ToList();
            if (outputs.Count < sequences.Length) throw new Exception("Not enough output symbols.");
            var len = sequences.Max(list => list.Count);
            var main = new List<T>();
            foreach (var item in source)
            {
                if (main.Count < len) main.Add(item);
                if (main.Count < len) continue;
                foreach (var (index, sequence) in sequences.Index())
                {
                    for (var i = 1; i <= sequence.Count; i++)
                    {
                        if (!Equals(main[sequence.Count - i], sequence[^i])) goto TryNext;
                    }
                    yield return outputs[index];
                    main.RemoveRange(0, sequence.Count);
                    goto Advance;
                    TryNext: ;
                }
                main.RemoveAt(0);
                continue;
                Advance: ;
            }
            if (main.Count > 0)
            {
                foreach (var (index, sequence) in sequences.Index())
                {
                    if (main.Count < sequence.Count) continue;
                    for (var i = 1; i <= sequence.Count; i++)
                    {
                        if (!Equals(main[sequence.Count - i], sequence[^i])) goto TryNext;
                    }
                    yield return outputs[index];
                    main.RemoveRange(0, sequence.Count);
                    break;
                    TryNext: ;
                }
            }
        }

        public static int FirstDifference<T>(this IEnumerable<T> source, IEnumerable<T> other)
        {
            return source.ZipShortest(other, (a, b) => Equals(a, b)).TakeWhile(b => b).Count();
        }

        public static IEnumerable<(T, T)> Pairs<T>(this IList<T> source)
        {
            return SequencesIncreasing(2, source.Count, true).Select(pair => (source[pair[0]], source[pair[1]]));
        }
    }
}