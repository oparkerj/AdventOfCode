namespace AdventToolkit.Utilities.Computer.Core;

public interface INode<out T>
{
    T Key { get; }
}