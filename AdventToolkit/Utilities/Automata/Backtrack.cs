using System.Collections.Generic;

namespace AdventToolkit.Utilities.Automata;

public class Backtrack<T>
{
    public readonly Stack<MatchPoint<T>> Next = new();
    public readonly Stack<int> Groups = new();
    public readonly StateList<(int state, int index)> Path;

    public Backtrack(StateList<(int state, int index)> path)
    {
        Path = path;
    }

    public bool HasNext => Next.Count > 0;
}