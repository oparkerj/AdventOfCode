namespace AdventToolkit.Utilities.Computer;

// Represents the memory of the cpu
public interface IMemory<T>
{
    T this[T t] { get; set; }
    
    T this[int i] { get; set; }
    
    T this[char c] { get; set; }
    
    T this[long l] { get; set; }
    
    T this[string s] { get; set; }
}