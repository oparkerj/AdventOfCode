namespace AdventToolkit.Utilities
{
    public readonly struct IntRange
    {
        public readonly int Start;
        public readonly int End;

        public IntRange(int start, int end)
        {
            Start = start;
            End = end;
        }

        public bool Contains(int i) => i >= Start && i < End;

        public bool ContainsInclusive(int i) => i >= Start && i <= End;
    }
}