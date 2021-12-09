using System;
using System.Collections.Generic;
using AdventOfCode2019.IntCode;
using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2019.Puzzles;

public class Day15 : Puzzle
{
    public const int Wall = 0;
    public const int Empty = 1;
    public const int Oxygen = 2;
        
    public readonly Grid<int> Map = new();

    public Day15()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var droid = new RepairDroid(InputLine, Map);
        Map[droid.Position] = Empty;
        droid.ExploreAll(Map.Has);
        var dist = Map.ShortestPathBfs(droid.Position, Map.Find(Oxygen), pos => Map[pos] != Wall);
        WriteLn(dist);
    }

    public override void PartTwo()
    {
        var droid = new RepairDroid(InputLine, Map);
        Map[droid.Position] = Empty;
        droid.ExploreAll(Map.Has);
        var max = Map.LongestPathBfs(Map.Find(Oxygen), pos => Map[pos] != Wall);
        WriteLn(max);
    }

    public class RepairDroid : IExploreDrone<Pos, int>
    {
        public AlignedSpace<Pos, int> Map;
            
        private Computer _controller;
        private DataLink _data;

        public RepairDroid(string program, AlignedSpace<Pos, int> map)
        {
            Map = map;
            _controller = Computer.From(program);
            _data = new DataLink(_controller);
        }

        public Pos Position { get; set; }
            
        public IEnumerable<Pos> GetNeighbors() => Map.GetNeighbors(Position);

        public bool TryMove(Pos offset, out int sense)
        {
            var dir = offset switch
            {
                (0, 1) => 1,
                (0, -1) => 2,
                (-1, 0) => 3,
                (1, 0) => 4,
                _ => throw new Exception("Invalid offset")
            };
            _data.Insert(dir);
            sense = _controller.NextInt();
            if (sense != Wall)
            {
                Position += offset;
                Map[Position] = sense;
                return true;
            }
            Map[Position + offset] = sense;
            return false;
        }
    }
}