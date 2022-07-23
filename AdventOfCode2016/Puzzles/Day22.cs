using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;
using RegExtract;

namespace AdventOfCode2016.Puzzles;

public class Day22 : Puzzle
{
    public Grid<Node> Grid = new();

    public void ReadInput()
    {
        var nodes = Input[2..].Extract<(int, int, int, int)>(@"x(\d+)-y(\d+).+?(\d+).+?(\d+)");
        foreach (var (x, y, size, used) in nodes)
        {
            Grid[x, y] = new Node {Size = size, Used = used};
        }
    }

    public bool CheckPair(Node a, Node b) => ViablePair(a, b) || ViablePair(b, a);

    public bool ViablePair(Node a, Node b) => a.Used != 0 && b.Available >= a.Used;

    public override void PartOne()
    {
        ReadInput();
        var pairs = Grid.Values.ToList().Pairs().SpreadSelect(CheckPair).Count(true);
        WriteLn(pairs);
    }

    public bool ValidMove(Pos pos) => Grid[pos]?.Size < 100;

    public override void PartTwo()
    {
        ReadInput();
        var hole = Grid.First(pair => pair.Value.Used == 0).Key;
        var data = Grid.Bounds.DiagMaxMin;
        var setupDist = Grid.DijkstraFind(hole, data, ValidMove);
        // It happens to be a straight shot from the goal data to our node.
        // Number of steps to get the empty drive to the goal data, and it takes 5
        // steps to "walk" around the goal data and move it forward by one.
        var dist = setupDist + (data.MDist(Pos.Zero) - 1) * 5;
        WriteLn(dist);
    }

    public class Node
    {
        public int Size;
        public int Used;

        public int Available => Size - Used;
    }
}