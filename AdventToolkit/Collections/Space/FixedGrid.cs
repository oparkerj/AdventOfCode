using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventToolkit.Collections.Space
{
    public class FixedGrid<T> : GridBase<T>
    {
        public readonly T[,] Data;
        private readonly BitArray _has;
        private readonly int[] _temp;
        
        // Offset needed to shift positions to 0-indexed positions.
        public Pos Offset { get; set; }

        private FixedGrid(int realSize, bool includeCorners) : base(includeCorners)
        {
            _has = new BitArray(realSize);
            _temp = new int[(realSize >> 5) + 1];
        }

        public FixedGrid(int width, int height, bool includeCorners = false) : this(width * height, includeCorners)
        {
            Data = new T[width, height];
        }

        public FixedGrid(T[,] data, bool includeCorners = false) : this(data.Length, includeCorners)
        {
            Data = data;
            _has.SetAll(true);
        }

        public FixedGrid(FixedGrid<T> other) : this(other.Data.Length, other.IncludeCorners)
        {
            CopyFrom(other);
        }

        private int BitIndex(Pos p) => p.X + p.Y * Data.GetLength(0);

        private Pos BitPos(int index) => new(index % Data.GetLength(0), index / Data.GetLength(0));

        public Pos RealPosition(Pos p) => p + Offset;
        
        // Not sure if this should be affected by the offset
        public T this[int x, int y]
        {
            get => Data[x, y];
            set => Data[x, y] = value;
        }

        public override bool TryGet(Pos pos, out T value)
        {
            var data = Data;
            pos = RealPosition(pos);
            if (data.Has(pos) && _has[BitIndex(pos)])
            {
                value = data.Get(pos);
                return true;
            }
            value = default;
            return false;
        }

        public override void Add(Pos pos, T val)
        {
            var data = Data;
            pos = RealPosition(pos);
            if (!data.Has(pos)) return;
            Data.Set(pos, val);
            _has[BitIndex(pos)] = true;
        }

        public override bool Remove(Pos pos)
        {
            var data = Data;
            pos = RealPosition(pos);
            var bit = BitIndex(pos);
            if (!data.Has(pos) || !_has[bit]) return false;
            data.Set(pos, default);
            _has[bit] = false;
            return true;
        }

        public override bool Has(Pos pos) => Data.Has(RealPosition(pos));

        public override void Clear()
        {
            Array.Clear(Data, 0, Data.Length);
            _has.SetAll(false);
        }

        public override IEnumerable<Pos> Positions
        {
            get
            {
                return _has.Cast<bool>()
                    .Index()
                    .WhereValue(true)
                    .Select(pair => BitPos(pair.Key));
            }
        }

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