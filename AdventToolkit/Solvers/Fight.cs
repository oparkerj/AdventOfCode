using System;
using AdventToolkit.Collections.Space;

namespace AdventToolkit.Solvers
{
    public class Fight<TUnit, TPos, TCell>
    {
        private SparseSpace<TPos, TCell> _map;
        private FreeSpace<TPos, TUnit> _units = new();

        public Fight(SparseSpace<TPos, TCell> map)
        {
            _map = map;
        }

        public Fight(Func<SparseSpace<TPos, TCell>> cons) : this(cons()) { }
    }
}