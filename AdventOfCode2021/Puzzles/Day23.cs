using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Utilities;

namespace AdventOfCode2021.Puzzles;

public class Day23 : Puzzle
{
    public Day23()
    {
        Part = 2;
    }

    public record State(string Hallway, string A, string B, string C, string D)
    {
        public static State New(State from, char[] hall, int i, string hole)
        {
            return new State(new string(hall),
                i == 0 ? hole : from.A,
                i == 1 ? hole : from.B,
                i == 2 ? hole : from.C,
                i == 3 ? hole : from.D);
        }
        
        public string this[int i]
        {
            get
            {
                return i switch
                {
                    0 => A,
                    1 => B,
                    2 => C,
                    3 => D,
                    _ => null
                };
            }
        }
    }

    // Returns reachable whitespace of a string starting at the specified index.
    // This does not yield the starting position.
    public IEnumerable<int> Bfs(string s, int i)
    {
        var visited = new HashSet<int>();
        var queue = new Queue<int>();
        queue.Enqueue(i);
        visited.Add(i);
        while (queue.Count > 0)
        {
            var next = queue.Dequeue();
            var near = new[] {next - 1, next + 1}.Where(p => !visited.Contains(p) && p >= 0 && p < s.Length);
            foreach (var n in near)
            {
                if (char.IsWhiteSpace(s[n]))
                {
                    yield return n;
                    queue.Enqueue(n);
                    visited.Add(n);
                }
            }
        }
    }

    public Dijkstra<State, (State, int)> GetPathFinder(int size)
    {
        return new Dijkstra<State, (State, int)>
        {
            Neighbors = state =>
            {
                // Find all neighbors from the current state
                var possible = new List<(State, int)>();
                var entries = new[] {2, 4, 6, 8};
                // Add each way of taking an item out of a hole into the hallway
                foreach (var i in entries)
                {
                    var hole = state[i / 2 - 1];
                    if (string.IsNullOrWhiteSpace(hole)) continue;
                    var targets = Bfs(state.Hallway, i).Except(entries).ToList();
                    foreach (var target in targets)
                    {
                        var data = state.Hallway.ToCharArray();
                        data[target] = hole.Trim()[0];
                        var newHole = hole.Trim()[1..].PadLeft(size);
                        var next = State.New(state, data, i / 2 - 1, newHole);
                        var cost = Math.Abs(target - i) + (size - newHole.Trim().Length);
                        cost *= 10.Pow(data[target] - 'A');
                        possible.Add((next, cost));
                    }
                }
                // Add each way of moving an item from the hallway into a hole
                foreach (var (at, which) in state.Hallway.Indexed().WhereValue(char.IsLetter))
                {
                    var dest = which - 'A';
                    if (!Bfs(state.Hallway, at).Intersect(entries).Select(i => i / 2 - 1).Contains(dest)) continue;
                    if (state[dest].Trim().Any(c => c != which)) continue;
                    var data = state.Hallway.ToCharArray();
                    data[at] = ' ';
                    var next = State.New(state, data, dest, (which + state[dest].Trim()).PadLeft(size));
                    var cost = Math.Abs(at - (dest + 1) * 2) + (size - state[dest].Trim().Length);
                    cost *= 10.Pow(dest);
                    possible.Add((next, cost));
                }
                return possible;
            },
            Distance = tuple => tuple.Item2,
            Cell = (_, tuple) => tuple.Item1
        };
    }

    public override void PartOne()
    {
        var input = Input.Flatten().Where(char.IsLetter).Str();
        var a = $"{input[0]}{input[4]}";
        var b = $"{input[1]}{input[5]}";
        var c = $"{input[2]}{input[6]}";
        var d = $"{input[3]}{input[7]}";
        var start = new State("           ", a, b, c, d);
        var dest = new State("           ", "AA", "BB", "CC", "DD");
        
        var cost = GetPathFinder(2).ComputeFind(start, dest);
        WriteLn(cost);
    }

    public override void PartTwo()
    {
        var input = Input.Flatten().Where(char.IsLetter).Str();
        var a = $"{input[0]}DD{input[4]}";
        var b = $"{input[1]}CB{input[5]}";
        var c = $"{input[2]}BA{input[6]}";
        var d = $"{input[3]}AC{input[7]}";
        var start = new State("           ", a, b, c, d);
        var dest = new State("           ", "AAAA", "BBBB", "CCCC", "DDDD");
        
        var cost = GetPathFinder(4).ComputeFind(start, dest);
        WriteLn(cost);
    }
}