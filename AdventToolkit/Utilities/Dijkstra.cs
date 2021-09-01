using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Utilities
{
    public class Dijkstra<TCell, TMid>
    {
        public Func<TCell, IEnumerable<TMid>> Neighbors;
        public Func<TMid, int> Distance;
        public Func<TCell, TMid, TCell> Cell;

        public Dictionary<TCell, int> ComputeAll(TCell start, IEnumerable<TCell> all)
        {
            var dist = new Dictionary<TCell, int>();
            foreach (var cell in all)
            {
                dist[cell] = int.MaxValue;
            }
            dist[start] = 0;
            var queue = new PriorityQueue<TCell>(dist.Select(pair => (pair.Key, pair.Value)));
            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();
                var current = dist[cell];
                foreach (var neighbor in Neighbors(cell))
                {
                    var other = Cell(cell, neighbor);
                    var weight = Distance(neighbor);
                    if (current + weight < dist[other])
                    {
                        dist[other] = current + weight;
                    }
                }
            }
            return dist;
        }

        public Dictionary<TCell, int> Compute(TCell start)
        {
            var dist = new DefaultDict<TCell, int> {DefaultValue = int.MaxValue, [start] = 0};
            var seen = new HashSet<TCell>();
            var queue = new PriorityQueue<TCell>();
            queue.Enqueue(start, 0);
            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();
                if (seen.Contains(cell)) continue;
                seen.Add(cell);
                var current = dist[cell];
                foreach (var neighbor in Neighbors(cell))
                {
                    var other = Cell(cell, neighbor);
                    var weight = Distance(neighbor);
                    if (!seen.Contains(other)) queue.Enqueue(other, current + weight);
                    if (current + weight < dist[other])
                    {
                        dist[other] = current + weight;
                    }
                }
            }
            return dist;
        }

        public Dictionary<TCell, int> ComputeWhere(TCell start, Func<TCell, bool> valid)
        {
            var dist = new DefaultDict<TCell, int> {DefaultValue = int.MaxValue, [start] = 0};
            var seen = new HashSet<TCell>();
            var queue = new PriorityQueue<TCell>();
            queue.Enqueue(start, 0);
            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();
                if (seen.Contains(cell)) continue;
                seen.Add(cell);
                var current = dist[cell];
                foreach (var neighbor in Neighbors(cell))
                {
                    var other = Cell(cell, neighbor);
                    if (!valid(other)) continue;
                    var weight = Distance(neighbor);
                    if (!seen.Contains(other)) queue.Enqueue(other, current + weight);
                    if (current + weight < dist[other])
                    {
                        dist[other] = current + weight;
                    }
                }
            }
            return dist;
        }

        public Dictionary<TCell, (int Dist, TCell From)> ComputeFrom(TCell start, Func<TCell, bool> valid = null)
        {
            valid ??= _ => true;
            var dist = new DefaultDict<TCell, (int Dist, TCell From)> {DefaultValue = (int.MaxValue, default), [start] = (0, default)};
            var seen = new HashSet<TCell>();
            var queue = new PriorityQueue<TCell>();
            queue.Enqueue(start, 0);
            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();
                if (seen.Contains(cell)) continue;
                seen.Add(cell);
                var current = dist[cell].Dist;
                foreach (var neighbor in Neighbors(cell))
                {
                    var other = Cell(cell, neighbor);
                    if (!valid(other)) continue;
                    var weight = Distance(neighbor);
                    if (!seen.Contains(other)) queue.Enqueue(other, current + weight);
                    if (current + weight < dist[other].Dist)
                    {
                        dist[other] = (current + weight, cell);
                    }
                }
            }
            return dist;
        }

        public int ComputeFind(TCell start, TCell target, Func<TCell, bool> valid = null)
        {
            valid ??= _ => true; 
            var dist = new DefaultDict<TCell, int> {DefaultValue = int.MaxValue, [start] = 0};
            var seen = new HashSet<TCell>();
            var queue = new PriorityQueue<TCell>();
            queue.Enqueue(start, 0);
            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();
                if (seen.Contains(cell)) continue;
                var current = dist[cell];
                if (Equals(cell, target)) return current;
                seen.Add(cell);
                foreach (var neighbor in Neighbors(cell))
                {
                    var other = Cell(cell, neighbor);
                    if (!valid(other)) continue;
                    var weight = Distance(neighbor);
                    if (!seen.Contains(other)) queue.Enqueue(other, current + weight);
                    if (current + weight < dist[other])
                    {
                        dist[other] = current + weight;
                    }
                }
            }
            return -1;
        }
    }

    public interface IDijkstra<TCell, TMid>
    {
        IEnumerable<TMid> GetNeighbors(TCell cell);

        int GetWeight(TMid mid);

        TCell GetNeighbor(TCell from, TMid mid);
    }

    public static class DijkstraExtensions
    {
        private static Dijkstra<TCell, TMid> Build<TCell, TMid>(this IDijkstra<TCell, TMid> dijkstra)
        {
            return new()
            {
                Neighbors = dijkstra.GetNeighbors,
                Distance = dijkstra.GetWeight,
                Cell = dijkstra.GetNeighbor
            };
        }

        public static Dijkstra<TCell, TMid> ToDijkstra<TCell, TMid>(this IDijkstra<TCell, TMid> dijkstra)
        {
            return dijkstra.Build();
        }

        public static Dictionary<TCell, int> DijkstraAll<TCell, TMid>(this IDijkstra<TCell, TMid> dijkstra, TCell start, IEnumerable<TCell> all)
        {
            return dijkstra.Build().ComputeAll(start, all);
        }
        
        public static Dictionary<TCell, int> Dijkstra<TCell, TMid>(this IDijkstra<TCell, TMid> dijkstra, TCell start)
        {
            return dijkstra.Build().Compute(start);
        }

        public static Dictionary<TCell, int> DijkstraWhere<TCell, TMid>(this IDijkstra<TCell, TMid> dijkstra, TCell start, Func<TCell, bool> valid)
        {
            return dijkstra.Build().ComputeWhere(start, valid);
        }

        public static Dictionary<TCell, (int Dist, TCell From)> DijkstraFrom<TCell, TMid>(this IDijkstra<TCell, TMid> dijkstra, TCell start, Func<TCell, bool> valid = null)
        {
            return dijkstra.Build().ComputeFrom(start, valid);
        }

        public static int DijkstraFind<TCell, TMid>(this IDijkstra<TCell, TMid> dijkstra, TCell start, TCell target, Func<TCell, bool> valid = null)
        {
            return dijkstra.Build().ComputeFind(start, target, valid);
        }

        public static IReadOnlyCollection<TCell> GetPathTo<TCell>(this Dictionary<TCell, (int Dist, TCell From)> dist, TCell target)
        {
            var list = new LinkedList<TCell>();
            list.AddFirst(target);
            while (true)
            {
                if (!dist.TryGetValue(target, out var pair)) return Array.Empty<TCell>();
                if (pair.Dist == 0) break;
                list.AddFirst(target = pair.From);
            }
            return list;
        }
    }
}