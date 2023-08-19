namespace AdventToolkit.New.Extensions;

public static class SpanExtensions
{
    public ref struct SpanSplitCharEnumerator
    {
        private ReadOnlySpan<char> _head;
        private ReadOnlySpan<char> _tail;
        private char _search;

        public SpanSplitCharEnumerator(ReadOnlySpan<char> span, char search)
        {
            _head = default;
            _tail = span;
            _search = search;
        }

        public ReadOnlySpan<char> Current => _head;

        public SpanSplitCharEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            if (_tail.Length == 0) return false;

            var next = _tail.IndexOf(_search);
            if (next >= 0)
            {
                _head = _tail[..next];
                _tail = _tail[(next + 1)..];
            }
            else
            {
                _head = _tail;
                _tail = default;
            }

            return true;
        }
    }

    public static SpanSplitCharEnumerator EnumerateSplit(this ReadOnlySpan<char> span, char c) => new(span, c);
}