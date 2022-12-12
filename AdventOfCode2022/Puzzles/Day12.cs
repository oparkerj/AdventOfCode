using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2022.Puzzles;

public class Day12 : Puzzle<int>
{
    public Day12()
    {
        Part = 1;
    }

    public override int PartOne()
    {
        var map = Input.ToGrid();
        return map.ShortestPath('S', 'E', (from, to) => to - from <= 1 || to == 'E' || (from is 'S' && to <= 'b'));
    }
    
    public override int PartTwo()
    {
        var map = Input.ToGrid();
        map[map.Find('S')] = 'a';
        return map.ShortestPath('E', 'a', (from, to) => to - from >= -1);
    }
}







// First solution
// Part 1:
// var dist = map.DijkstraFind(start, map.Find('E'), (from, to) => map.Has(to) && (map[to] - map[from] <= 1 || map[to] == 'E'));

// Part 2:
// var dist = map.DijkstraFind(map.Find('E'), pos => map[pos] == 'a', (from, to) => map.Has(to) && (map[to] - map[from] >= -1 || map[to] == 'S' || map[from] == 'E'));
// return dist.Item1;