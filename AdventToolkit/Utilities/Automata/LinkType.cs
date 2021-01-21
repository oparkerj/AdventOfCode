using System;

namespace AdventToolkit.Utilities.Automata
{
    [Flags]
    public enum LinkType
    {
        Default = Greedy,
        Greedy = 1 << 0,
        Lazy = 1 << 1,
    }
}