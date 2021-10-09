using System.Collections.Generic;

namespace AdventToolkit.Utilities
{
    // Something that has a position and neighboring positions
    public interface IDrone<out TPos>
    {
        TPos Position { get; }

        IEnumerable<TPos> GetNeighbors();
    }

    // A drone which can move and report what it finds at the new location
    public interface IExploreDrone<TPos, TSense> : IDrone<TPos>
    {
        bool TryMove(TPos offset, out TSense sense);

        bool TryMove(TPos offset) => TryMove(offset, out _);
    }
}