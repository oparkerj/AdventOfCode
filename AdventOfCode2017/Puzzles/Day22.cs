using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2017.Puzzles
{
    public class Day22 : Puzzle
    {
        public Pos Carrier;
        public Pos Direction;

        public Day22()
        {
            Part = 2;
        }

        public override void PartOne()
        { 
            Grid<bool> map = Input.Select2D(c => c == '#').ToGrid();
            Carrier = map.Bounds.MidPos;
            Direction = Pos.Up;
            var infections = 0;
            
            void Step()
            {
                if (map[Carrier]) Direction = Direction.Clockwise();
                else Direction = Direction.CounterClockwise();
                infections = (map[Carrier] = !map[Carrier]) ? infections + 1 : infections;
                Carrier += Direction;
            }
            
            foreach (var _ in Enumerable.Range(0, 10000))
            {
                Step();
            }
            
            WriteLn(infections);
        }

        public override void PartTwo()
        {
            Grid<int> map = Input.Select2D(c => c == '#' ? 2 : 0).ToGrid();
            Carrier = map.Bounds.MidPos;
            Direction = Pos.Up;
            var infections = 0;

            void Step()
            {
                var state = map[Carrier];
                if (state == 0) Direction = Direction.CounterClockwise();
                else if (state == 2) Direction = Direction.Clockwise();
                else if (state == 3) Direction = -Direction;
                infections = (map[Carrier] = (state + 1) % 4) == 2 ? infections + 1 : infections;
                Carrier += Direction;
            }
            
            foreach (var _ in Enumerable.Range(0, 10_000_000))
            {
                Step();
            }
            
            WriteLn(infections);
        }
    }
}