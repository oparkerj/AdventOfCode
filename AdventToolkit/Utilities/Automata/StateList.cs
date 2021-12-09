using System.Collections.Generic;

namespace AdventToolkit.Utilities.Automata;

public class StateList<T> : List<T>
{
    private readonly Stack<int> _states = new();

    public void Save()
    {
        _states.Push(Count);
    }

    public void Release()
    {
        _states.Pop();
    }

    public void Restore()
    {
        var size = _states.Pop();
        var remove = Count - size;
        if (remove == 0) return;
        RemoveRange(size, remove);
    }
}