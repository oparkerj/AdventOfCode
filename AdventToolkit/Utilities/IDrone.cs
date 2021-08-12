using System.Collections.Generic;

namespace AdventToolkit.Utilities
{
    public interface IDrone<out TPos>
    {
        TPos Position { get; }

        IEnumerable<TPos> GetNeighbors();
    }

    public interface IExploreDrone<TPos, TSense> : IDrone<TPos>
    {
        bool TryMove(TPos offset, out TSense sense);

        bool TryMove(TPos offset) => TryMove(offset, out _);
    }
}