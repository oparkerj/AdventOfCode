namespace AdventToolkit.Utilities.Computer;

public interface INode<out T>
{
    T Key { get; }
}