using System.Collections.Generic;

namespace AdventToolkit.Extensions;

public static class QueueExtensions
{
    public static void Add<T>(this Queue<T> queue, T item) => queue.Enqueue(item);

    public static void AddRange<T>(this Queue<T> queue, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            queue.Enqueue(item);
        }
    }
}