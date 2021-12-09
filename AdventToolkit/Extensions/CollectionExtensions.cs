using System;
using System.Collections.Generic;

namespace AdventToolkit.Extensions;

public static class CollectionExtensions
{
    public static void Replace<T>(this Stack<T> stack, T t)
    {
        if (!stack.TryPop(out _)) throw new Exception("Stack is empty.");
        stack.Push(t);
    }
}