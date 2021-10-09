using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Extensions
{
    public static class ListExtensions
    {
        public static IEnumerable<T> Get<T>(this IList<T> t, IEnumerable<int> indices)
        {
            return indices.Select(i => t[i]);
        }

        public static IEnumerable<(T, T)> Pairs<T>(this IList<T> source)
        {
            return Algorithms.SequencesIncreasing(2, source.Count, true).Select(pair => (source[pair[0]], source[pair[1]]));
        }

        public static LinkedListNode<T> NextCircular<T>(this LinkedListNode<T> node)
        {
            return node.Next ?? node.List?.First;
        }

        public static LinkedListNode<T> PreviousCircular<T>(this LinkedListNode<T> node)
        {
            return node.Previous ?? node.List?.Last;
        }
        
        // Removes elements that meet a condition while a list is being iterated; adjusts the
        // iteration index so that the loop will continue in the right spot after elements are removed.
        // Returns the number of elements removed.
        public static int RemoveConcurrent<T>(this List<T> list, Func<T, bool> cond, ref int index)
        {
            var removed = 0;
            for (var i = 0; i < list.Count; i++)
            {
                if (cond(list[i]))
                {
                    removed++;
                    if (i <= index) index--;
                    list.RemoveAt(i--);
                }
            }
            return removed;
        }
    }
}