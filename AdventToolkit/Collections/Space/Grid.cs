using System.Collections.Generic;
using AdventToolkit.Common;

namespace AdventToolkit.Collections.Space
{
    public class Grid<T> : GridBase<T>
    {
        private readonly FreeSpace<Pos, T> _data = new();

        public Grid() { }
        
        public Grid(bool includeCorners = false) : base(includeCorners) { }
        
        public Grid(GridBase<T> other) : base(other) { }

        public override int Count => _data.Count;

        public override bool TryGet(Pos pos, out T value)
        {
            return _data.TryGet(pos, out value);
        }

        public override void Add(Pos pos, T val)
        {
            _data.Add(pos, val);
        }

        public override bool Remove(Pos pos)
        {
            return _data.Remove(pos);
        }

        public override bool Has(Pos pos)
        {
            return _data.Has(pos);
        }

        public override bool HasValue(T val)
        {
            return _data.HasValue(val);
        }

        public override void Clear()
        {
            _data.Clear();
        }

        public override IEnumerable<Pos> Positions => _data.Positions;

        public override IEnumerable<T> Values => _data.Values;

        public override IEnumerator<KeyValuePair<Pos, T>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }
    }
}