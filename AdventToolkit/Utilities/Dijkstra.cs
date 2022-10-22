using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections;
using AdventToolkit.Collections.Space;
using Dist = System.ValueTuple<int, int>;

namespace AdventToolkit.Utilities;

public class Dijkstra<TCell, TMid>
{
    public Func<TCell, IEnumerable<TMid>> Neighbors;
    public Func<TMid, int> Distance;
    public Func<TCell, TMid, TCell> Cell;
    // Setting a heuristic will cause the pathfinder to behave like A*
    public Func<TCell, int> Heuristic = _ => 0;

    public Dictionary<TCell, int> ComputeAll(TCell start, IEnumerable<TCell> all)
    {
        var dist = new Dictionary<TCell, int>();
        foreach (var cell in all)
        {
            dist[cell] = int.MaxValue;
        }
        dist[start] = 0;
        var queue = new PriorityQueue<TCell, int>(dist.Select(pair => (pair.Key, pair.Value)));
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
        var queue = new PriorityQueue<TCell, Dist>();
        queue.Enqueue(start, new Dist(Heuristic(start), 0));
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
                var newScore = current + weight;
                if (!seen.Contains(other)) queue.Enqueue(other, new Dist(newScore + Heuristic(other), newScore));
                if (newScore < dist[other])
                {
                    dist[other] = newScore;
                }
            }
        }
        return dist;
    }
    
    public Dictionary<TCell, int> Compute(TCell start, Func<TCell, bool> valid)
    {
        var dist = new DefaultDict<TCell, int> {DefaultValue = int.MaxValue, [start] = 0};
        var seen = new HashSet<TCell>();
        var queue = new PriorityQueue<TCell, Dist>();
        queue.Enqueue(start, new Dist(Heuristic(start), 0));
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
                var newScore = current + weight;
                if (!seen.Contains(other)) queue.Enqueue(other, new Dist(newScore + Heuristic(other), newScore));
                if (newScore < dist[other])
                {
                    dist[other] = newScore;
                }
            }
        }
        return dist;
    }

    public int Count(TCell start, Func<TCell, bool> count)
    {
        var total = 0;
        var seen = new HashSet<TCell>();
        var queue = new Queue<TCell>();
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            var cell = queue.Dequeue();
            if (seen.Contains(cell)) continue;
            seen.Add(cell);
            foreach (var neighbor in Neighbors(cell))
            {
                var other = Cell(cell, neighbor);
                if (count(other))
                {
                    total++;
                    continue;
                }
                if (!seen.Contains(other)) queue.Enqueue(other);
            }
        }
        return total;
    }

    public Dictionary<TCell, int> ComputeWhere(TCell start, Func<TCell, bool> valid)
    {
        var dist = new DefaultDict<TCell, int> {DefaultValue = int.MaxValue, [start] = 0};
        var seen = new HashSet<TCell>();
        var queue = new PriorityQueue<TCell, Dist>();
        queue.Enqueue(start, new Dist(Heuristic(start), 0));
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
                var newScore = current + weight;
                if (!seen.Contains(other)) queue.Enqueue(other, new Dist(newScore + Heuristic(other), newScore));
                if (newScore < dist[other])
                {
                    dist[other] = newScore;
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
        var queue = new PriorityQueue<TCell, Dist>();
        queue.Enqueue(start, new Dist(Heuristic(start), 0));
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
                var newScore = current + weight;
                if (!seen.Contains(other)) queue.Enqueue(other, new Dist(newScore + Heuristic(other), newScore));
                if (newScore < dist[other].Dist)
                {
                    dist[other] = (newScore, cell);
                }
            }
        }
        return dist;
    }
    
    public Dictionary<TCell, (int Dist, TCell From)> ComputeFrom(TCell start, Func<TCell, int, bool> valid = null)
    {
        valid ??= (_, _) => true;
        var dist = new DefaultDict<TCell, (int Dist, TCell From)> {DefaultValue = (int.MaxValue, default), [start] = (0, default)};
        var seen = new HashSet<TCell>();
        var queue = new PriorityQueue<TCell, Dist>();
        queue.Enqueue(start, new Dist(Heuristic(start), 0));
        while (queue.Count > 0)
        {
            var cell = queue.Dequeue();
            if (seen.Contains(cell)) continue;
            seen.Add(cell);
            var current = dist[cell].Dist;
            foreach (var neighbor in Neighbors(cell))
            {
                var other = Cell(cell, neighbor);
                var weight = Distance(neighbor);
                var newScore = current + weight;
                if (!valid(other, newScore)) continue;
                if (!seen.Contains(other)) queue.Enqueue(other, new Dist(newScore + Heuristic(other), newScore));
                if (newScore < dist[other].Dist)
                {
                    dist[other] = (newScore, cell);
                }
            }
        }
        return dist;
    }

    public Dictionary<TCell, (int Dist, TCell From)> ComputePath(TCell start, TCell target, Func<TCell, bool> valid = null)
    {
        valid ??= _ => true;
        var dist = new DefaultDict<TCell, (int Dist, TCell From)> {DefaultValue = (int.MaxValue, default), [start] = (0, default)};
        var seen = new HashSet<TCell>();
        var queue = new PriorityQueue<TCell, Dist>();
        queue.Enqueue(start, new Dist(Heuristic(start), 0));
        while (queue.Count > 0)
        {
            var cell = queue.Dequeue();
            if (seen.Contains(cell)) continue;
            seen.Add(cell);
            if (Equals(cell, target)) return dist;
            var current = dist[cell].Dist;
            foreach (var neighbor in Neighbors(cell))
            {
                var other = Cell(cell, neighbor);
                if (!valid(other)) continue;
                var weight = Distance(neighbor);
                var newScore = current + weight;
                if (!seen.Contains(other)) queue.Enqueue(other, new Dist(newScore + Heuristic(other), newScore));
                if (newScore < dist[other].Dist)
                {
                    dist[other] = (newScore, cell);
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
        var queue = new PriorityQueue<TCell, Dist>();
        queue.Enqueue(start, new Dist(Heuristic(start), 0));
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
                var newScore = current + weight;
                if (!seen.Contains(other)) queue.Enqueue(other, new Dist(newScore + Heuristic(other), newScore));
                if (newScore < dist[other])
                {
                    dist[other] = newScore;
                }
            }
        }
        return -1;
    }
    
    public (int, TCell) ComputeFind(TCell start, Func<TCell, bool> target, Func<TCell, bool> valid = null)
    {
        valid ??= _ => true; 
        var dist = new DefaultDict<TCell, int> {DefaultValue = int.MaxValue, [start] = 0};
        var seen = new HashSet<TCell>();
        var queue = new PriorityQueue<TCell, Dist>();
        queue.Enqueue(start, new Dist(Heuristic(start), 0));
        while (queue.Count > 0)
        {
            var cell = queue.Dequeue();
            if (seen.Contains(cell)) continue;
            var current = dist[cell];
            if (target(cell)) return (current, cell);
            seen.Add(cell);
            foreach (var neighbor in Neighbors(cell))
            {
                var other = Cell(cell, neighbor);
                if (!valid(other)) continue;
                var weight = Distance(neighbor);
                var newScore = current + weight;
                if (!seen.Contains(other)) queue.Enqueue(other, new Dist(newScore + Heuristic(other), newScore));
                if (newScore < dist[other])
                {
                    dist[other] = newScore;
                }
            }
        }
        return (-1, default);
    }
}

// If the cell and neighbor types are the same,
// Then the cell function will be filled in automatically
public class Dijkstra<T> : Dijkstra<T, T>
{
    public Dijkstra()
    {
        Cell = (_, b) => b;
    }

    public Dijkstra(Func<T, IEnumerable<T>> neighbors)
    {
        Cell = (_, b) => b;
        Distance = _ => 1;
        Neighbors = neighbors;
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

    public static Dijkstra<T> ToDijkstra<T>(this ISpace<T> space) => new(space.GetNeighbors);

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
    
    public static (int, TCell) DijkstraFind<TCell, TMid>(this IDijkstra<TCell, TMid> dijkstra, TCell start, Func<TCell, bool> target, Func<TCell, bool> valid = null)
    {
        return dijkstra.Build().ComputeFind(start, target, valid);
    }

    public static Dictionary<TCell, (int Dist, TCell From)> DijkstraPath<TCell, TMid>(this IDijkstra<TCell, TMid> dijkstra, TCell start, TCell target, Func<TCell, bool> valid = null)
    {
        return dijkstra.Build().ComputePath(start, target, valid);
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