using System;
using System.Collections.Generic;
using AdventToolkit.Collections.Space;

namespace AdventToolkit.Solvers
{
    public abstract class Fight<TUnit>
    {
        public readonly List<TUnit> Units = new();

        public abstract bool Tick();

        public void RunBattle()
        {
            while (true)
            {
                if (!Tick()) break;
            }
        }
    }
    
    public abstract class Fight<TUnit, TPos, TCell> : Fight<TUnit>
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