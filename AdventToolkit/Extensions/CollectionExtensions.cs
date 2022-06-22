using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Extensions;

public static class CollectionExtensions
{
    public static void Replace<T>(this Stack<T> stack, T t)
    {
        if (!stack.TryPop(out _)) throw new Exception("Stack is empty.");
        stack.Push(t);
    }

    // IMPORTANT: This only works when the collection does not have duplicates.
    public static bool ContentEquals<T>(this ICollection<T> a, ICollection<T> b)
    {
        return a.Count == b.Count && !a.Except(b).Any();
    }
}