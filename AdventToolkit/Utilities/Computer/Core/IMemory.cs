namespace AdventToolkit.Utilities.Computer.Core;

// Represents the memory of the cpu
public interface IMemory<T>
{
    void Reset();
    
    T this[T t] { get; set; }
    
    T this[int i] { get; set; }
    
    T this[char c] { get; set; }
    
    T this[long l] { get; set; }
    
    T this[string s] { get; set; }
}