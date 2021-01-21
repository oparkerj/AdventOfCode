namespace AdventToolkit.Utilities.Automata
{
    public class MatchPoint<T>
    {
        public readonly int State;
        public readonly Link<T> Link;
        public readonly int Index;
        public readonly Backtrack<T> Backtrack;

        public MatchPoint(int state, Link<T> link, int index, Backtrack<T> backtrack = default)
        {
            State = state;
            Link = link;
            Index = index;
            Backtrack = backtrack;
        }

        public MatchPoint<T> WithBacktrack(Backtrack<T> backtrack)
        {
            return new(State, Link, Index, backtrack);
        }
    }
}