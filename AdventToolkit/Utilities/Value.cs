namespace AdventToolkit.Utilities
{
    public class Value<T>
    {
        public T Data;

        public Value() { }

        public Value(T data) => Data = data;

        public static implicit operator T(Value<T> value) => value.Data;

        public override string ToString() => Data?.ToString() ?? "null";
    }

    public static class ValueExtensions
    {
        public static Value<T> Capture<T>(this T value) => new(value);

        public static Value<int> Increment(this Value<int> value)
        {
            value.Data++;
            return value;
        }

        // True the first time cond is false after it has been true once
        public static bool After(this Value<bool> value, bool cond)
        {
            if (value.Data && !cond) return true;
            value.Data |= cond;
            return false;
        }
    }
}