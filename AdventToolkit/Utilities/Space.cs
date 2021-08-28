using System;
using System.Collections;
using System.Collections.Generic;

namespace AdventToolkit.Utilities
{
    public abstract class AlignedSpace<TPos, TVal> : IEnumerable<KeyValuePair<TPos, TVal>>, IDijkstra<TPos, TPos>
    {
        public readonly Dictionary<TPos, TVal> Points = new();

        public bool UseDefault = true;
        public TVal Default = default;

        public virtual TVal this[TPos pos, bool sideEffects = true]
        {
            get => Points.TryGetValue(pos, out var val) ? val : UseDefault ? Default : throw new Exception("Point doesn't exist.");
            set => Points[pos] = value;
        }

        public abstract IEnumerable<TPos> GetNeighbors(TPos pos);

        public int Count => Points.Count;

        public bool Lookup(TPos pos, out TVal val) => Points.TryGetValue(pos, out val);

        public bool Has(TPos pos) => Points.ContainsKey(pos);

        public bool HasValue(TVal val) => Points.ContainsValue(val);

        public bool Remove(TPos pos) => Points.Remove(pos);

        public void Clear() => Points.Clear();

        public virtual int GetWeight(TPos mid) => 1;

        public TPos GetNeighbor(TPos from, TPos mid) => mid;

        public IEnumerable<TPos> Positions => Points.Keys;

        public IEnumerable<TVal> Values => Points.Values;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual IEnumerator<KeyValuePair<TPos, TVal>> GetEnumerator()
        {
            return Points.GetEnumerator();
        }
    }

    public class CopySpace<TPos, TVal> : AlignedSpace<TPos, TVal>
    {
        private readonly Func<TPos, IEnumerable<TPos>> _neighbors;

        public CopySpace(Func<TPos, IEnumerable<TPos>> neighbors)
        {
            _neighbors = neighbors;
        }

        public CopySpace(AlignedSpace<TPos, TVal> reference)
        {
            _neighbors = reference.GetNeighbors;
        }

        public override IEnumerable<TPos> GetNeighbors(TPos pos) => _neighbors(pos);
    }

    public class FreeSpace<TPos, TVal> : AlignedSpace<TPos, TVal>
    {
        public override IEnumerable<TPos> GetNeighbors(TPos pos)
        {
            throw new NotSupportedException();
        }
    }
}