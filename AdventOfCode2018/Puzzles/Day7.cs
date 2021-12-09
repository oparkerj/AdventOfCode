using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Collections.Graph;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;
using MoreLinq;

namespace AdventOfCode2018.Puzzles;

public class Day7 : Puzzle
{
    public Day7()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var graph = Input.ToDigraph(s => s[5], s => s[36]);
            
        var queue = new SelfPriorityQueue<Vertex<char, DirectedEdge<char>>>(Comparing<Vertex<char, DirectedEdge<char>>>.By(vertex => vertex.Value));
        foreach (var vertex in graph.Where(vertex => !graph.HasIncomingEdges(vertex)))
        {
            queue.Enqueue(vertex);
        }
        while (queue.Count > 0)
        {
            var vertex = queue.Dequeue();
            Write(vertex.Value);
            var next = vertex.Neighbors.ToList();
            graph.RemoveVertex(vertex);
            foreach (var v in next.Where(v => !graph.HasIncomingEdges(v)))
            {
                queue.Enqueue(v);
            }
        }
        NewLine();
    }

    public override void PartTwo()
    {
        var graph = Input.ToDigraph(s => s[5], s => s[36]);
        var workers = new (int Time, Vertex<char, DirectedEdge<char>> Current)[5];

        var comparing = Comparing<Vertex<char, DirectedEdge<char>>>.By(vertex => vertex.Value);
        var queue = new SelfPriorityQueue<Vertex<char, DirectedEdge<char>>>(comparing);
        foreach (var vertex in graph.Where(vertex => !graph.HasIncomingEdges(vertex)))
        {
            queue.Enqueue(vertex);
        }

        var total = 0;
        while (true)
        {
            // Give jobs to workers
            while (queue.Count > 0 && workers.Any(worker => worker.Time == 0))
            {
                var vertex = queue.Dequeue();
                var worker = workers.Index().WhereValue(worker => worker.Time == 0).Keys().First();
                workers[worker] = (60 + vertex.Value - 'A' + 1, vertex);
            }
            // No work was given, done
            if (workers.Select(worker => worker.Time).AllEqual(0)) break;
            // When the next person completes, open up the possibility for any new components
            var time = workers.Select(worker => worker.Time).Where(i => i > 0).Min();
            total += time;
            foreach (var (i, (_, vertex)) in workers.Index().Where(pair => pair.Value.Time == time && pair.Value.Current != null))
            {
                var next = vertex.Neighbors.ToList();
                graph.RemoveVertex(vertex);
                foreach (var v in next.Where(v => !graph.HasIncomingEdges(v)))
                {
                    queue.Enqueue(v);
                }
                workers[i] = default;
            }
            // Subtract the completed time from workers that are still working
            for (var i = 0; i < workers.Length; i++)
            {
                if (workers[i].Time > 0) workers[i].Time -= time;
            }
        }
        WriteLn(total);
    }
}