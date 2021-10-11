using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventToolkit.Collections.Space
{
    // Fixed size grid backed by a 2d array.
    // The Bounds for this grid acts as the window where you can operate.
    // Only elements within the window are considered.
    // Parts of the window outside the backing array also are not considered.
    // This grid sets FitBounds to false by default, so writing outside the window will have no effect.
    // The position 0,0 is relative to the window.
    //
    // null window will cause exceptions. (Could possibly be made so null window = full array)
    public class FixedGrid<T> : GridBase<T>
    {
        public readonly T[,] Data;
        private readonly BitArray _has;
        private readonly int[] _temp;

        private readonly Rect _fullWindow;

        private FixedGrid(int realSize, bool includeCorners) : base(includeCorners)
        {
            _has = new BitArray(realSize);
            _temp = new int[(realSize >> 5) + 1];
            FitBounds = false;
        }

        public FixedGrid(int width, int height, bool includeCorners = false) : this(width * height, includeCorners)
        {
            Data = new T[width, height];
            Bounds = new Rect(width, height);
            _fullWindow = new Rect(Bounds);
        }

        // Can be used to wrap a slice of a 2d array.
        // Offset will be set so that 0,0 is the corner of the window.
        // Not specifying the window will wrap the entire array.
        public FixedGrid(T[,] data, Rect window = null, bool includeCorners = false) : this(GetWindowSize(data, ref window), includeCorners)
        {
            Data = data;
            _has.SetAll(true);
            Bounds = window;
            _fullWindow = new Rect(data.GetLength(0), data.GetLength(1));
        }

        public FixedGrid(FixedGrid<T> other) : this(other.Data.Length, other.IncludeCorners)
        {
            CopyFrom(other);
        }

        private static int GetWindowSize(T[,] data, ref Rect rect)
        {
            rect ??= new Rect(data.GetLength(0), data.GetLength(1));
            return rect.Area;
        }

        private int BitIndex(Pos p) => p.X + p.Y * Data.GetLength(0);

        private Pos BitPos(int index) => new(index % Data.GetLength(0), index / Data.GetLength(0));

        public Pos RealPosition(Pos p) => p + Bounds.Min;

        protected bool InBounds(Pos real)
        {
            return Bounds.Contains(real) && Data.Has(real);
        }

        public override bool TryGet(Pos pos, out T value)
        {
            var data = Data;
            pos = RealPosition(pos);
            if (InBounds(pos) && _has[BitIndex(pos)])
            {
                value = data.Get(pos);
                return true;
            }
            value = default;
            return false;
        }

        public override void Add(Pos pos, T val)
        {
            pos = RealPosition(pos);
            if (!InBounds(pos)) return;
            Data.Set(pos, val);
            _has[BitIndex(pos)] = true;
        }

        public override bool Remove(Pos pos)
        {
            var data = Data;
            pos = RealPosition(pos);
            var bit = BitIndex(pos);
            if (!InBounds(pos) || !_has[bit]) return false;
            data.Set(pos, default);
            _has[bit] = false;
            return true;
        }

        public override bool Has(Pos pos)
        {
            var real = RealPosition(pos);
            return InBounds(real) && _has[BitIndex(real)];
        }

        // Marks the window as having no elements without modifying the array
        public void SoftClear()
        {
            foreach (var pos in InWindow)
            {
                _has[BitIndex(pos)] = false;
            }
        }

        // Clear the contents of the array within the window
        public override void Clear()
        {
            foreach (var pos in InWindow)
            {
                Data.Set(pos, default);
                _has[BitIndex(pos)] = false;
            }
        }

        public IEnumerable<Pos> InWindow => Bounds.Intersection(_fullWindow);

        public override IEnumerable<Pos> Positions => InWindow.Where(pos => _has[BitIndex(pos)]);

        public override int Count
        {
            get
            {
                // Count active bits in BitArray
                var bits = _temp;
                _has.CopyTo(bits, 0);
                // Cut off extra bits
                bits[^1] &= ~(-1 << (_has.Count % 32));
                return bits.Unsigned().Select(BitOperations.PopCount).Sum();
            }
        }

        public override IEnumerator<KeyValuePair<Pos, T>> GetEnumerator()
        {
            var data = Data;
            return Positions.Select(pos => new KeyValuePair<Pos, T>(pos, data.Get(pos))).GetEnumerator();
        }
    }
}